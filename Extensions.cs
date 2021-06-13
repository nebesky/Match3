using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Match3
{
    internal static class Extensions
    {
        public static bool Intersect(this Button b, Vector2 position) =>
            b.transform.Position.X < position.X &&
            b.transform.Position.Y < position.Y &&
            b.transform.Position.X + b.target.textureRegion.Width * b.transform.Scale.X > position.X &&
            b.transform.Position.Y + b.target.textureRegion.Height * b.transform.Scale.Y > position.Y;

        public static void DrawLine(
            this SpriteBatch spriteBatch,
            Vector2 start,
            Vector2 end,
            Color color,
            float thickness = 2f)
        {
            var delta = end - start;
            spriteBatch.Draw(
                ContentManager.GetPixel(),
                start,
                null,
                color,
                delta.ToAngle(),
                new Vector2(0, 0.5f),
                new Vector2(delta.Length(), thickness),
                SpriteEffects.None,
                0f);
        }

        private static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }
}