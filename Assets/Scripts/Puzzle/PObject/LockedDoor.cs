using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class LockedDoor : InteractableObject
{
    public string requiredItemID = "Key_N1";
    public string openWarpRoomName;
    public Vector2 openWarpCoordinate;

    [Header("NPC Conversation Gate")]
    public bool requireNpcConversation = true;
    public string requiredNpcConversationVariable = "$is_npc_moved";

    [Header("Locked")]
    public string NodeLock;

    [Header("Unlocked")]
    public string NodeUnLock;

    [Header("Action")]
    public UnityEvent Event;

    public bool IsUnlocked => isUnlocked;

    private bool isUnlocked = false;
    private DialogueRunner dialogueRunner;
    public PlayerMovement playerMovement;

    private void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    public override void Interact()
    {
        InventoryManager inv = FindAnyObjectByType<InventoryManager>();
        WarpManager warp = FindAnyObjectByType<WarpManager>();

        if (isUnlocked)
        {
            if (requireNpcConversation && !HasCompletedNpcConversation())
            {
                if (playerMovement != null) playerMovement.canMove = true;
                return;
            }

            Debug.Log("Unlocked door");
            if (warp != null)
            {
                Event?.Invoke();
                warp.ExecuteWarp(openWarpRoomName, openWarpCoordinate.x, openWarpCoordinate.y);
            }

            return;
        }

        if (playerMovement != null) playerMovement.canMove = false;

        if (inv != null && inv.HasItem(requiredItemID))
        {
            Debug.Log("Door unlocked");
            dialogueRunner.StartDialogue(NodeUnLock);
            isUnlocked = true;
            inv.RemoveItem(requiredItemID);

            dialogueRunner.onDialogueComplete.AddListener(OnUnlockDialogueComplete);
        }
        else
        {
            dialogueRunner.StartDialogue(NodeLock);
            dialogueRunner.onDialogueComplete.AddListener(OnLockDialogueComplete);
        }
    }

    private void OnUnlockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnUnlockDialogueComplete);
        StartCoroutine(WarpSequence());

        Debug.Log("Door opened");
    }

    private IEnumerator WarpSequence()
    {
        yield return null;

        WarpManager warp = FindAnyObjectByType<WarpManager>();
        if (requireNpcConversation && !HasCompletedNpcConversation())
        {
            if (playerMovement != null) playerMovement.canMove = true;
            yield break;
        }

        if (warp != null)
        {
            warp.ExecuteWarp(openWarpRoomName, openWarpCoordinate.x, openWarpCoordinate.y);
        }

        if (playerMovement != null) playerMovement.canMove = true;
    }

    private void OnLockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnLockDialogueComplete);

        if (playerMovement != null) playerMovement.canMove = true;
        Debug.Log("Door is locked");
    }

    public void UnlockByPuzzle()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        Debug.Log("Door unlocked by event");
    }

    private bool HasCompletedNpcConversation()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindAnyObjectByType<DialogueRunner>();
            if (dialogueRunner == null)
            {
                return false;
            }
        }

        return dialogueRunner.VariableStorage.TryGetValue<bool>(requiredNpcConversationVariable, out bool hasCompleted)
            && hasCompleted;
    }
}
