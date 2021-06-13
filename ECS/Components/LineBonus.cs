using System;
using Match3.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Match3
{
    public class LineBonus : CustomRenderer
    {
        public Action<Element, Direction> OnActivate;

        private Element _element;
        private readonly TextureRegion2D lineTextureArea;
        private Direction _direction;

        public LineBonus(Element element, Direction direction)
        {
            layerDepth = 0.2f;

            _direction = direction;
            lineTextureArea = ContentManager.GetTextureRegion(TextureNames.Line);

            _element = element;
            _element.OnMarkRemove += delegate(bool active)
            {
                if (active) OnActivate?.Invoke(_element, _direction);
            };
        }

        public override void Draw(SpriteBatch _spriteBatch, Transform transform)
        {
            _spriteBatch.Draw(
                lineTextureArea.Texture,
                new Rectangle(
                    (int) transform.Position.X + (_direction == Direction.vertical ? 40 : 0),
                    (int) transform.Position.Y,
                    (int) (lineTextureArea.Width * transform.Scale.X),
                    (int) (lineTextureArea.Height * transform.Scale.Y)
                ),
                new Rectangle(
                    lineTextureArea.X,
                    lineTextureArea.Y,
                    lineTextureArea.Width,
                    lineTextureArea.Height
                ),
                Color.White,
                _direction == Direction.vertical ? 90.0f : 0.0f,
                Vector2.Zero,
                SpriteEffects.None,
                layerDepth);
        }
    }
}