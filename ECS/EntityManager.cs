using System;
using System.Collections.Generic;

namespace Match3.ECS.Entities
{
    public class EntityManager
    {
        public static EntityManager Instance;

        public event Action<Entity, Component> OnComponentAdded;
        public event Action<Entity, Component> OnComponentRemoved;
        public event Action<Entity> OnEntityAdded;
        public event Action<Entity> OnEntityRemoved;

        private int lastId;
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        public EntityManager()
        {
            Instance = this;
        }

        public void AddEntity(Entity _ent)
        {
            if (entities.ContainsValue(_ent))
            {
                return;
            }

            _ent.entityID = lastId++;

            entities.Add(_ent.entityID, _ent);

            OnEntityAdded?.Invoke(_ent);
        }

        public void RemoveEntity(Entity _ent)
        {
            if (!entities.ContainsValue(_ent))
            {
                return;
            }

            entities.Remove(_ent.entityID);

            OnEntityRemoved?.Invoke(_ent);
        }

        public void ComponentAdded(Entity _ent, Component _comp)
        {
            OnComponentAdded?.Invoke(_ent, _comp);
        }

        public void ComponentRemoved(Entity _ent, Component _comp)
        {
            OnComponentRemoved?.Invoke(_ent, _comp);
        }

        public Entity GetEntity(int _id)
        {
            return entities.ContainsKey(_id) ? entities[_id] : null;
        }
    }
}