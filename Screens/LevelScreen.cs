using System.Collections.Generic;
using Match3.ECS.Entities;
using Match3.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;

namespace Match3.Screens
{
    public class LevelScreen : GameScreen
    {
        public LevelScreen(Game game) : base(game)
        {
            milliseconds = 0;
        }

        private new Match3Game Game => (Match3Game) base.Game;
        private List<Entity> entities = new List<Entity>();

        private bool isLevelStarted;
        private int milliseconds;
        private Text timerTextComponent;
        private Text scoreTextComponent;

        private const int TIME_LIMIT = 60000;

        public override void LoadContent()
        {
            var fieldPosition = Positions.FieldPosition;
            var fieldEntity = new Entity("field", "");
            fieldEntity.AddComponent(new Transform(fieldPosition));
            fieldEntity.AddComponent(new DrawFieldComponent(GameParameters.CellSize, GameParameters.CellCount));
            entities.Add(fieldEntity);

            var levelEntity = new Entity("level", "");
            levelEntity.AddComponent(new Transform(fieldPosition));

            var levelScript = new LevelScript(GameParameters.CellCount, GameParameters.CellSize);
            levelScript.OnScoreChange += OnScoreChange;
            levelScript.OnLevelStart += OnLevelStart;
            levelEntity.AddComponent(levelScript);
            entities.Add(levelEntity);

            var timerPanel = EntityFactory.GetImage(
                "scorePanel",Positions.TimerPanel,
                ContentManager.GetTextureRegion(TextureNames.ScoreboardPanel));
            entities.Add(timerPanel);

            var timerText = EntityFactory.GetText(
                "timer",
                Positions.TimerText,
                ""
            );
            timerTextComponent = timerText.GetComponent<Text>();
            entities.Add(timerText);

            var scorePanel = EntityFactory.GetImage(
                "scorePanel",
                Positions.ScorePanel,
                ContentManager.GetTextureRegion(TextureNames.TimerPanel));
            scorePanel.GetComponent<Transform>().Scale = Positions.ScorePanelScale;
            entities.Add(scorePanel);

            var scoreText = EntityFactory.GetText(
                "timer",
                Positions.ScoreText,
                "0"
            );

            scoreTextComponent = scoreText.GetComponent<Text>();
            scoreTextComponent.text = Localization.Starting;

            entities.Add(scoreText);
        }

        private void OnLevelStart()
        {
            isLevelStarted = true;
            scoreTextComponent.text = "0";
        }

        private void OnScoreChange(int newScore)
        {
            scoreTextComponent.text = newScore.ToString();
        }

        public override void Update(GameTime gameTime)
        {
            if (isLevelStarted)
            {
                milliseconds += gameTime.ElapsedGameTime.Milliseconds;
            }

            var scoreText = (TIME_LIMIT - milliseconds) / 1000;
            timerTextComponent.text = scoreText > 0 ? scoreText.ToString() : "0";

            if (milliseconds > TIME_LIMIT)
            {
                Game.LoadScene(GameState.GameOver);
            }

            Game._scriptSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawBackground();

            Game._renderSystem.Draw(Game._spriteBatch, Game._camera.GetViewMatrix());
        }

        private void DrawBackground()
        {
            Game._spriteBatch.Begin(
                SpriteSortMode.Deferred,
                null,
                SamplerState.LinearWrap,
                null,
                null);
            Game._spriteBatch.Draw(
                ContentManager.GetTexture("grass"),
                new Rectangle(
                    0,
                    0,
                    GraphicsDevice.PresentationParameters.Bounds.Width,
                    GraphicsDevice.PresentationParameters.Bounds.Height ),
                new Rectangle(
                    0,
                    0,
                    GraphicsDevice.PresentationParameters.Bounds.Width,
                    GraphicsDevice.PresentationParameters.Bounds.Height),
                Color.White);

            Game._spriteBatch.End();
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