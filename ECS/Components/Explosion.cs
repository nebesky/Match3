using System;
using Microsoft.Xna.Framework;

namespace Match3
{
    public class Explosion : CustomComponent
    {
        public Explosion(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Action<int, int> OnExplode;

        private readonly int x;
        private readonly int y;
        private int milliseconds;

        private const int DELAY = 250;

        public override void Update(GameTime gameTime)
        {
            milliseconds += gameTime.ElapsedGameTime.Milliseconds;

            if (milliseconds > DELAY)
            {
                GetEntity().Destroy();
                OnExplode?.Invoke(x, y);
            }
        }
    }
}