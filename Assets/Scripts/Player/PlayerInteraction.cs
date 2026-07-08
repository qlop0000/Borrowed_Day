using UnityEngine;
using Yarn.Unity;

public class PlayerInteraction : MonoBehaviour
{
    public float checkDistance = 4.0f;
    public LayerMask interactableLayer;

    private Vector2 lastDirection = Vector2.down;
    private DialogueRunner runner;

    void Start()
    {
        runner = FindAnyObjectByType<DialogueRunner>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            lastDirection = new Vector2(x, y).normalized;
        }

        if (runner != null && runner.IsDialogueRunning) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckForInteractable();
        }
    }

    void CheckForInteractable()
    {
        // Skip decorative colliders and interact with the first real target.
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, lastDirection, checkDistance, interactableLayer);

        foreach (RaycastHit2D hit in hits)
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                interactable.Interact();
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastDirection * checkDistance);
    }
}
