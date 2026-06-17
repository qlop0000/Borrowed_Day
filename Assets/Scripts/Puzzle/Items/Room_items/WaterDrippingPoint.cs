using UnityEngine;
using Yarn.Unity;

public class WaterDrippingPoint : InteractableObject
{
    [Header("아이템 설정")]
    public string bucketItemID = "Bucket"; // 인벤토리 매니저에 등록된 양동이 ID

    [Header("Yarn Spinner 노드 이름")]
    public string nodeWithoutBucket = "Water_NoBucket"; // 양동이 없을 때 대사
    public string nodeWithBucket = "Water_HasBucket";    // 양동이 있을 때 대사(선택지 포함)
    public string nodePBucket = "Done_Bucket";           // 바닥에 둔 양동이와 상호작용 대사

    [Header("퍼즐 트리거 연결")]
    public PuzzleTrigger puzzleTrigger; // 트리거 연결

    private DialogueRunner dialogueRunner;
    public PlayerMovement playerMovement;
    private bool isBucketPlaced = false; // 이미 양동이를 놓았는지 기억하는 스위치

    private void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    public override void Interact()
    {
        if (playerMovement != null) playerMovement.canMove = false;
        // 이미 양동이를 놓은 상태라면 더 이상 상호작용 안 함
        if (isBucketPlaced)
        {
            dialogueRunner.StartDialogue(nodePBucket);
            dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
            return;
        }

        InventoryManager inv = FindAnyObjectByType<InventoryManager>();
        if (inv == null) return;
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);

        // 양동이 검사
        if (inv.HasItem(bucketItemID))
        {
            // 양동이가 있을 때
            dialogueRunner.StartDialogue(nodeWithBucket);
        }
        else
        {
            // 양동이가 없을 때
            dialogueRunner.StartDialogue(nodeWithoutBucket);
        }
    }

    private void OnDialogueComplete()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
        if (playerMovement != null) playerMovement.canMove = true;
        Debug.Log("check");
    }

    // [YarnCommand]를 붙이면 대화 스크립트 안에서 함수를 호출할 수 있음
    [YarnCommand("place_bucket")]
    public void PlaceBucketEffect()
    {
        InventoryManager inv = FindAnyObjectByType<InventoryManager>();
        if (inv != null)
        {
            inv.RemoveItem(bucketItemID); // 가방에서 양동이 삭제
        }

        isBucketPlaced = true; // 상태 변경

        // 퍼즐 트리거 활성화
        if (puzzleTrigger != null)
        {
            puzzleTrigger.SetTriggerComplete(true);
        }

        Debug.Log("DONE");
    }
}