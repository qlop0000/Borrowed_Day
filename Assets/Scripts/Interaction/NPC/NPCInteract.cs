using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity; // Yarn Spinner 기능 사용
using System.Collections;

public class NPCInteract : InteractableObject
{
    [Header("Yarn 설정")]
    public string talkNode = "Start"; // 이 NPC와 대화할 때 실행할 Yarn 노드 이름

    [Header("참조")]
    public PlayerMovement playerMovement;
    public bool isDialogueActive = false;

    [Header("이동 관련 UI 제어")]
    public UnityEvent onMoveStart; 
    public UnityEvent onMoveEnd; 

    private DialogueRunner dialogueRunner;

    void Start()
    {
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

    
    // 코루틴(IEnumerator)을 반환하면, NPC가 목적지에 도착할 때까지 자동 대기
    [YarnCommand("MoveNPC")]
    public IEnumerator MoveNPC(string waypointName, float speed)
    {
        // 목적지가 될 오브젝트를 추적
        GameObject waypoint = GameObject.Find(waypointName);
        if (waypoint == null)
        {
            Debug.LogError($"[NPC 이동 에러] {waypointName}를 찾을 수 없음");
            yield break; // 없으면 코루틴 종료
        }
        onMoveStart?.Invoke();

        Vector3 targetPos = waypoint.transform.position;

        // 추후 애니메이션 추가
        // Animator anim = GetComponent<Animator>();
        // if(anim != null) anim.SetBool("isWalking", true);

        // 목적지와 가까워질때까지 반복
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }

        // 좌표 동기화
        transform.position = targetPos;
        // if(anim != null) anim.SetBool("isWalking", false);

        Debug.Log("이동 종료");
        onMoveEnd?.Invoke();
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