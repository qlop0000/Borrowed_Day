using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보 (현재 사용)")]
    public string itemID;        // 아이템 고유 ID (예: "Key_Red")
    public string itemName;      // 게임에 표시될 이름
    [TextArea]
    public string description;   // 아이템 설명
    public Sprite itemIcon;      // 인벤토리용 이미지

    [Header("추후 확장 가능")]
    public bool isInspectable;   // 조사가 가능 유무
    [TextArea] public string inspectionText; // 조사했을 때 나오는 힌트

    public bool isCombinable;    // 조합이 가능한가?
    public ItemData combinableWith; // 어떤 아이템과 합쳐지는가?
    public ItemData combinedResult; // 합쳐지면 나오는 결과물 아이템
}