using UnityEngine;

public abstract class CatCharacter : MonoBehaviour 
{
    public abstract Vector2 Velocity { get; }

    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
}
