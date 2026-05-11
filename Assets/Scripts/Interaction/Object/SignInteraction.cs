using UnityEngine;

public class SignInteraction : InteractableObject
{
    public string signText;

    //확장성을 고려한 오브젝트 스크립트 매뉴얼 
    public override void Interact()
    {
        // 표지판 : 간단한 메시지만 띄우거나, 
        // NPC처럼 DialogueManager or 행동을 다르게 정의 가능.
        Debug.Log("표지판을 읽었습니다: " + signText);
    }
}