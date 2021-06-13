using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.ECS.Entities
{
    public class RenderSystem
    {
        private Dictionary<Component, Transform> spriteComponents = new Dictionary<Component, Transform>();
        private Dictionary<Renderer, Text> textComponents = new Dictionary<Renderer, Text>();

        private List<Component> toRemove = new List<Component>();

        public RenderSystem()
        {
            EntityManager.Instance.OnComponentAdded += InstanceOnComponentAdded;
            EntityManager.Instance.OnComponentRemoved += InstanceOnComponentRemoved;
            EntityManager.Instance.OnEntityRemoved += InstanceOnEntityRemoved;
        }

        private void InstanceOnEntityRemoved(Entity obj)
        {
            var renderComponent = spriteComponents.FirstOrDefault(c => c.Key.entityId == obj.entityID);
            var textComponent = textComponents.FirstOrDefault(c => c.Key.entityId == obj.entityID);

            if(renderComponent.Key != null)
            {
                toRemove.Add(renderComponent.Key);
            }

            if (textComponent.Key != null && !toRemove.Contains(textComponent.Key))
            {
                toRemove.Add(textComponent.Key);
            }
        }

        private void InstanceOnComponentRemoved(Entity arg1, Component arg2)
        {

        }

        private void InstanceOnComponentAdded(Entity arg1, Component arg2)
        {
            if (arg2 is Renderer || arg2 is CustomRenderer)
            {
                if (!spriteComponents.ContainsKey(arg2))
                {
                    spriteComponents.Add(arg2, arg1.GetComponent<Transform>());
                }
            }

            if (arg2 is Text text)
            {
                if (textComponents.ContainsValue(text))
                    return;

                var render = arg1.GetComponent<Renderer>() ?? throw new Exception("Text need Render component");

                if (!textComponents.ContainsKey(render))
                {
                    textComponents.Add(render, text);
                }
            }
        }

        private void HandleRemove()
        {
            foreach (var component in toRemove)
            {
                if (spriteComponents.ContainsKey(component))
                {
                    spriteComponents.Remove(component);
                }

                if (component.GetType() == typeof(Renderer) && textComponents.ContainsKey((Renderer)component))
                    textComponents.Remove((Renderer)component);
            }

            toRemove.Clear();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in spriteComponents)
            {
                if (item.Key.GetType() != typeof(CustomRenderer))
                    continue;

                var renderer =(CustomRenderer)item.Key;
                renderer.Update(gameTime);
            }

            HandleRemove();
        }

        public void Draw(SpriteBatch _spriteBatch, Matrix transformMatrix)
        {
            _spriteBatch.Begin(transformMatrix: transformMatrix, sortMode: SpriteSortMode.FrontToBack);

            foreach (var (component, transform) in spriteComponents)
            {
                if (component.GetEntity() == null || !component.GetEntity().isActive)
                    continue;

                if (component.GetType() == typeof(Renderer))
                {
                    var render = (Renderer) component;
                    if (render.textureRegion != null)
                    {
                        _spriteBatch.Draw(
                            render.textureRegion.Texture,
                            new Rectangle(
                                (int) transform.Position.X,
                                (int) transform.Position.Y,
                                (int) (render.textureRegion.Width * transform.Scale.X),
                                (int) (render.textureRegion.Height * transform.Scale.Y)
                            ),
                            new Rectangle(
                                render.textureRegion.X,
                                render.textureRegion.Y,
                                render.textureRegion.Width,
                                render.textureRegion.Height
                            ),
                            render.drawColor,
                            0.0f, Vector2.Zero, SpriteEffects.None, render.layerDepth);
                    }

                    if (textComponents.ContainsKey(render) && textComponents[render].text != "")
                    {
                        var textPosition = transform.Position
                           + textComponents[render].offset
                           - ContentManager.font.MeasureString(textComponents[render].text) / 2;

                        if (render.textureRegion != null)
                            textPosition += new Vector2(render.textureRegion.Width / 2, render.textureRegion.Height / 2);

                        _spriteBatch.DrawString(
                            ContentManager.font,
                            textComponents[render].text,
                            textPosition,
                            textComponents[render].textColor,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            0.4f);
                    }
                } else if (component is CustomRenderer customRenderer)
                {
                    customRenderer.Draw(_spriteBatch, transform);
                }
            }

            _spriteBatch.End();

            HandleRemove();
        }
    }
}