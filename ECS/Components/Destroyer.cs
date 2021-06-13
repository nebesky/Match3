using System;

namespace Match3
{
    public class Destroyer : CustomComponent
    {
        public Action<int> OnDestroy;

        public Destroyer(Animator animator)
        {
            animator.OnAnimationEnd += () =>
            {
                OnDestroy?.Invoke(entityId);

                GetEntity().Destroy();
            };
        }
    }
}