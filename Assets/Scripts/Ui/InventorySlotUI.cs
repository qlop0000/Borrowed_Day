using UnityEngine;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    private ItemData currentItem;
    private InventoryManager manager;
    public TextMeshProUGUI itemText; // 슬롯 텍스트 연결

    // 슬롯이 생성될 때 아이템 정보와 매니저를 전달받는 함수
    public void Setup(ItemData item, InventoryManager inventoryManager)
    {
        currentItem = item;
        manager = inventoryManager;

        if (itemText != null)
        {
            itemText.text = item.itemName;
        }
    }

    // 클릭되었을 때 실행될 함수
    public void OnSlotClicked()
    {
        if (manager != null && currentItem != null)
        {
            manager.ShowItemInfo(currentItem);
        }
    }
}