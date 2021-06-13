using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Match3
{
    public class BombBonus : CustomRenderer
    {
        private readonly TextureRegion2D bombTextureArea;

        public Action<Element> OnActivate;

        public BombBonus(Element _element)
        {
            layerDepth = 0.2f;

            bombTextureArea = ContentManager.GetTextureRegion(TextureNames.Bomb);

            _element.OnMarkRemove += delegate(bool active)
            {
                if (active) OnActivate?.Invoke(_element);
            };
        }

        public override void Draw(SpriteBatch _spriteBatch, Transform transform)
        {
            _spriteBatch.Draw(
                bombTextureArea.Texture,
                new Rectangle(
                    (int) transform.Position.X + 11,
                    (int) transform.Position.Y + 7,
                    (int) (bombTextureArea.Width * transform.Scale.X * 0.5f),
                    (int) (bombTextureArea.Height * transform.Scale.Y * 0.5f)
                ),
                new Rectangle(
                    bombTextureArea.X,
                    bombTextureArea.Y,
                    bombTextureArea.Width,
                    bombTextureArea.Height
                ),
                Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }
    }
}