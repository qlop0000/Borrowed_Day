using UnityEngine;

public class DirectWarpObject : InteractableObject
{
    [Header("이동할 목적지 정보")]
    public string targetRoomName; // 방 이름
    public string targetSpawnPointName; //스폰포인트 이름

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
            warpManager.ExecuteWarp(targetRoomName, targetSpawnPointName);
        }
        else
        {
            Debug.LogError("씬에 WarpManager가 없습니다");
        }
    }
}