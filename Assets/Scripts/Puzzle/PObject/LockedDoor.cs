using NUnit;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class LockedDoor : InteractableObject
{
    public string requiredItemID = "Key_N1"; // 필요한 열쇠 ID
    public string openWarpRoomName;          // 열렸을 때 이동할 방 이름
    public string targetSpawnPointName;     // 이동할 워프포인트 이름
    [Header("잠긴 상태일 때")]
    public string NodeLock;
    [Header("열릴 때")]
    public string NodeUnLock;


    [Header("행동")]
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
            Debug.Log("열린문");
            if (warp != null)
            {
                Event?.Invoke();
                warp.ExecuteWarp(openWarpRoomName, targetSpawnPointName);
            }
                
            return; 
        }

        if (playerMovement != null) playerMovement.canMove = false;

        if (inv != null && inv.HasItem(requiredItemID))
        {
            Debug.Log("문이 열렸다.");
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
    // 문이 열리는 대화
    private void OnUnlockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnUnlockDialogueComplete);
        StartCoroutine(WarpSequence());

        Debug.Log("문 열림");
    }
    private IEnumerator WarpSequence()
    {
        yield return null;

        WarpManager warp = FindAnyObjectByType<WarpManager>();
        if (warp != null)
        {
            warp.ExecuteWarp(openWarpRoomName, targetSpawnPointName);
        }

        if (playerMovement != null) playerMovement.canMove = true;
    }

    // 잠긴 문 대화
    private void OnLockDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnLockDialogueComplete);

        if (playerMovement != null) playerMovement.canMove = true;
        Debug.Log("잠겨 있는 문");
    }

    public void UnlockByPuzzle()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        Debug.Log($"문이 열림");
    }
}