public class NPCInteraction : InteractableObject
{
    public string npcName; // NPC 이름
    public DialogueLine[] npcDialogue; // 이 NPC가 할 대사들 (인스펙터에서 설정)
    public bool flipX; // 체크하면 좌우반전, 해제하면 정방향

    public override void Interact()
    {
        // NPC는 상호작용하면 대화를 시작합니다.
        DialogueManager.instance.StartDialogue(npcDialogue);
    }
}