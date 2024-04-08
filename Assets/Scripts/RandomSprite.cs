using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] sprites;

    private void Start()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
