using UnityEngine;
using Yarn.Unity; // Yarn Spinner 기능 사용

public class NPCInteract : InteractableObject
{
    [Header("Yarn 설정")]
    public string talkNode = "Start"; // 이 NPC와 대화할 때 실행할 Yarn 노드 이름

    [Header("참조")]
    public PlayerMovement playerMovement;
    public bool isDialogueActive = false;

    private DialogueRunner dialogueRunner;

    void Start()
    {
        // 씬에 있는 DialogueRunner를 자동으로 찾아옵니다.
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    public override void Interact()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            return;
        }
        if (playerMovement != null)
        {
            playerMovement.canMove = false; // 플레이어 조작 금지
        }
        // NPC가 실제로 '대화를 시작하는 순간'에만 이벤트를 구독
        dialogueRunner.onDialogueComplete.AddListener(EndDialogue);
        dialogueRunner.StartDialogue(talkNode);
    }

    // 대화가 끝나는 시점에 자동 호출
    private void EndDialogue()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = true; // 플레이어 조작 다시 허용
        }
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(EndDialogue);
        }

        Debug.Log("대화가 종료되어 플레이어 이동이 다시 활성화되었습니다.");
    }

    private void OnDestroy()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(EndDialogue);
        }
    }



}