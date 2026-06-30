using UnityEngine;
using Yarn.Unity;

public class PlayerInteraction : MonoBehaviour
{
    public float checkDistance = 0.35f;
    public LayerMask interactableLayer;

    [Header("Interaction Collider")]
    public BoxCollider2D interactionCollider;
    public Vector2 verticalInteractionSize = new Vector2(0.9f, 0.35f);
    public Vector2 horizontalInteractionSize = new Vector2(0.35f, 1.1f);

    private Vector2 lastDirection = Vector2.down;
    private DialogueRunner runner;

    void Awake()
    {
        EnsureInteractionCollider();
        UpdateInteractionCollider();
    }

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
            lastDirection = GetCardinalDirection(x, y);
            UpdateInteractionCollider();
        }

        if (runner != null && runner.IsDialogueRunning) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckForInteractable();
        }
    }

    void CheckForInteractable()
    {
        EnsureInteractionCollider();
        UpdateInteractionCollider();

        Vector2 center = interactionCollider.bounds.center;
        Vector2 size = interactionCollider.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, interactableLayer);

        InteractableObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            InteractableObject interactable = hit.GetComponentInParent<InteractableObject>();
            if (interactable == null) continue;

            float distance = Vector2.Distance(transform.position, hit.bounds.center);
            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        if (closest != null)
        {
            closest.Interact();
        }
    }

    void EnsureInteractionCollider()
    {
        if (interactionCollider != null) return;

        Transform detector = transform.Find("InteractionDetector");
        if (detector == null)
        {
            GameObject detectorObject = new GameObject("InteractionDetector");
            detectorObject.transform.SetParent(transform);
            detector = detectorObject.transform;
        }

        detector.localRotation = Quaternion.identity;
        detector.localScale = GetInverseParentScale();

        interactionCollider = detector.GetComponent<BoxCollider2D>();
        if (interactionCollider == null)
        {
            interactionCollider = detector.gameObject.AddComponent<BoxCollider2D>();
        }

        interactionCollider.isTrigger = true;
    }

    void UpdateInteractionCollider()
    {
        if (interactionCollider == null) return;

        bool isVertical = lastDirection == Vector2.up || lastDirection == Vector2.down;
        Vector2 size = isVertical ? verticalInteractionSize : horizontalInteractionSize;
        float forwardOffset = checkDistance + (isVertical ? size.y : size.x) * 0.5f;
        Vector2 worldOffset = lastDirection * forwardOffset;
        Vector3 parentScale = transform.lossyScale;

        interactionCollider.transform.localScale = GetInverseParentScale();
        interactionCollider.transform.localPosition = new Vector3(
            parentScale.x != 0f ? worldOffset.x / parentScale.x : worldOffset.x,
            parentScale.y != 0f ? worldOffset.y / parentScale.y : worldOffset.y,
            0f);
        interactionCollider.size = size;
        interactionCollider.offset = Vector2.zero;
    }

    Vector2 GetCardinalDirection(float x, float y)
    {
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            return x > 0 ? Vector2.right : Vector2.left;
        }

        return y > 0 ? Vector2.up : Vector2.down;
    }

    Vector3 GetInverseParentScale()
    {
        Vector3 scale = transform.lossyScale;
        return new Vector3(
            scale.x != 0f ? 1f / scale.x : 1f,
            scale.y != 0f ? 1f / scale.y : 1f,
            scale.z != 0f ? 1f / scale.z : 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (interactionCollider != null)
        {
            UpdateInteractionCollider();
            Gizmos.DrawWireCube(interactionCollider.bounds.center, interactionCollider.bounds.size);
            return;
        }

        bool isVertical = lastDirection == Vector2.up || lastDirection == Vector2.down;
        Vector2 size = isVertical ? verticalInteractionSize : horizontalInteractionSize;
        float forwardOffset = checkDistance + (isVertical ? size.y : size.x) * 0.5f;
        Vector3 center = transform.position + (Vector3)(lastDirection * forwardOffset);
        Gizmos.DrawWireCube(center, size);
    }
}
