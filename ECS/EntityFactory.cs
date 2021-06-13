using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace Match3.ECS.Entities
{
    public static class EntityFactory
    {
        private static Entity GetEntity(string name, Vector2? position)
        {
            var newEntity = new Entity(name, "");

            newEntity.AddComponent(new Transform(position ?? Vector2.Zero));
            newEntity.AddComponent(new Renderer());

            return newEntity;
        }

        public static Entity GetImage(string name, Vector2 position, TextureRegion2D textureRegion2D)
        {
            var newEntity = new Entity(name, "");

            newEntity.AddComponent(new Transform(position));
            newEntity.AddComponent(new Renderer(textureRegion2D));

            return newEntity;
        }

        public static Entity GetText(string name, Vector2 position, string text)
        {
            var newEntity = GetEntity(name, position);
            var newTextComponent = new Text(text, Vector2.Zero, Color.DarkGray);

            newEntity.AddComponent(newTextComponent);

            return newEntity;
        }

        public static Entity GetButton(
            string name,
            Vector2 position,
            string text,
            TextureRegion2D normalTexture,
            TextureRegion2D pressedTextureRegion2D)
        {
            var newEntity = GetImage(name, position, normalTexture);

            var buttonComponent = new Button(
                newEntity.GetComponent<Renderer>(),
                newEntity.GetComponent<Transform>(),
                pressedTextureRegion2D
            );

            var textComponent = new Text(text, Vector2.Zero, Color.DarkGray);

            newEntity.AddComponent(buttonComponent);
            newEntity.AddComponent(textComponent);

            return newEntity;
        }
    }
}