using NaughtyAttributes;
using UnityEngine;

public class CatFollower : CatCharacter
{
    private Vector3 target;
    public Vector3 Target
    {
        get => target; 
        set => target = value;
    }

    [ShowNonSerializedField]
    private Vector3 currentVelocity;
    public override Vector2 Velocity => currentVelocity;

    private void Start()
    {
        target = transform.position;
    }

    private void FixedUpdate()
    {
        float dt = Time.deltaTime;
        var previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(previousPosition, target, MoveSpeed * dt);
        var difference = transform.position - previousPosition;
        currentVelocity = difference / dt; 
    }
}


