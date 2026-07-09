using UnityEngine;

public class DirectWarpObject : InteractableObject
{
    [Header("이동할 목적지 정보")]
    public string targetRoomName;       // Room
    public string targetWarpPointName;  // 이동할 목적지 포인트 이름

    [Header("Progress Settings (진도 제어)")]
    public bool changeProgressOnWarp = false;
    public int nextProgressValue = 1;

    private WarpManager warpManager;

    void Start()
    {
        warpManager = FindAnyObjectByType<WarpManager>();
    }

    public override void Interact()
    {
        if (warpManager != null)
        {
            // 진도 제어 유무
            if (changeProgressOnWarp && ProgressManager.Instance != null)
            {
                ProgressManager.Instance.SetProgress(nextProgressValue);
            }

            warpManager.ExecuteWarp(targetRoomName, targetWarpPointName);
        }
        else
        {
            Debug.LogError("씬에 WarpManager가 없습니다.");
        }
    }
}