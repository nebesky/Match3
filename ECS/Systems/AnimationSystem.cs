using System;
using System.Collections.Generic;
using Match3.ECS.Entities;
using Microsoft.Xna.Framework;

namespace Match3.ECS.Systems
{
    public class AnimationSystem
    {
        private bool startAnimation;
        private Dictionary<Animator, Transform> components = new Dictionary<Animator, Transform>();
        private List<Animator> toRemove = new List<Animator>();
        private const float ScaleOffset = 0.5f;

        public static Action OnAnimationEnd;
        public AnimationSystem()
        {
            EntityManager.Instance.OnComponentAdded += InstanceOnComponentAdded;
            EntityManager.Instance.OnComponentRemoved += InstanceOnComponentRemoved;
            EntityManager.Instance.OnEntityRemoved += InstanceOnEntityRemoved;
        }

        private void InstanceOnEntityRemoved(Entity obj)
        {

        }

        private void InstanceOnComponentRemoved(Entity arg1, Component arg2)
        {
            if (arg2.GetType() == typeof(Animator))
                toRemove.Add((Animator)arg2);
        }

        private void InstanceOnComponentAdded(Entity arg1, Component arg2)
        {
            if (!(arg2 is Animator animator))
                return;

            if (!components.ContainsKey(animator))
                components.Add(animator, arg1.GetComponent<Transform>());
        }

        public void Update(GameTime gameTime)
        {
            var hasMovingElement = false;

            foreach (var (animator, transform) in components)
            {
                if (animator.newPosition == transform.Position && animator.newScale == transform.Scale)
                    continue;

                if (animator.newPosition != transform.Position)
                {
                    startAnimation = true;
                    hasMovingElement = true;

                    var Difference = Vector2.Subtract(transform.Position, animator.newPosition);

                    if (Math.Abs(Difference.X + Difference.Y) > 2f)
                    {
                        var Direction = Vector2.Normalize(Difference);

                        transform.Position -= Direction *
                                              GameParameters.AnimationSpeed *
                                              animator.speed *
                                              (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        animator.newPosition = transform.Position;
                    }
                }

                if (animator.newScale != transform.Scale)
                {
                    startAnimation = true;
                    hasMovingElement = true;

                    var Difference = Vector2.Subtract(transform.Scale, animator.newScale);

                    if (Math.Abs(Difference.X + Difference.Y) > 0.01f)
                    {
                        var Direction = Vector2.Normalize(Difference);

                        transform.Scale -= Direction *
                                           ScaleOffset *
                                           GameParameters.AnimationSpeed *
                                           (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        animator.newScale = transform.Scale;
                    }
                }

                if (animator.newPosition == transform.Position && animator.newScale == transform.Scale)
                {
                    animator.OnAnimationEnd?.Invoke();
                }
            }

            if (!hasMovingElement && startAnimation)
            {
                startAnimation = false;

                OnAnimationEnd?.Invoke();
            }

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