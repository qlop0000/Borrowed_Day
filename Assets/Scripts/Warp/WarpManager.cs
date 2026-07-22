using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class WarpManager : MonoBehaviour
{
    [System.Serializable]
    public struct RoomData
    {
        public string roomName;
        public GameObject roomObject; // 방 오브젝트 묶음

        [Header("조명 설정")]
        public bool isFixedTone;       // true = 고유의 조명, false = 진도에 따라 유동적 변경
        public string defaultToneName; // 고정 조명일 때 사용할 톤 이름 ("Default")
    }

    [Header("맵 리스트")]
    public List<RoomData> roomList = new List<RoomData>();

    [Header("페이드 연출 설정")]
    public CanvasGroup fadeCanvasGroup;   // FadePanel의 CanvasGroup
    public float fadeDuration = 0.5f;     // 어두워지거나 밝아지는 시간 (초)
    public float delayInBlack = 0.2f;     // 머무는 시간 (초)

    private DialogueRunner dialogueRunner;
    private PlayerMovement playerMovement;
    private bool isWarping = false; // 워프 중복 실행 방지

    void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();

        // Yarn Spinner 명령어 (방이름, 워프포인트 이름)
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string, string>("Warp", ExecuteWarp);

            dialogueRunner.AddCommandHandler<int>("SetProgress", (p) => {
                if (ProgressManager.Instance != null) ProgressManager.Instance.SetProgress(p);
            });
        }
    }

    // 외부(LockedDoor, DirectWarpObject 등)에서 호출하는 함수
    public void ExecuteWarp(string targetRoomName, string targetWarpPointName)
    {
        if (isWarping) return; // 워프 중일 때 무시
        StartCoroutine(WarpRoutine(targetRoomName, targetWarpPointName));
    }

    // 코루틴 함수
    private IEnumerator WarpRoutine(string targetRoomName, string targetWarpPointName)
    {
        isWarping = true;

        // 워프 시작 시 플레이어 조작 잠금
        if (playerMovement != null) playerMovement.canMove = false;

        // (Fade In)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(delayInBlack);

        RoomData targetRoomData = new RoomData();
        GameObject targetRoomObject = null;

        foreach (var room in roomList)
        {
            if (room.roomName == targetRoomName)
            {
                room.roomObject.SetActive(true);
                targetRoomObject = room.roomObject; // 위치 검색용 오브젝트 저장
                targetRoomData = room;              // 조명 데이터 저장
            }
            else
            {
                room.roomObject.SetActive(false);
            }
        }

        if (playerMovement != null && targetRoomObject != null)
        {
            Transform warpPoint = targetRoomObject.transform.Find(targetWarpPointName);

            if (warpPoint != null)
            {
                playerMovement.transform.position = warpPoint.position;
            }
            else
            {
                Debug.LogError($"[WarpManager] {targetRoomName} 내부에서 '{targetWarpPointName}' 오브젝트를 찾을 수 없습니다!");
                playerMovement.transform.position = targetRoomObject.transform.position; // 예외 처리
            }
        }

        DetermineAndApplyTone(targetRoomData);
        yield return new WaitForSeconds(delayInBlack);

        // ====== 오브젝트 전환 기능 구현 ======
        if (VisionManager.Instance != null)
        {
            VisionManager.Instance.SetCurrentRoom(targetRoomName);
        }

        if (playerMovement != null && targetRoomObject != null)
        {
            Transform warpPoint = targetRoomObject.transform.Find(targetWarpPointName);

            if (warpPoint != null)
            {
                playerMovement.transform.position = warpPoint.position;
            }
            else
            {
                Debug.LogError($"[WarpManager] {targetRoomName} 내부에서 '{targetWarpPointName}' 오브젝트가 존재하지 않음");
                playerMovement.transform.position = targetRoomObject.transform.position; // 예외 처리
            }
        }

        DetermineAndApplyTone(targetRoomData);
        yield return new WaitForSeconds(delayInBlack);
        // ============

        // (Fade Out)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;

        // 플레이어 조작 허용 및 워프 락 해제
        if (playerMovement != null) playerMovement.canMove = true;
        isWarping = false;
    }

    // 조명 변경 부분
    private void DetermineAndApplyTone(RoomData targetRoomData)
    {
        AtmosphereManager atmosphereManager = FindAnyObjectByType<AtmosphereManager>();

        if (atmosphereManager == null) return;

        // 방이라면 진도에 상관없이 조정
        if (targetRoomData.isFixedTone)
        {
            atmosphereManager.ChangeTone(targetRoomData.defaultToneName, 0f);
        }
        // Outdoor일 때 ProgressManager의 진도에 따라 변경
        else
        {
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.UpdateCurrentAtmosphere(0f);
            }
        }
    }
}