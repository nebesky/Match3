using System.Collections.Generic;
using System.IO;
using System.Linq;
using Match3.structs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace Match3
{
    public static class ContentManager
    {
        private static Texture2D pixel;
        public static SpriteFont font;
        private static List<SpriteInfo> _spriteInfos;
        private static Dictionary<string, Texture2D> sprites = new Dictionary<string, Texture2D>();

        private static readonly Dictionary<string, string> spritePaths = new Dictionary<string, string>
        {
            {"data","SpriteData"},
            {"grass","an-jihun-grass-b"}
        };

        public static void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            foreach (var (key, value) in spritePaths)
                sprites.Add(key, content.Load<Texture2D>(value));

            pixel = new Texture2D(sprites.First().Value.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { new Color(255,255,255,1) });

            font = content.Load<SpriteFont>("font");

            var filePath = Path.Combine(content.RootDirectory, "SpriteData.json");

            using var stream = TitleContainer.OpenStream(filePath);
            using var reader = new StreamReader(stream);
            _spriteInfos = JsonConvert.DeserializeObject<List<SpriteInfo>>(reader.ReadToEnd());
        }

        public static Texture2D GetPixel()
        {
            return pixel;
        }

        public static Texture2D GetTexture(string key)
        {
            return sprites[key];
        }

        public static TextureRegion2D GetTextureRegion(string key)
        {
            var spriteInfo = _spriteInfos.First(s => s.filename == key);

            return new TextureRegion2D(
                GetTexture("data"),
                spriteInfo.frame.x,
                spriteInfo.frame.y,
                spriteInfo.frame.w,
                spriteInfo.frame.h
            );
        }
    }
}