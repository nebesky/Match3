using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public class DrawFieldComponent : CustomRenderer
    {
        private int filedSize;
        private int cellCnt;

        public DrawFieldComponent(int _filedSize, int _cellCnt)
        {
            filedSize = _filedSize;
            cellCnt = _cellCnt;
        }

        public override void Draw(SpriteBatch _spriteBatch, Transform transform)
        {
            var (x, y) = transform.Position - new Vector2(filedSize * cellCnt / 2, filedSize * cellCnt / 2);

            for (var i = 0; i <= cellCnt; i++)
            {
                _spriteBatch.DrawLine(
                    new Vector2(filedSize * i + x, 0 + y),
                    new Vector2(
                        filedSize * i + x,
                        filedSize * cellCnt + y),
                    Color.White);

                _spriteBatch.DrawLine(
                    new Vector2( 0 + x, filedSize * i + y),
                    new Vector2(filedSize * cellCnt + x,
                        filedSize * i + y),
                    Color.White);
            }
        }
    }
}