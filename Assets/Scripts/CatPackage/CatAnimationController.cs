using UnityEngine;

namespace CatPackage
{
    public class CatAnimationController : MonoBehaviour
    {
        [SerializeField]
        private SpritesetAnimator animator;
        [SerializeField]
        private CatCharacter catCharacter;

        private void Update()
        {
            var xSpeed = catCharacter.Velocity.x;
            var xRotation = xSpeed > 0 ? 1 : -1;
            animator.SpriteRenderer.transform.localScale = new Vector3(xRotation, 1, 1);

            var speed = catCharacter.Velocity.magnitude;
            if (speed > 0.01f)
            {
                animator.AnimationSpeed = Mathf.Max(SpritesetAnimator.idleAnimationSpeed, catCharacter.MoveSpeed);
                SetAnimation(CatAnimation.Jumping);
                return;
            }
        
            animator.AnimationSpeed = SpritesetAnimator.idleAnimationSpeed;
            SetAnimation(CatAnimation.Standing);
        }

        private void SetAnimation(CatAnimation animation)
        {
            animator.SetAnimation((int)animation);
        }
    }
}



