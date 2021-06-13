using System.Collections.Generic;
using Match3.ECS.Entities;
using Match3.Enums;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace Match3.Screens
{
    public class GameOverScreen : GameScreen
    {
        public GameOverScreen(Game game) : base(game) { }
        private new Match3Game Game => (Match3Game) base.Game;
        private List<Entity> entities = new List<Entity>();

        public override void LoadContent()
        {
            entities.Add(EntityFactory.GetText(
                "caption",
                Positions.GameOverCaption,
                Localization.GameOverCaption));

            var playButton = EntityFactory.GetButton(
                "play_button",
                Positions.OkButton,
                Localization.OkButton,
                ContentManager.GetTextureRegion(TextureNames.Button),
                ContentManager.GetTextureRegion(TextureNames.ButtonHover));

            playButton.GetComponent<Button>().OnClick += () =>
            {
                Game.LoadScene(GameState.Main);
            };

            entities.Add(playButton);
        }

        public override void Update(GameTime gameTime)
        {
            Game._scriptSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game._renderSystem.Draw(Game._spriteBatch, Game._camera.GetViewMatrix());
        }

        public override void Dispose()
        {
            foreach (var entity in entities)
            {
                entity.Destroy();
            }

            entities.Clear();

            base.Dispose();
        }
    }
}