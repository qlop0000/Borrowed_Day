using UnityEngine;
using Yarn.Unity;

public class PlayerInteraction : MonoBehaviour
{
    public float checkDistance = 4.0f;
    public float downBonusDistance = 1.5f;
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
        // 방향에 따라 스캔 거리 변경
        float currentDistance = GetCurrentCheckDistance();
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, lastDirection, currentDistance, interactableLayer);

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

    // 방향에 따라 스캔 거리 계산
    private float GetCurrentCheckDistance()
    {
        if (lastDirection.y < 0)
        {
            return checkDistance + downBonusDistance;
        }

        return checkDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float currentDistance = GetCurrentCheckDistance();
        Gizmos.DrawRay(transform.position, lastDirection * currentDistance);
    }
}
