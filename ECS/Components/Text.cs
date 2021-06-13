using Microsoft.Xna.Framework;

namespace Match3
{
    public class Text : Component
    {
        public string text;
        public Vector2 offset;
        public Color textColor;

        public Text(string _text, Vector2 _offset, Color _color)
        {
            text = _text;
            offset = _offset;
            textColor = _color;
        }
    }
}