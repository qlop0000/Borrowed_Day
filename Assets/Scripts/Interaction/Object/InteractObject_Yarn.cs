using UnityEngine;
using Yarn.Unity;
using static Unity.Collections.Unicode;

public class InteractObject_Yarn : InteractableObject
{
    [Header("실행할 Yarn 노드 이름")]
    public string targetNodeName;

    [Header("참조")]
    public PlayerMovement playerMovement;

    private DialogueRunner dialogueRunner;

    void Start()
    {
        // 씬에서 DialogueRunner를 찾기
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();

        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    public override void Interact()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = false; // 플레이어 조작 금지
        }
        // 대화 매니저가 존재하고, 현재 아무 대화도 진행 중이 아닐 때만 실행
        if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
        {
            if (!string.IsNullOrEmpty(targetNodeName))
            {
                // 이 객체가 대화를 시작하는 바로 이 순간에만 이벤트를 구독
                dialogueRunner.onDialogueComplete.AddListener(EndInteractObject);

                // 지정한 Yarn 대본의 타이틀(노드)을 실행
                dialogueRunner.StartDialogue(targetNodeName);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} 오브젝트에 연결된 Yarn 노드 이름이 없습니다");
            }
        }
    }
    private void EndInteractObject()
    {
        if (playerMovement != null)
        {
            playerMovement.canMove = true; // 플레이어 조작 다시 허용
        }
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(EndInteractObject);
        }

        Debug.Log("오브젝트 상호작용 종료.");
    }
}
