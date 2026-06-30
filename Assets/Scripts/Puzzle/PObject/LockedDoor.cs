using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class LockedDoor : InteractableObject
{
    public string requiredItemID = "Key_N1";
    public string openWarpRoomName;
    public Vector2 openWarpCoordinate;

    [Header("Locked")]
    public string NodeLock;

    [Header("Unlocked")]
    public string NodeUnLock;

    [Header("Exit Requirement")]
    public bool requireDialogueBeforeExit = true;
    public string requiredDialogueNodeBeforeExit = "Start";

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
            if (!CanExit())
            {
                PlayLockedDialogue();
                return;
            }

            Debug.Log("Opened door");
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
            if (!CanExit())
            {
                PlayLockedDialogue();
                return;
            }

            Debug.Log("Door unlocked");
            dialogueRunner.StartDialogue(NodeUnLock);
            isUnlocked = true;
            inv.RemoveItem(requiredItemID);

            dialogueRunner.onDialogueComplete.AddListener(OnUnlockDialogueComplete);
        }
        else
        {
            PlayLockedDialogue();
        }
    }

    private bool CanExit()
    {
        return !requireDialogueBeforeExit || NPCInteract.HasCompletedTalkNode(requiredDialogueNodeBeforeExit);
    }

    private void PlayLockedDialogue()
    {
        if (playerMovement != null) playerMovement.canMove = false;

        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(NodeLock);
            dialogueRunner.onDialogueComplete.AddListener(OnLockDialogueComplete);
            return;
        }

        if (dialogueRunner == null && playerMovement != null)
        {
            playerMovement.canMove = true;
        }
    }

    private void OnUnlockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnUnlockDialogueComplete);
        StartCoroutine(WarpSequence());

        Debug.Log("Door open dialogue complete");
    }

    private IEnumerator WarpSequence()
    {
        yield return null;

        WarpManager warp = FindAnyObjectByType<WarpManager>();
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
        Debug.Log("Locked door");
    }

    public void UnlockByPuzzle()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        Debug.Log("Door unlocked by puzzle");
    }
}
