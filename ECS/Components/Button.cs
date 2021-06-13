using System;
using MonoGame.Extended.TextureAtlases;

namespace Match3
{
    public class Button : Component
    {
        public Action OnClick;
        private bool isPressed;

        public bool IsPressed
        {
            get => isPressed;
            set
            {
                if (isPressed == value)
                    return;

                target.textureRegion = value ? pressedTextureRegion2D : normalTextureRegion2D;
                isPressed = value;
            }
        }

        public Renderer target;
        public Transform transform;

        public TextureRegion2D normalTextureRegion2D;
        public TextureRegion2D pressedTextureRegion2D;

        public Button(
            Renderer _renderer,
            Transform _transform,
            TextureRegion2D _pressedTextureRegion2D = null)
        {
            target = _renderer;
            transform = _transform;
            normalTextureRegion2D = _renderer.textureRegion;
            pressedTextureRegion2D = _pressedTextureRegion2D ?? normalTextureRegion2D;
            IsPressed = false;
        }
    }
}