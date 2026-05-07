using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance; // 전역으로 접근 가능하게 설정

    public GameObject dialogueGroup;
    public PlayerMovement playerMovement; // 플레이어 이동 스크립트 연결 (canMove 접근)
    public bool isDialogueActive = false;

    void Awake() => instance = this;

    public void StartDialogue()
    {
        isDialogueActive = true;
        dialogueGroup.SetActive(true); // 배경 어둡게 + 대화창 켜기
        playerMovement.canMove = false; // 플레이어 조작 금지
    }

    public void EndDialogue()
    {
        dialogueGroup.SetActive(false); // 대화창 끄기
        playerMovement.canMove = true;  // 플레이어 조작 허용

        StartCoroutine(ResetDialogueState());
    }

    System.Collections.IEnumerator ResetDialogueState()
    {
        yield return null; // 한 프레임 대기
        isDialogueActive = false;
    }

    void Update()
    {
        // 대화 중일 때 Z나 Enter를 누르면 대화 종료 (임시 기능)
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)))
        {
            EndDialogue();
        }
    }
}