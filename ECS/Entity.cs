using System.Collections.Generic;
using System.Linq;

namespace Match3.ECS.Entities
{
    public class Entity
    {
        public string entityName;
        public string entityTag;
        public int entityID;
        public bool isActive = true;

        private List<Component> _components = new List<Component>();

        public Entity(string _name, string _tag)
        {
            entityName = _name;
            entityTag = _tag;
            isActive = true;

            EntityManager.Instance.AddEntity(this);
        }

        public void AddComponent(Component _component)
        {
            _components.Add(_component);

            _component.entityId = entityID;

            EntityManager.Instance.ComponentAdded(this, _component);
        }

        public T GetComponent<T>() where T: Component
        {
            var comp = _components.FirstOrDefault(c => c.GetType() == typeof(T));

            return (T) comp;
        }

        public List<Component> GetComponents()
        {
            return _components;
        }

        public void RemoveComponent(Component _component)
        {
            _components.Remove(_component);

            EntityManager.Instance.ComponentRemoved(this, _component);
        }

        public void Destroy()
        {
            EntityManager.Instance.RemoveEntity(this);
        }
    }
}