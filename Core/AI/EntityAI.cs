using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrotera.Core.AI
{
    public class EntityAI
    {
        public static Dictionary<string, IEntiyAIFactory> Factories = new Dictionary<string, IEntiyAIFactory>();

        public static void AddFactory(string name, IEntiyAIFactory factory)
        {
            string nameUpper = name.ToUpperInvariant();
            if (Factories.ContainsKey(nameUpper))
                Factories[nameUpper] = factory;
            else
                Factories.Add(nameUpper, factory);
        }

        public static EntityAI Create(string name, Entity entity)
        {
            string nameUpper = name.ToUpperInvariant();
            if (Factories.ContainsKey(nameUpper))
            {
                EntityAI ai = Factories[nameUpper].CreateEntityAI(entity);
                ai.Startup(entity);
            }

            return new EntityAI();
        }

        public Entity LinkedEntity = null;

        protected void Startup(Entity entity)
        {
            LinkedEntity = entity;
            Init();
        }

        public virtual void Init()
        {
        }

        public virtual void Update(float deltaT, float time)
        {
        }
    }

    public interface IEntiyAIFactory
    {
        EntityAI CreateEntityAI(Entity entity);
    }
}
