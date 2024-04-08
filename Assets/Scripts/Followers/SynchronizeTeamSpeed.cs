using UnityEngine;

public class SynchronizeTeamSpeed : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D sourceRigidbody;

    [SerializeField]
    private FollowLeader leader;

    private void Update()
    {
        var speed = sourceRigidbody.velocity.magnitude;
        foreach (var follower in leader.Followers)
            follower.MoveSpeed = speed;
    }
}
