using UnityEngine;

public class DirectWarpObject : InteractableObject
{
    [Header("이동할 목적지 정보")]
    public string targetRoomName; // 예: "room"
    public Vector2 targetCoordinate; // 이동할 X, Y 좌표

    private WarpManager warpManager;

    void Start()
    {
        // WarpManage
        warpManager = FindAnyObjectByType<WarpManager>();
    }

    public override void Interact()
    {
        if (warpManager != null)
        {
            // 매니저의 워프 기능 실행
            warpManager.ExecuteWarp(targetRoomName, targetCoordinate.x, targetCoordinate.y);
        }
        else
        {
            Debug.LogError("씬에 WarpManager가 없습니다! 배치했는지 확인하세요.");
        }
    }
}