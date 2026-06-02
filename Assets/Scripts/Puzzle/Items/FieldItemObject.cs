using UnityEngine;

public class FieldItemObject : InteractableObject
{
    [Header("아이템 데이터")]
    public ItemData itemToGive; // 아이템 에셋

    public override void Interact()
    {
        if (itemToGive == null)
        {
            Debug.LogWarning("ItemData가 등록되지 않았습니다");
            return;
        }

        InventoryManager inv = FindAnyObjectByType<InventoryManager>();

        if (inv != null)
        {
            // 인벤토리에 아이템 집어넣기
            inv.AddItem(itemToGive);

            // 아이템 오브젝트는 파괴(삭제)
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("InventoryManager가 없습니다");
        }
    }
}