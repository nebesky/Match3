using System;
using Microsoft.Xna.Framework;

namespace Match3
{
    public class Animator : Component
    {
        public Vector2 newScale;
        public Vector2 newPosition;
        public readonly float speed;
        public Action OnAnimationEnd;

        public Animator(Vector2 _newPosition, Vector2 _newScale, float _newSpeed = 200f)
        {
            newPosition = _newPosition;
            newScale = _newScale;
            speed = _newSpeed;
        }
    }
}