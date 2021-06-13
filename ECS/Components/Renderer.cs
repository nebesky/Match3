using Match3.ECS.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace Match3
{
    public class Renderer : Component, IRender
    {
        public TextureRegion2D textureRegion;
        public Color drawColor;

        public Renderer(TextureRegion2D _textureRegion = null, float _layerDepth = 0.1f)
        {
            textureRegion = _textureRegion;
            drawColor = Color.White;
            layerDepth = _layerDepth;
        }

        public float layerDepth { get; set; }
    }
}