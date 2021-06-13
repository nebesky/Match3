using System;
using Match3.ECS.Entities;
using Match3.ECS.Systems;
using Match3.Enums;
using Match3.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;

namespace Match3
{
    public class Match3Game : Game
    {
        private readonly ScreenManager _screenManager;
        public SpriteBatch _spriteBatch;
        public OrthographicCamera _camera;

        public RenderSystem _renderSystem;
        public ScriptSystem _scriptSystem;
        private InputSystem _inputSystem;
        private AnimationSystem _animationSystem;

        public Match3Game()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new EntityManager();
            // ReSharper disable once ObjectCreationAsStatement
            new GraphicsDeviceManager(this);

            _screenManager = new ScreenManager();
            _renderSystem = new RenderSystem();
            _inputSystem = new InputSystem();
            _scriptSystem = new ScriptSystem();
            _animationSystem = new AnimationSystem();

            Components.Add(_screenManager);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);

            LoadScene(GameState.Main);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ContentManager.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));

            _inputSystem.Update(mouseState, worldPosition);
            _animationSystem.Update(gameTime);
            _renderSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        public void LoadScene(GameState gameScreen)
        {
            switch (gameScreen)
            {
                case GameState.Level:
                    _screenManager.LoadScreen(new LevelScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
                    break;
                case GameState.Main:
                    _screenManager.LoadScreen(new MainScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
                    break;
                case GameState.GameOver:
                    _screenManager.LoadScreen(new GameOverScreen(this), new FadeTransition(GraphicsDevice, Color.Black));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameScreen), gameScreen, null);
            }
        }
    }
}