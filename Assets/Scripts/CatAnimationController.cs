using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    [SerializeField]
    private SpritesetAnimator animator;
    [SerializeField]
    private CatCharacter catCharacter;

    private void Update()
    {
        float xSpeed = catCharacter.Velocity.x;
        if (xSpeed > 0) 
        {
            animator.SpriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (xSpeed < 0)
        {
            animator.SpriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        }

        float speed = catCharacter.Velocity.magnitude;
        if (speed > 0.01f)
        {
            animator.AnimationSpeed = Mathf.Max(SpritesetAnimator.idleAnimationSpeed, catCharacter.MoveSpeed);
            SetAnimation(CatAnimation.Jumping);
        }
        else 
        {
            animator.AnimationSpeed = SpritesetAnimator.idleAnimationSpeed;
            SetAnimation(CatAnimation.Standing);
        }
    }

    private void SetAnimation(CatAnimation animation)
    {
        animator.SetAnimation((int)animation);
    }
}



