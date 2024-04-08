using NaughtyAttributes;
using UnityEngine;

public class CatAnimationTest : MonoBehaviour
{
    [SerializeField]
    private SpritesetAnimator animator;
    [SerializeField]
    private CatAnimation catAnimation;
    [SerializeField]
    private int animationLength = 4;
    [SerializeField]
    private bool reversed = false;

    [Button]
    private void Refresh()
    {
        animator.SetAnimation((int)catAnimation, animationLength, reversed);
    }
}
