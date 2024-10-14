using UnityEngine;

public class ArenaConstraint : MonoBehaviour
{
    public float arenaRadius = 10f;
    public Vector3 arenaCenter = Vector3.zero;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 offsetFromCenter = transform.position - arenaCenter;
        offsetFromCenter.y = 0; // Ignore vertical position

        if (offsetFromCenter.magnitude > arenaRadius)
        {
            Vector3 constrainedPosition = arenaCenter + offsetFromCenter.normalized * arenaRadius;
            constrainedPosition.y = transform.position.y; // Maintain original Y position

            if (characterController != null)
            {
                characterController.enabled = false;
                transform.position = constrainedPosition;
                characterController.enabled = true;
            }
            else
            {
                transform.position = constrainedPosition;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(arenaCenter, arenaRadius);
    }
}