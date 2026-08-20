using UnityEngine;

public class DirectWarpObject : InteractableObject
{
    [Header("이동할 목적지 정보")]
    public string targetRoomName;       // Room
    public string targetWarpPointName;  // 이동할 목적지 포인트 이름

    [Header("Progress Settings (진도 제어)")]
    public bool changeProgressOnWarp = false;
    public int nextProgressValue = 1;

    [Header("워프 방식 설정")]
    public bool warpOnTrigger = false;

    private WarpManager warpManager;

    void Start()
    {
        warpManager = FindAnyObjectByType<WarpManager>();
    }

    public override void Interact()
    {
        DoWarp();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 워프 옵션 확인, 플레이어인지 확인
        if (warpOnTrigger && collision.CompareTag("Player"))
        {
            DoWarp();
        }
    }

    //warp 기능을 함수로 분리
    private void DoWarp()
    {
        if (warpManager != null)
        {
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