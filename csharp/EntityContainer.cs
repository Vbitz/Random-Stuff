using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntitySystem
{
    public enum EntityRequestTypes
    {
        Add,
        Remove
    }

    internal class EntityRequest
    {
        public Entity Target;
        public EntityRequestTypes RequestType;

        public EntityRequest(Entity target, EntityRequestTypes type)
        {
            this.Target = target;
            this.RequestType = type;
        }
    }

    public class EntityContainer : Entity
    {
        protected List<Entity> Entitys = new List<Entity>();
        private List<EntityRequest> PendingRequests = new List<EntityRequest>();

        public void RemoveAll()
        {
            foreach (Entity item in this.Entitys)
            {
                this.AddRequest(item, EntityRequestTypes.Remove);
            }
        }

        public void AddRequest(Entity item, EntityRequestTypes type)
        {
            this.PendingRequests.Add(new EntityRequest(item, type));
        }

        public void DoRequests()
        {
            foreach (EntityRequest item in this.PendingRequests)
            {
                switch (item.RequestType)
                {
                    case EntityRequestTypes.Add:
                        this.Entitys.Add(item.Target);
                        break;
                    case EntityRequestTypes.Remove:
                        this.Entitys.Remove(item.Target);
                        break;
                }
            }
        }

    }
}
