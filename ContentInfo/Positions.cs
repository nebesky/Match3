using Microsoft.Xna.Framework;

namespace Match3
{
    public static class Positions
    {
        //positions
        public static readonly Vector2 FieldPosition = new Vector2(400, 240);
        public static readonly Vector2 TimerPanel = new Vector2(20, 50);
        public static readonly Vector2 TimerText = new Vector2(95, 70);
        public static readonly Vector2 ScorePanel = new Vector2(20, 100);
        public static readonly Vector2 ScoreText = new Vector2(100, 155);
        public static readonly Vector2 GreetingCaption = new Vector2(400, 240);
        public static readonly Vector2 PlayButton = new Vector2(320,270);
        public static readonly Vector2 OkButton = new Vector2(320,270);
        public static readonly Vector2 GameOverCaption = new Vector2(380, 240);

        //scales
        public static readonly Vector2 ScorePanelScale = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 elementScale = new Vector2(0.3f, 0.3f);
        public static readonly Vector2 destroyerScale = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 targetScale = new Vector2(0.9f, 0.9f);
        public static readonly Vector2 targetOffsetScale = new Vector2(12f, 12f);
    }
}