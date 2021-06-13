using System.Collections.Generic;
using Match3.ECS.Entities;
using Microsoft.Xna.Framework;

namespace Match3.ECS.Systems
{
    public class ScriptSystem
    {
        private List<CustomComponent> components = new List<CustomComponent>();
        private List<CustomComponent> toRemove = new List<CustomComponent>();

        private bool hasStarted = false;

        public ScriptSystem()
        {
            EntityManager.Instance.OnComponentAdded += Instance_OnComponentAdded;
            EntityManager.Instance.OnComponentRemoved += Instance_OnComponentRemoved;
            EntityManager.Instance.OnEntityRemoved += Instance_OnEntityRemoved;
        }

        private void Instance_OnEntityRemoved(Entity obj)
        {
            var comps = components.FindAll(c => c.entityId == obj.entityID);

            foreach (var item in comps)
            {
                toRemove.Add(item);
            }
        }

        private void Instance_OnComponentRemoved(Entity arg1, Component arg2)
        {
        }

        private void Instance_OnComponentAdded(Entity arg1, Component arg2)
        {
            if (arg2 is CustomComponent customComponent)
            {
                if (!components.Contains(customComponent))
                {
                    components.Add(customComponent);

                    customComponent.Start();
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            for (var i = 0; i < components.Count; i++)
            {
                components[i].Update(gameTime);
            }

            HandleRemove();
        }

        private void HandleRemove()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < toRemove.Count; i++)
            {
                components.Remove(toRemove[i]);
                toRemove[i].Destroy();
            }

            toRemove.Clear();
        }
    }
}