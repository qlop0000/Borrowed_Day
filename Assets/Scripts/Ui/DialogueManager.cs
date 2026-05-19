using TMPro;         // TextMeshPro 사용을 위해 필요
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 사용을 위해 필요

[System.Serializable]
public class DialogueLine
{
    public string name;      // 말하는 주체 이름
    [TextArea(3, 10)]
    public string context;   // 대사 내용
    public Sprite portrait;  // 일러스트
    public bool isFlip;      // 반전 여부
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI 연결")]
    public GameObject dialogueGroup;
    public TextMeshProUGUI nameText;     // 추가: 이름 표시용
    public TextMeshProUGUI dialogueText; // 추가: 대사 표시용
    public Image portraitImage;          // 추가: 초상화 표시용

    [Header("대화 데이터")]
    public DialogueLine[] lines;         // 추가: 대사 목록 주머니
    private int currentIndex = 0;        // 추가: 현재 몇 번째 대사인지 기록

    [Header("참조")]
    public PlayerMovement playerMovement;
    public bool isDialogueActive = false;

    void Awake() => instance = this;

    public void StartDialogue(DialogueLine[] newLines)
    {
        lines = newLines;      // 전달받은 대사들로 교체
        currentIndex = 0;      // 처음부터 시작

        isDialogueActive = true;
        dialogueGroup.SetActive(true); // 배경 어둡게 + 대화창 켜기
        playerMovement.canMove = false; // 플레이어 조작 금지

        DisplayNextLine();
    }

    // 다음 대사를 화면에 출력하는 기능
    public void DisplayNextLine()
    {
        // 더 이상 보여줄 대사가 없다면 종료
        if (currentIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines[currentIndex];

        // 1. 이름과 대사 텍스트 업데이트
        nameText.text = currentLine.name;
        dialogueText.text = currentLine.context;

        // 2. 초상화 처리 (데이터가 있을 때만 활성화)
        if (currentLine.portrait != null)
        {
            portraitImage.sprite = currentLine.portrait;
            portraitImage.gameObject.SetActive(true);

            if (currentLine.isFlip)
            {
                portraitImage.rectTransform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                portraitImage.rectTransform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            portraitImage.gameObject.SetActive(false); // 초상화 없으면 숨김
        }

        currentIndex++; // 다음 줄 번호 증가
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
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return)))
        {
            DisplayNextLine(); // EndDialogue 대신 다음 대사 출력 함수 호출
        }
    }
}