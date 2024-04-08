using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FollowLeader : MonoBehaviour 
{
    [SerializeField, Tooltip("Saved positions count per unit")]
    private int pathResolution = 10;

    [SerializeField, ReadOnly]
    private List<Vector3> pathBehindLeader = new List<Vector3>();

    public int TeamSize => followers.Count;
    public int MaxPathLength => pathResolution * TeamSize + 1;

    [SerializeField]
    private List<CatFollower> followers = new List<CatFollower>();
    public IReadOnlyList<CatFollower> Followers => followers;    

    private void Start()
    {
        pathBehindLeader.Add(transform.position);
    }

    public void AddFollower(CatFollower followerController)
    {
        followers.Add(followerController);
    }

    public void RemoveController(CatFollower followerController)
    {
        followers.Remove(followerController);
    }
    
    private void Update()
    {
        var currentPosition = transform.position;
        if (Vector2.Distance(currentPosition, pathBehindLeader.Last()) > 1f / pathResolution)
        {
            pathBehindLeader.Add(currentPosition);
            if (pathBehindLeader.Count > MaxPathLength)
            {
                pathBehindLeader.RemoveAt(0);
            }
        }

        for (int i = 0; i < followers.Count; i++)
        {
            var pathIndex = pathBehindLeader.Count - pathResolution * (i + 1);
            if (pathIndex > 0 && pathIndex < pathBehindLeader.Count)
                followers[i].Target = pathBehindLeader[pathIndex];
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        for (int i = 1; i < pathBehindLeader.Count; i++)
        {
            Gizmos.DrawLine(pathBehindLeader[i - 1], pathBehindLeader[i]);
        }
    }
}

