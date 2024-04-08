using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : CatCharacter
{
    public event System.Action OnStartedTargetting;
    public event System.Action OnTargetReached;

    private Camera viewCamera;

    [ShowNonSerializedField]
    private Vector2 movementTarget;
    public Vector2 MovementTarget => movementTarget;

    private ContactPoint2D[] contacts = new ContactPoint2D[4];

    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();
            return _rigidbody;
        }
    }

    [ShowNonSerializedField]
    private bool isMovingToTarget;
    public bool IsMovingToTarget => isMovingToTarget;

    private bool wasPressedLastFrame;
    private Vector2 direction;

    public override Vector2 Velocity => Rigidbody.velocity;

    private void Awake()
    {
        viewCamera = Camera.main;
    }

    private void Update()
    {
        bool isPressing = Input.GetMouseButton(0) || Input.GetMouseButton(1);
        if (isPressing)
        {
            var screenPosition = Input.mousePosition;
            isMovingToTarget = true;
            movementTarget = viewCamera.ScreenToWorldPoint(screenPosition);
            if (wasPressedLastFrame == false)
                OnStartedTargetting?.Invoke();
        }

        wasPressedLastFrame = isPressing;
    }

    private void FixedUpdate()
    {
        direction = movementTarget - (Vector2)transform.position;
        if (isMovingToTarget && direction.sqrMagnitude < 0.001f)
        {
            isMovingToTarget = false;
            Rigidbody.velocity = Vector2.zero;
            OnTargetReached?.Invoke();
            return;
        }

        if (isMovingToTarget)
        {
            int contactsCount = Rigidbody.GetContacts(contacts);
            for (int i = 0; i < contactsCount; i++)
            {
                var normal = contacts[i].normal;
                var tangent = Vector2.Perpendicular(normal);
                if (Vector2.Dot(direction, normal) < 0)
                    direction = Vector3.Project(direction, tangent);
            }

            if (direction.sqrMagnitude > 1)
                direction.Normalize();

            Rigidbody.velocity = direction * MoveSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(movementTarget, 0.2f);
        Gizmos.DrawLine(transform.position, movementTarget);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction);
    }
}
