using UnityEngine;
using Yarn.Unity; // Yarn Spinner 기능 사용

public class NPCInteract : InteractableObject
{
    [Header("Yarn 설정")]
    public string talkNode = "Start"; // 이 NPC와 대화할 때 실행할 Yarn 노드 이름

    [Header("참조")]
    public PlayerMovement playerMovement;
    public bool isDialogueActive = false;

    private DialogueRunner runner;

    void Start()
    {
        // 씬에 있는 DialogueRunner를 자동으로 찾아옵니다.
        runner = FindAnyObjectByType<DialogueRunner>();
        // 중요: 대화가 끝났을 때 EndDialogue 함수를 실행할 수 있도록 설정
        runner.onDialogueComplete.AddListener(EndDialogue);
    }

    public override void Interact()
    {
        if (runner.IsDialogueRunning)
        {
            return;
        }
        if (playerMovement != null)
        {
            playerMovement.canMove = false; // 플레이어 조작 금지
        }

        runner.StartDialogue(talkNode);
    }

    // 대화가 끝나는 시점에 자동 호출
    private void EndDialogue()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = true; // 플레이어 조작 다시 허용
        }

        Debug.Log("대화가 종료되어 플레이어 이동이 다시 활성화되었습니다.");
    }

    // 오브젝트가 파괴될 때 리스너를 제거
    private void OnDestroy()
    {
        if (runner != null)
        {
            runner.onDialogueComplete.RemoveListener(EndDialogue);
        }
    }


}