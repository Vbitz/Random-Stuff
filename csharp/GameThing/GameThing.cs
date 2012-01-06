using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using SFML;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

using NetDrawing = System.Drawing;

namespace GameThing
{
    class Item
    {
        private Sprite BasicSprite;

        protected void LoadImage(string filename)
        {
            this.BasicSprite = ResourceManager.GetImage(filename);
        }

        public virtual void PreLight(MainGame game, int x, int y)
        {

        }

        public virtual void Draw(MainGame game, int x, int y)
        {
            if (this.BasicSprite != null)
            {
                this.BasicSprite.Position = new Vector2(x, y);
                game.Draw(this.BasicSprite);
            }
        }

        public virtual void Hit()
        {

        }

        public virtual bool Colide()
        {
            return false;
        }

        public virtual bool CanColide()
        {
            return false;
        }

        public virtual bool CanDig()
        {
            return false;
        }

        public virtual void Dig()
        {

        }
    }

    class Light : Item
    {
        public override void PreLight(MainGame game, int x, int y)
        {
            game.AddLight(x, y);
        }
    }

    class Door : Item
    {
        private bool Opened = false;

        public Door()
        {
            this.LoadImage("door.png");
        }

        public override bool CanColide()
        {
            return true;
        }

        public override bool Colide()
        {
            return !Opened;
        }

        public override bool CanDig()
        {
            return true;
        }

        public override void Dig()
        {
            this.Opened = !this.Opened;
        }

        public override void Draw(MainGame game, int x, int y)
        {
            base.Draw(game, x, y);
        }
    }

    class Tile
    {
        Stack<Item> TopItem = new Stack<Item>();
        BasicTile BaseTile = null;
        List<Item> EngieLayer = new List<Item>();

        public virtual void Draw(MainGame game, int x, int y, int light)
        {
            foreach (Item item in this.EngieLayer)
            {
                item.Draw(game, x, y);
            }
            if (TopItem.Count > 0)
            {
                TopItem.Peek().Draw(game, x, y);
            }
            else
            {
                if (this.BaseTile != null)
                {
                    this.BaseTile.Draw(game, x, y, light);
                }
            }

        }

        public virtual bool Colide()
        {
            if (TopItem.Count > 0 && TopItem.Peek().CanColide())
            {
                return TopItem.Peek().Colide();
            }
            if (this.BaseTile != null)
            {
                return this.BaseTile.Colide();
            }
            return false;
        }

        public virtual bool Transperent()
        {
            if (this.BaseTile != null)
            {
                return this.BaseTile.Transperent();
            }
            return false;
        }

        public virtual void PreLight(MainGame game, int x, int y)
        {
            foreach (Item item in this.TopItem)
            {
                item.PreLight(game, x, y);
            }
            foreach (Item item in this.EngieLayer)
            {
                item.PreLight(game, x, y);
            }
        }

        public virtual bool Dig()
        {
            if (TopItem.Count > 0 && TopItem.Peek().CanDig())
            {
                TopItem.Peek().Dig();
                return true;
            }
            return false;
        }

        public virtual void Hit()
        {
            foreach (Item item in this.TopItem)
            {
                item.Hit();
            }
        }

        internal void SetBasicTile(BasicTile tile)
        {
            this.BaseTile = tile;
        }

        public void AddItemToTop(Item item)
        {
            this.TopItem.Push(item);
        }
    }

    class BasicTile : Tile
    {
        Sprite BasicSprite = null;

        Color[] lightLevels = new Color[] {
            new Color(0,0,0,0),
            new Color(0,0,0,50),
            new Color(0,0,0,100),
            new Color(0,0,0,150),
            new Color(0,0,0,200),
        };

        protected void LoadImage(string filename)
        {
            this.BasicSprite = ResourceManager.GetImage(filename);
        }

        public override void Draw(MainGame game, int x, int y, int light)
        {
            if (this.BasicSprite != null)
            {
                this.BasicSprite.Position = new Vector2(x, y);
                game.Draw(this.BasicSprite);
            }
            game.Draw(Shape.Rectangle(new Vector2(x, y), new Vector2(x + MainGame.TileSize, y + MainGame.TileSize), lightLevels[light]));
        }
    }

    class FloorTile : BasicTile
    {

        public FloorTile()
        {
            this.LoadImage("floor.png");
        }

        public override bool Transperent()
        {
            return true;
        }
    }

    class WallTile : BasicTile
    {

        public WallTile()
        {
            this.LoadImage("wall.png");
        }

        public override bool Colide()
        {
            return true;
        }
    }

    static class ResourceManager
    {
        static Dictionary<string, Sprite> LoadedImages = new Dictionary<string, Sprite>();

        public static void LoadImage(string filename)
        {
            LoadedImages.Add(filename, new Sprite(new Image(MainGame.BasePath + "\\" + filename)));
        }

        public static Sprite GetImage(string filename)
        {
            return LoadedImages[filename];
        }
    }

    class GameConsole
    {
        private String2D TextString = null;
        private Shape Background = Shape.Rectangle(new Vector2(0, 16 * MainGame.TileSize - 32), new Vector2(16 * MainGame.TileSize, 16 * MainGame.TileSize), new Color(255, 255, 255, 50));

        public bool Active = false;

        public GameConsole()
        {
            this.TextString = new String2D();
            this.TextString.Color = Color.White;
            this.TextString.Size = 16;
            this.TextString.Position = new Vector2(10, 16 * MainGame.TileSize - 28);
        }

        public void DoString(string str, MainGame game)
        {
            this.Print("> " + str);
            string[] tokens = str.Split(' ');
            switch (tokens[0])
            {
                case "exit":
                    game.Running = false;
                    break;
                case "torch":
                    this.Print("Adding Torch");
                    game.GetPlayerTile().AddItemToTop(new Light());
                    game.NeedsLight = true;
                    break;
                case "door":
                    this.Print("Adding Door");
                    game.GetPlayerTile().AddItemToTop(new Door());
                    game.NeedsLight = true;
                    break;
                case "noclip":
                    game.NoClip = !game.NoClip;
                    this.Print("Noclip: " + game.NoClip.ToString());
                    break;
                case "alllight":
                    game.AllLight = !game.AllLight;
                    this.Print("AllLight: " + game.AllLight.ToString());
                    break;
                case "engie":

                    break;
                case "help":
                    this.Print(@" exit: exit the game
 torch: place a torch
 door: place a door
 noclip: disable colision
 alllight: disable lighting
 cleartop: remove all top items from the thing you are standing on
 clearengie: remove all engie layer items from the thing you are standing on
 engie: show the engering layer
 help: print this message;
");
                    break;
                default:
                    this.Print("Command does not exist");
                    break;
            }
        }

        public void Print(string str)
        {
            // move this to a ingame console, maybe
            Console.WriteLine(str);
        }

        public void Draw(MainGame game)
        {
            if (this.Active)
            {
                game.Draw(this.Background);
            }
            game.Draw(this.TextString);
        }

        public void KeyPressed(MainGame game, KeyCode code, char keyChar)
        {
            if (code == KeyCode.F12)
            {
                this.Active = !this.Active;
            }

            if (this.Active)
            {
                if (code == KeyCode.Return)
                {
                    this.DoString(this.TextString.Text, game);
                    this.TextString.Text = "";
                }
                else if (code == KeyCode.Back)
                {
                    string str = this.TextString.Text;
                    if (str.Length > 0)
                    {
                        str = str.Substring(0, str.Length - 1);
                        this.TextString.Text = str;
                    }
                }
                else if (code == KeyCode.F12)
                {
                    return;
                }
                else
                {
                    if (TextString.Text.Length < 20)
                    {
                        TextString.Text += keyChar;
                    }
                }
            }
        }
    }

    static class TextInput
    {
        private static Dictionary<KeyCode, char> Items = new Dictionary<KeyCode, char>();

        public static void Init()
        {
            Items.Add(KeyCode.A, 'a');
            Items.Add(KeyCode.B, 'b');
            Items.Add(KeyCode.C, 'c');
            Items.Add(KeyCode.D, 'd');
            Items.Add(KeyCode.E, 'e');
            Items.Add(KeyCode.F, 'f');
            Items.Add(KeyCode.G, 'g');
            Items.Add(KeyCode.H, 'h');
            Items.Add(KeyCode.I, 'i');
            Items.Add(KeyCode.J, 'j');
            Items.Add(KeyCode.K, 'k');
            Items.Add(KeyCode.L, 'l');
            Items.Add(KeyCode.M, 'm');
            Items.Add(KeyCode.N, 'n');
            Items.Add(KeyCode.O, 'o');
            Items.Add(KeyCode.P, 'p');
            Items.Add(KeyCode.Q, 'q');
            Items.Add(KeyCode.R, 'r');
            Items.Add(KeyCode.S, 's');
            Items.Add(KeyCode.T, 't');
            Items.Add(KeyCode.U, 'u');
            Items.Add(KeyCode.V, 'v');
            Items.Add(KeyCode.W, 'w');
            Items.Add(KeyCode.X, 'x');
            Items.Add(KeyCode.Y, 'y');
            Items.Add(KeyCode.Z, 'z');
            Items.Add(KeyCode.Num0, '0');
            Items.Add(KeyCode.Num1, '1');
            Items.Add(KeyCode.Num2, '2');
            Items.Add(KeyCode.Num3, '3');
            Items.Add(KeyCode.Num4, '4');
            Items.Add(KeyCode.Num5, '5');
            Items.Add(KeyCode.Num6, '6');
            Items.Add(KeyCode.Num7, '7');
            Items.Add(KeyCode.Num8, '8');
            Items.Add(KeyCode.Num9, '9');
            Items.Add(KeyCode.Space, ' ');
        }

        public static char GetKey(KeyCode code)
        {
            if (Items.ContainsKey(code))
            {
                return Items[code];
            }
            else
            {
                return ' ';
            }
        }
    }

    class MainGame : RenderWindow
    {
        // I preformince tested these variables
        // 8192 is too big by a long shot, vis will exaust all memory
        // otherwise the rendering is done so it only works with stuff on screen
        // so you can do up to 4096 with no lag on a mid high end laptop

        // Consts
        public const int TileWidth = 1024;
        public const int TileHeight = 1024;
        public const int TileSize = 32;

        public const int OffsetX = 7;
        public const int OffsetY = 7;

        public const string BasePath = "..\\..\\Data";

        // stuff
        public Random rand = new Random();
        public bool Running = true;

        // cheats
        public bool AllLight = false;
        public bool NoClip = false;

        // tile stuff
        private Tile[,] Tiles = new Tile[TileWidth, TileHeight];

        // Player Stuff
        public int PlayerTileX = 0;
        public int PlayerTileY = 0;

        // Input Stuff
        private List<KeyCode> KeysPressedThisFrame = new List<KeyCode>();

        // light Stuff
        private int[,] CurrentLight = null;
        public bool NeedsLight = true;

        // console Stuff
        private GameConsole console = new GameConsole();



        public MainGame()
            : base(new VideoMode(16 * TileSize, 16 * TileSize), "It's not hard but it's a game")
        {
            ResourceManager.LoadImage("floor.png");
            ResourceManager.LoadImage("wall.png");
            ResourceManager.LoadImage("door.png");

            TextInput.Init();

            for (int x = 0; x < TileWidth; x++)
            {
                for (int y = 0; y < TileHeight; y++)
                {
                    this.Tiles[x, y] = new Tile();
                }
            }

            this.GenerateBasicMap();
        }

        static void Main(string[] args)
        {
            MainGame game = new MainGame();
            game.Run();
            Process.GetCurrentProcess().Kill();
        }

        public void Run()
        {
            while (this.Running)
            {
                this.KeysPressedThisFrame.Clear();

                Event evnt = default(Event);

                while (this.GetEvent(out evnt))
                {
                    switch (evnt.Type)
                    {
                        case EventType.Closed:
                            this.Close();
                            return;
                        case EventType.KeyPressed:
                            this.KeysPressedThisFrame.Add(evnt.Key.Code);
                            this.console.KeyPressed(this, evnt.Key.Code, TextInput.GetKey(evnt.Key.Code));
                            break;
                        case EventType.MouseButtonPressed:
                            if (evnt.MouseButton.Button == MouseButton.Left)
                            {
                                this.DoMouseClick(false, evnt.MouseButton.X, evnt.MouseButton.Y);
                            }
                            else if (evnt.MouseButton.Button == MouseButton.Right)
                            {
                                this.DoMouseClick(true, evnt.MouseButton.X, evnt.MouseButton.Y);
                            }
                            break;
                        case EventType.KeyReleased:
                            break;
                        default:
                            break;
                    }
                }

                this.Clear(SFML.Graphics.Color.Black);

                if (this.NeedsLight)
                {
                    this.ClearLight();
                    this.AddLight(this.PlayerTileX + OffsetX, this.PlayerTileY + OffsetY);
                    this.PreLight();
                    this.NeedsLight = false;
                }

                this.DrawMap();

                this.DoPlayer();

                this.console.Draw(this);

                this.Display();
            }
            if (this.IsOpened())
            {
                this.Close();
            }
        }

        public void SetTile(int x, int y, BasicTile tile)
        {
            this.Tiles[x, y].SetBasicTile(tile);
        }

        //public bool KeyPressed(KeyCode code, bool repeat)
        public bool GameKeyPressed(KeyCode code)
        {
            return this.KeysPressedThisFrame.Contains(code);
        }

        public void GenerateBasicMap()
        {
            NetDrawing.Bitmap img = new NetDrawing.Bitmap(TileWidth, TileHeight);
            NetDrawing.Graphics gra = NetDrawing.Graphics.FromImage(img);

            gra.FillRectangle(NetDrawing.Brushes.Black, new NetDrawing.Rectangle(0, 0, 20, 20));

            for (int i = 0; i < 10000; i++)
            {
                int size = rand.Next(6, 16);
                gra.FillEllipse(NetDrawing.Brushes.Black, new NetDrawing.Rectangle(rand.Next(TileWidth), rand.Next(TileHeight), size, size));
            }

            for (int x = 0; x < TileWidth; x++)
            {
                for (int y = 0; y < TileHeight; y++)
                {
                    NetDrawing.Color col = img.GetPixel(x, y);
                    //Console.WriteLine(col.A);
                    //Console.WriteLine(col.ToKnownColor().ToString());
                    if (col.A == 255)
                    {
                        this.SetTile(x, y, new FloorTile());
                    }
                    else
                    {
                        this.SetTile(x, y, new WallTile());
                    }
                }
            }
        }

        public void PreLight()
        {
            for (int x = PlayerTileX - 4; x < PlayerTileX + 16 + 8; x++)
            {
                for (int y = PlayerTileY - 4; y < PlayerTileY + 16 + 8; y++)
                {
                    if (this.CheckBounds(x, y))
                    {
                        this.Tiles[x, y].PreLight(this, x, y);
                    }
                }
            }
        }

        public void DrawMap()
        {
            for (int x = PlayerTileX; x < PlayerTileX + 16; x++)
            {
                for (int y = PlayerTileY; y < PlayerTileY + 16; y++)
                {
                    if (this.CheckBounds(x, y))
                    {
                        if (!this.AllLight)
                        {
                            if (this.CurrentLight[x, y] < 5)
                            {
                                this.Tiles[x, y].Draw(this, (x - PlayerTileX) * TileSize, (y - PlayerTileY) * TileSize, this.CurrentLight[x, y]);
                            }
                        }
                        else
                        {
                            this.Tiles[x, y].Draw(this, (x - PlayerTileX) * TileSize, (y - PlayerTileY) * TileSize, 0);
                        }
                    }
                }
            }
        }

        private bool CheckColision(int x, int y)
        {
            return !this.Tiles[x, y].Colide();
        }

        public void DoPlayer()
        {
            if (!this.console.Active)
            {
                if (this.GameKeyPressed(KeyCode.W) && this.PlayerTileY + OffsetY > 0)
                {
                    if (this.CheckColision(PlayerTileX + OffsetX, PlayerTileY + OffsetY - 1))
                    {
                        this.PlayerTileY--;
                        this.NeedsLight = true;
                    }
                    else
                    {
                        this.Tiles[PlayerTileX + OffsetX, PlayerTileY + OffsetY - 1].Hit();
                    }
                }
                if (this.GameKeyPressed(KeyCode.S) && this.PlayerTileY + OffsetY < TileHeight - 1)
                {
                    if (this.CheckColision(PlayerTileX + OffsetX, PlayerTileY + OffsetY + 1))
                    {
                        this.PlayerTileY++;
                        this.NeedsLight = true;
                    }
                    else
                    {
                        this.Tiles[PlayerTileX + OffsetX, PlayerTileY + OffsetY + 1].Hit();
                    }
                }
                if (this.GameKeyPressed(KeyCode.A) && this.PlayerTileX + OffsetX > 0)
                {
                    if (this.CheckColision(PlayerTileX + OffsetX - 1, PlayerTileY + OffsetY))
                    {
                        this.PlayerTileX--;
                        this.NeedsLight = true;
                    }
                    else
                    {
                        this.Tiles[PlayerTileX + OffsetX - 1, PlayerTileY + OffsetY].Hit();
                    }
                }
                if (this.GameKeyPressed(KeyCode.D) && this.PlayerTileX + OffsetX < TileWidth - 1)
                {
                    if (this.CheckColision(PlayerTileX + OffsetX + 1, PlayerTileY + OffsetY))
                    {
                        this.PlayerTileX++;
                        this.NeedsLight = true;
                    }
                    else
                    {
                        this.Tiles[PlayerTileX + OffsetX + 1, PlayerTileY + OffsetY].Hit();
                    }
                }
            }

            this.Draw(Shape.Rectangle(new Vector2(7 * TileSize + 4, 7 * TileSize + 4),
                new Vector2(7 * TileSize + 30, 7 * TileSize + 30),
                new Color(255, 100, 100)));
        }

        private void DoMouseClick(bool place, int x, int y)
        {
            int worldx = (x / TileSize) + this.PlayerTileX;
            int worldy = (y / TileSize) + this.PlayerTileY;

            if (this.CheckBounds(worldx, worldy) && this.CurrentLight[worldx, worldy] < 5)
            {
                if (this.Tiles[worldx, worldy].Dig())
                {

                }
                else
                {
                    if (place)
                    {
                        this.Tiles[worldx, worldy].SetBasicTile(new WallTile());
                    }
                    else
                    {
                        this.Tiles[worldx, worldy].SetBasicTile(new FloorTile());
                    }
                }
                this.NeedsLight = true;
            }

            Console.WriteLine(worldx + " " + worldy);
        }

        private void ClearLight()
        {
            this.CurrentLight = new int[TileWidth, TileHeight];

            for (int x2 = 0; x2 < TileWidth; x2++)
            {
                for (int y2 = 0; y2 < TileHeight; y2++)
                {
                    this.CurrentLight[x2, y2] = 5;
                }
            }
        }

        public void AddLight(int x, int y)
        {
            bool[,] tilesChanged = new bool[TileWidth, TileHeight];

            List<Vector2> tileLocations = new List<Vector2>();

            tileLocations.Add(new Vector2(x, y));

            for (int i = 0; i < 5; i++)
            {
                List<Vector2> newLocations = new List<Vector2>();

                foreach (Vector2 item in tileLocations)
                {
                    if (!this.CheckBounds((int)item.X, (int)item.Y) || tilesChanged[(int)item.X, (int)item.Y])
                    {
                        continue;
                    }
                    this.CurrentLight[(int)item.X, (int)item.Y] = CalcVis((int)item.X, (int)item.Y, i);
                    tilesChanged[(int)item.X, (int)item.Y] = true;

                    CheckLight(item, 1, 0, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, -1, 0, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, 0, 1, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, 0, -1, newLocations, ref this.CurrentLight, i);

                    CheckLight(item, 1, 1, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, 1, -1, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, -1, 1, newLocations, ref this.CurrentLight, i);
                    CheckLight(item, -1, -1, newLocations, ref this.CurrentLight, i);
                }

                tileLocations = newLocations;
            }
        }

        public void CheckLight(Vector2 bse, int xoff, int yoff, List<Vector2> newLocations, ref int[,] tiles, int level)
        {
            if (this.CheckBounds((int)bse.X + xoff, (int)bse.Y + yoff))
            {
                if (this.Tiles[(int)bse.X + xoff, (int)bse.Y + yoff].Transperent())
                {
                    newLocations.Add(new Vector2(bse.X + xoff, bse.Y + yoff));
                }
                else
                {
                    if (level < 5)
                    {
                        tiles[(int)bse.X + xoff, (int)bse.Y + yoff] = CalcVis((int)bse.X + xoff, (int)bse.Y + yoff, level);
                    }
                }
            }
        }

        public int CalcVis(int x, int y, int level)
        {
            if (this.CurrentLight[x, y] == 5)
            {
                return level;
            }
            if (this.CurrentLight[x, y] - level < 0)
            {
                return 0;
            }
            else
            {
                return this.CurrentLight[x, y] - level;
            }
        }

        public bool CheckBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < TileWidth && y < TileHeight;
        }

        public Tile GetPlayerTile()
        {
            return this.Tiles[this.PlayerTileX + OffsetX, this.PlayerTileY + OffsetY];
        }
    }
}
