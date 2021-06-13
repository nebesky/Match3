using System.Collections.Generic;
using Match3.ECS.Entities;
using Match3.Enums;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace Match3.Screens
{
    public class MainScreen : GameScreen
    {
        public MainScreen(Game game) : base(game) { }
        private new Match3Game Game => (Match3Game) base.Game;
        private List<Entity> entities = new List<Entity>();

        public override void LoadContent()
        {
            var caption = EntityFactory.GetText(
                "caption",
                Positions.GreetingCaption,
                Localization.GreetingsCaption);

            entities.Add(caption);

            var playButton = EntityFactory.GetButton(
                "play_button",
                Positions.PlayButton,
                Localization.PlayButton,
                ContentManager.GetTextureRegion(TextureNames.Button),
                ContentManager.GetTextureRegion(TextureNames.ButtonHover));

            playButton.GetComponent<Button>().OnClick += () =>
            {
                Game.LoadScene(GameState.Level);
            };

            entities.Add(playButton);
        }

        public override void Update(GameTime gameTime) {}

        public override void Draw(GameTime gameTime)
        {
            Game._renderSystem.Draw(Game._spriteBatch, Game._camera.GetViewMatrix());
        }

        public override void Dispose()
        {
            foreach (var entity in entities)
                entity.Destroy();

            entities.Clear();

            base.Dispose();
        }
    }
}