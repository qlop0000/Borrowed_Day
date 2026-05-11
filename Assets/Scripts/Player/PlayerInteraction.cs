using System.Xml.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float checkDistance = 0.5f; // 감지 거리
    public LayerMask interactableLayer; // NPC나 사물이 속한 레이어 체크
    public GameObject dialoguePanel;   // 연결할 UI 패널

    private Vector2 lastDirection = Vector2.down; // 마지막으로 바라본 방향

    void Update()
    {
        // 플레이어의 이동 방향 기억 (이동 스크립트의 입력을 활용)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            lastDirection = new Vector2(x, y).normalized;
        }

        if (DialogueManager.instance.isDialogueActive) return;

        //'Z' 키를 눌렀을 때 레이캐스트
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckForInteractable();
        }
    }

    void CheckForInteractable()
    {
        // 플레이어 위치에서 바라보는 방향으로 선을 발사
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lastDirection, checkDistance, interactableLayer);

        if (hit.collider != null)
        {
            // 맞은 대상에게 NPC1DATA가 있는지 확인
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                interactable.Interact(); // 그 사물에 맞는 행동 실행
            }
        }
    }

    // 레이 체크를 위한 시각화 코드
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastDirection * checkDistance);
    }
}