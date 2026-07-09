using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class LockedDoor : InteractableObject
{
    public string requiredItemID = "Key_N1";
    public string openWarpRoomName;
    public string openWarpSpawnPointName;

    [Header("NPC Conversation Gate")]
    public bool requireNpcConversation = true;
    public string requiredNpcConversationVariable = "$is_npc_moved";

    [Header("Locked")]
    public string NodeLock;

    [Header("Unlocked")]
    public string NodeUnLock;

    [Header("Progress Settings")]
    public bool increaseProgressOnWarp = false; // 진도 증가 여부
    public int targetProgressValue = 2;         // 진도 값 변경

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
                CheckAndApplyProgress(); // 진도 변경 체크
                warp.ExecuteWarp(openWarpRoomName, openWarpSpawnPointName);
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
            CheckAndApplyProgress(); // 진도 변경 체크
            warp.ExecuteWarp(openWarpRoomName, openWarpSpawnPointName);
        }

        if (playerMovement != null) playerMovement.canMove = true;
    }

    private void OnLockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnLockDialogueComplete);
        if (playerMovement != null) playerMovement.canMove = true;
    }

    public void UnlockByPuzzle()
    {
        if (isUnlocked) return;
        isUnlocked = true;
    }

    private void CheckAndApplyProgress()
    {
        if (increaseProgressOnWarp && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetProgress(targetProgressValue);
        }
    }

    private bool HasCompletedNpcConversation()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindAnyObjectByType<DialogueRunner>();
            if (dialogueRunner == null) return false;
        }

        return dialogueRunner.VariableStorage.TryGetValue<bool>(requiredNpcConversationVariable, out bool hasCompleted)
            && hasCompleted;
    }
}