using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Enums;
using Microsoft.Xna.Framework;

namespace Match3.ECS.Entities
{
    public static class ElementFactory
    {
        private static readonly List<(string element, string hover)> elements = new List<(string, string)>
        {
            (TextureNames.Cat,     TextureNames.CatHover),
            (TextureNames.Owl,     TextureNames.OwlHover),
            (TextureNames.Lizard,  TextureNames.LizardHover),
            (TextureNames.Octopus, TextureNames.OctopusHover),
            (TextureNames.Pig,     TextureNames.PigHover)
        };

        public static Entity GetRandomElement(int x, int y, Vector2 position)
        {
            var elementType = new Random().Next(5);

            return GetElement(x, y, elementType, position);
        }

        private static Entity GetElement(int x, int y, int type, Vector2 position)
        {
            var entityElement = EntityFactory.GetButton(
                "element",
                Vector2.Zero,
                "",
                ContentManager.GetTextureRegion(elements.ElementAt(type).element),
                ContentManager.GetTextureRegion(elements.ElementAt(type).hover));

            var transform = entityElement.GetComponent<Transform>();
            transform.Position = position;
            transform.Scale = Positions.elementScale;

            entityElement.AddComponent(new Element(x, y, type));
            entityElement.GetComponent<Button>().OnClick += entityElement.GetComponent<Element>().OnClick;
            entityElement.AddComponent(new Animator(transform.Position, transform.Scale));

            return entityElement;
        }

        public static Entity GetLineElement(int x, int y, int type, Direction direction)
        {
            var entityElement = EntityFactory.GetButton(
                "bomb",
                Vector2.Zero,
                "",
                ContentManager.GetTextureRegion(elements.ElementAt(type).element),
                ContentManager.GetTextureRegion(elements.ElementAt(type).hover));

            var transform = entityElement.GetComponent<Transform>();
            transform.Scale = Positions.elementScale;

            var element = new Element(x, y, type);
            entityElement.AddComponent(element);

            entityElement.GetComponent<Button>().OnClick += entityElement.GetComponent<Element>().OnClick;
            entityElement.AddComponent(new Animator(transform.Position, transform.Scale));
            entityElement.AddComponent(new LineBonus(element, direction));

            return entityElement;
        }

        public static Entity GetBombElement(int x, int y, int type)
        {
            var entityElement = EntityFactory.GetButton(
                "bomb",
                Vector2.Zero,
                "",
                ContentManager.GetTextureRegion(elements.ElementAt(type).element),
                ContentManager.GetTextureRegion(elements.ElementAt(type).hover));

            var transform = entityElement.GetComponent<Transform>();
            transform.Scale = Positions.elementScale;

            var element = new Element(x, y, type);
            entityElement.AddComponent(element);

            entityElement.GetComponent<Button>().OnClick += entityElement.GetComponent<Element>().OnClick;
            entityElement.AddComponent(new Animator(transform.Position, transform.Scale));
            entityElement.AddComponent(new BombBonus(element));

            return entityElement;
        }

        public static Entity GetExplosion(int x, int y)
        {
            var entityExplosion = new Entity("explosion", "");
            entityExplosion.AddComponent(new Explosion(x, y));

            return entityExplosion;
        }
        public static Entity GetDestroyer(Vector2 position)
        {
            var entityDestroyer = EntityFactory.GetImage(
                "destroyer",
                Vector2.Zero,
                ContentManager.GetTextureRegion(TextureNames.Line));

            entityDestroyer.GetComponent<Renderer>().layerDepth = 0.3f;

            var transform = entityDestroyer.GetComponent<Transform>();
            transform.Position = position;
            transform.Scale = Positions.destroyerScale;

            var animator = new Animator(
                entityDestroyer.GetComponent<Transform>().Position,
                Positions.destroyerScale);

            entityDestroyer.AddComponent(animator);
            entityDestroyer.AddComponent(new Destroyer(animator));

            return entityDestroyer;
        }
    }
}