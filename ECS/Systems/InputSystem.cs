using System.Collections.Generic;
using Match3.ECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Match3.ECS.Systems
{
    public class InputSystem
    {
        public static bool IsActive { get; set; }

        private List<Button> components = new List<Button>();
        private List<Button> toRemove = new List<Button>();

        private ButtonState currentButtonState;

        public InputSystem()
        {
            IsActive = true;

            EntityManager.Instance.OnComponentAdded += InstanceOnComponentAdded;
            EntityManager.Instance.OnComponentRemoved += InstanceOnComponentRemoved;
            EntityManager.Instance.OnEntityRemoved += InstanceOnEntityRemoved;
        }

        private void InstanceOnEntityRemoved(Entity obj)
        {
            var comps = components.FindAll(c => c.entityId == obj.entityID);

            foreach (var item in comps)
            {
                toRemove.Add(item);
            }
        }

        private void InstanceOnComponentRemoved(Entity arg1, Component arg2)
        {
            if (arg2.GetType() == typeof(Button))
                toRemove.Add((Button)arg2);
        }

        private void InstanceOnComponentAdded(Entity arg1, Component arg2)
        {
            if (!(arg2 is Button rend))
                return;

            if (!components.Contains(rend))
                components.Add(rend);
        }

        public void Update(MouseState mouseState, Vector2 worldPosition)
        {
            if (!IsActive) return;

            foreach (var button in components)
            {
                //hover handling
                if (mouseState.LeftButton == ButtonState.Pressed && currentButtonState == ButtonState.Released)
                {
                    button.IsPressed = button.Intersect(worldPosition);
                }

                //pressed handling
                if (mouseState.LeftButton == ButtonState.Released && currentButtonState == ButtonState.Pressed)
                {
                    if (button.IsPressed && button.Intersect(worldPosition))
                    {
                        button.OnClick?.Invoke();
                    }

                    button.IsPressed = false;
                }
            }

            currentButtonState = mouseState.LeftButton;

            HandleRemove();
        }

        private void HandleRemove()
        {
            foreach (var item in toRemove)
            {
                components.Remove(item);
            }

            toRemove.Clear();
        }
    }
}