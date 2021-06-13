using Match3.ECS.Entities;

namespace Match3
{
    public class Component
    {
        public int entityId;
        public bool isActive = true;

        public Entity GetEntity()
        {
            return EntityManager.Instance.GetEntity(entityId);
        }
    }
}