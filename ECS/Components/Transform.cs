using Microsoft.Xna.Framework;

namespace Match3
{
    public class Transform : Component
    {
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;

        public Transform(Vector2 position)
        {
            Position = position;
            Scale = Vector2.One;
            Rotation = 0f;
        }
    }
}