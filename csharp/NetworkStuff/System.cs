using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemsGame
{
    class FilePremission
    {
        public bool Read;
        public bool Write;

        public FilePremission(bool read, bool write)
        {
            this.Read = read;
            this.Write = write;
        }
    }

    class File
    {
        public string Name;
        public int Size;
        public bool Directory;
        public User Creator;
        public List<File> Children = new List<File>();
        public Dictionary<User, FilePremission> Premitions = new Dictionary<User, FilePremission>();
        public Dictionary<UserGroup, FilePremission> GroupPremitions = new Dictionary<UserGroup, FilePremission>();

        public File(string name, int size, User owner)
        {
            this.Name = name;
            this.Size = size;
            this.Creator = owner;
        }

        public bool CanView(User usr)
        {
            if (usr == this.Creator)
            {
                return true;
            }
            if (this.Premitions.ContainsKey(usr))
            {
                return this.Premitions[usr].Read;
            }
            foreach (UserGroup item in usr.Membership)
            {
                if (this.GroupPremitions.ContainsKey(item))
                {
                    return this.GroupPremitions[item].Read;
                }
            }
            return false;
        }

        public File GetDirectory(string name, bool create, User owner)
        {
            foreach (File item in this.Children)
            {
                if (item.Name == name && item.Directory)
                {
                    return item;
                }
            }
            if (create)
            {
                File newFile = new File(name, 0, owner);
                newFile.Directory = true;
                this.Children.Add(newFile);
                return newFile;
            }
            else
            {
                return null;
            }
        }
    }

    class UserGroup
    {
        public string Name;

        public UserGroup(string name)
        {
            this.Name = name;
        }
    }

    class User
    {
        public string Name = Singiltons.GetRandomWord(8);
        public string Password = Singiltons.GetRandomWord(8);
        public ComputerSystem Parent;

        public List<UserGroup> Membership = new List<UserGroup>();

        public User(ComputerSystem parent)
        {
            this.Parent = parent;
        }

        public User(ComputerSystem parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Parent.Name + "\\\\" + this.Name;
        }
    }

    class Session
    {
        public Session(User currentUser)
        {
            this.CurrentUser = currentUser;
        }

        public User CurrentUser;
    }

    class ComputerSystem
    {
        public int ID = 0;
        public string Name = Singiltons.GetRandomWord(12);
        public Network Parent;

        public File RootFile;

        public bool DomainControler = false;

        public List<User> Users = new List<User>();
        public List<UserGroup> Groups = new List<UserGroup>();

        public bool Standalone = true;

        public User RootUser;

        public ComputerSystem()
        {
            this.RootUser = new User(this, "SYSTEM");
            this.Users.Add(this.RootUser);

            this.RootFile = new File("Root", 0, this.RootUser);
            this.RootFile.Directory = true;
            this.GenerateUsers();

            FileSystemGenerator.GenerateBasicFilesystem(this, new Session(this.RootUser));
        }

        public ComputerSystem(Network parent)
        {
            this.Parent = parent;
            if (this.Parent.DomainControler != null)
            {
                this.Standalone = false;
            }
            else
            {
                this.GenerateUsers();
            }

            this.RootUser = new User(this, "SYSTEM");
            this.Users.Add(this.RootUser);

            this.RootFile = new File("Root", 0, this.RootUser);
            this.RootFile.Directory = true;
            FileSystemGenerator.GenerateBasicFilesystem(this, new Session(this.RootUser));
        }

        public bool CanRead(Session sess, File item)
        {
            if (sess.CurrentUser == this.RootUser)
            {
                return true;
            }
            return item.CanView(sess.CurrentUser);
        }

        public UserGroup GetGroup(string name)
        {
            if (this.DomainControler || this.Standalone)
            {
                foreach (UserGroup item in this.Groups)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                return this.Parent.DomainControler.GetGroup(name);
            }
        }

        public Session TryLogin(string name, string password)
        {
            User target = this.GetUser(name);
            if (target == null)
            {
                return null;
            }
            if (target.Password == password)
            {
                return new Session(target);
            }
            return null;
        }

        public void SetDomainControler()
        {
            this.DomainControler = true;
        }

        public void GenerateUsers()
        {
            this.Groups.Add(new UserGroup("Admins"));

            User admin = new User(this, "Admin");
            admin.Membership.Add(this.GetGroup("Admins"));
            this.Users.Add(admin);
        }

        public User GetUser(string name)
        {
            if (name == "SYSTEM" || this.DomainControler || this.Standalone)
            {
                foreach (User item in this.Users)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
            else
            {
                return this.Parent.DomainControler.GetUser(name);
            }
        }

        public void FSCreateDirectory(string name, Session sess)
        {
            Console.WriteLine(this.Parent.Name + "\\\\" + this.Name + " : Creating Directory : " + name);
            string[] items = name.Split('\\');
            File currentDir = this.RootFile;
            for (int i = 0; i < items.Length; i++)
            {
                currentDir = currentDir.GetDirectory(items[i], true, sess.CurrentUser);
            }
        }

        public File FSGetDirectory(string name, Session sess)
        {
            string[] items = name.Split('\\');
            File currentDir = this.RootFile;
            for (int i = 0; i < items.Length - 1; i++)
            {
                File target = currentDir.GetDirectory(items[i], false, sess.CurrentUser);
                if (target != null)
                {
                    currentDir = target;
                }
                else
                {
                    throw new Exception("Directory not Found");
                }
            }
            return currentDir;
        }

        public void FSCreateFile(string path, string name, int size, Session sess)
        {
            File currentDirectory = this.FSGetDirectory(path, sess);
            File newFile = new File(name, size, sess.CurrentUser);
            currentDirectory.Children.Add(newFile);
        }

        public void FSSweepPremissions(string path, bool subdirectorys, UserGroup toJoin, FilePremission newPremission, Session sess)
        {
            File rootBase;
            if (path == "")
            {
                rootBase = this.RootFile;
            }
            else
            {
                rootBase = this.FSGetDirectory(path, sess);
            }
            this.FSSetPremission(rootBase, toJoin, newPremission, sess);
            this.FSSweepPremissions(rootBase, subdirectorys, toJoin, newPremission, sess);
        }

        public void FSSweepPremissions(File path, bool subdirectorys, UserGroup toJoin, FilePremission newPremission, Session sess)
        {
            foreach (File item in path.Children)
            {
                if (item.Directory)
                {
                    if (!subdirectorys)
                    {
                        continue;
                    }
                    this.FSSetPremission(item, toJoin, newPremission, sess);
                    this.FSSweepPremissions(item, subdirectorys, toJoin, newPremission, sess);
                }
                else
                {
                    this.FSSetPremission(item, toJoin, newPremission, sess);
                }
            }
        }

        public void FSSetPremission(File item, UserGroup toJoin, FilePremission newPremission, Session sess)
        {
            if (item.CanView(sess.CurrentUser))
            {
                if (item.GroupPremitions.ContainsKey(toJoin))
                {
                    item.GroupPremitions[toJoin] = newPremission;
                }
                else
                {
                    item.GroupPremitions.Add(toJoin, newPremission);
                }
            }
        }
    }
}
