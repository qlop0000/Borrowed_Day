using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class InventoryManager : MonoBehaviour
{
    [Header("인벤토리 데이터")]
    // 아이템 리스트
    public List<ItemData> playerInventory = new List<ItemData>();

    [Header("UI 컴포넌트")]
    public GameObject inventoryPanel;      // 인벤토리 창
    public Transform itemListParent;       // 좌측 아이템 텍스트들이 배치될 부모(Left_ItemList)
    public GameObject itemTextPrefab;      // 복사해서 쓸 텍스트 원본(ItemText_Sample)

    [Header("우측 패널 컴포넌트")]
    public GameObject rightInfoPanel; 
    public Image itemIconImage;            // 우측 아이콘
    public TextMeshProUGUI itemNameText;   // 우측 이름
    public TextMeshProUGUI itemDescText;   // 우측 설명

    public PlayerMovement playerMovement;
    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindAnyObjectByType<Yarn.Unity.DialogueRunner>();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (playerMovement == null) playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    //E키를 누르는 것을 감지
    void Update()
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    // 인벤토리 함수
    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (playerMovement != null) playerMovement.canMove = !isActive;

        // 최신 데이터로 화면 갱신.
        if (isActive)
        {
            UpdateInventoryUI();
        }
    }

    // 인벤토리 화면을 새로고침
    public void UpdateInventoryUI()
    {
        // 리스트의 글자 지우기.
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        // playerInventory에 있는 아이템 개수만큼 글자 생성
        for (int i = 0; i < playerInventory.Count; i++)
        {
            ItemData item = playerInventory[i];

            // 프리팹 생성
            GameObject newSlotObj = Instantiate(itemTextPrefab, itemListParent);

            //  SlotUI 컴포넌트를 가져와 세팅.
            InventorySlotUI slotUI = newSlotObj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(item, this);
            }

            // 현재는 단순 소지형 (이름을 나열하는 정도) 
            // 방향키나 마우스로 아이템을 클릭하면 정보창이 바뀌는 기능 확장 예정
            // 첫 번째 아이템의 정보로 우측 패널 채우기 (수정 예정)
            if (i == 0) ShowItemInfo(item);
        }

        // 가방이 비어있으면 패널 지우기
        if (playerInventory.Count == 0)
        {
            ClearInfoPanel();
        }
    }

    // 정보창에 아이템 정보를 띄워주는 함수
    public void ShowItemInfo(ItemData item)
    {
        if (rightInfoPanel != null) rightInfoPanel.SetActive(true);
        if (itemIconImage != null) itemIconImage.sprite = item.itemIcon;
        if (itemNameText != null) itemNameText.text = item.itemName;
        if (itemDescText != null) itemDescText.text = item.description;
    }

    private void ClearInfoPanel()
    {
        if (rightInfoPanel != null)
        {
            rightInfoPanel.SetActive(false);
            Debug.Log("인벤토리가 비어있음");
        }
    }

    // 아이템 획득 함수 (상자나 바닥에서 아이템 주웠을 때 호출)
    public void AddItem(ItemData newItem)
    {
        playerInventory.Add(newItem);
        Debug.Log($"{newItem.itemName}을(를) 획득");
        
        //UI 갱신 코드 호출 예정
    }

    // **단순 소지형 퍼즐용 체크 함수
    // 퍼즐 오브젝트가 특정 아이템이 있는지 확인
    public bool HasItem(string targetID)
    {
        foreach (var item in playerInventory)
        {
            if (item.itemID == targetID)
            {
                return true; 
            }
        }
        return false;
    }

    // 아이템 사용/소모 함수 (인벤토리에서 삭제)
    public void RemoveItem(string targetID)
    {
        for (int i = 0; i < playerInventory.Count; i++)
        {
            if (playerInventory[i].itemID == targetID)
            {
                playerInventory.RemoveAt(i);
                Debug.Log($"{targetID} 아이템 소모.");
                return;
            }
        }
    }
}