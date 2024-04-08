using UnityEngine;

public class MovementTarget : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private GameObject visual;

    private void OnEnable()
    {
        playerMovement.OnStartedTargetting += PlayerMovement_OnStartedTargetting;
        playerMovement.OnTargetReached += PlayerMovement_OnTargetReached;
    }

    private void PlayerMovement_OnTargetReached()
    {
        visual.SetActive(false);
    }

    private void PlayerMovement_OnStartedTargetting()
    {
        visual.SetActive(true);
    }

    private void LateUpdate()
    {
        transform.position = playerMovement.MovementTarget;
    }

    private void OnDisable()
    {
        playerMovement.OnStartedTargetting -= PlayerMovement_OnStartedTargetting;
        playerMovement.OnTargetReached -= PlayerMovement_OnTargetReached;
    }
}
