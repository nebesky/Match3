using Match3.ECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class CustomRenderer : Component, IRender
    {
        protected CustomRenderer(int _layerDepth = 1)
        {
            layerDepth = _layerDepth;
        }

        public virtual void Draw(SpriteBatch _spriteBatch, Transform transform) { }
        public virtual void Update(GameTime gameTime) { }
        public float layerDepth { get; set; }
    }
}