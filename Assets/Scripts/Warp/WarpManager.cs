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

        // Yarn Spinner에 <<Warp>> 명령어 등록
        // 문자열(방이름), 문자열(스폰포인트이름)을 인자로 받기
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string, string>("Warp", ExecuteWarp);
        }
    }

    // 외부에서 호출하는 함수. 실제 실행은 내부의 코루틴을 실행
    public void ExecuteWarp(string targetRoomName, string spawnPointName)
    {
        if (isWarping) return; // 워프 중일 때 무시
        StartCoroutine(WarpRoutine(targetRoomName, spawnPointName));
    }

    // 코루틴 함수
    private IEnumerator WarpRoutine(string targetRoomName, string spawnPointName)
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

        // fade in 상태에서 잠시 대기 (딜레이 휴식)
        yield return new WaitForSeconds(delayInBlack);

        GameObject targetRoomObject = null;

        foreach (var room in roomList)
        {
            if (room.roomName == targetRoomName)
            {
                room.roomObject.SetActive(true);
                targetRoomObject = room.roomObject; // 타겟 방 기억
            }
            else
            {
                room.roomObject.SetActive(false);
            }
        }

        // 방 안에서 스폰포인트 좌표 찾아서 워프
        if (playerMovement != null && targetRoomObject != null)
        {
            // 타겟 방의 자식 중 spawnPointName와 일치하는 오브젝트 찾기
            Transform spawnPointTransform = targetRoomObject.transform.Find(spawnPointName);

            if (spawnPointTransform != null)
            {
                playerMovement.transform.position = spawnPointTransform.position;
            }
            else
            {
                Debug.LogError($"{targetRoomName} 안에서 '{spawnPointName}' 오브젝트를 찾을 수 없습니다.");
                //못 찾으면 방의 중심점(0,0,0)으로 워프
                playerMovement.transform.position = targetRoomObject.transform.position;
            }
        }

        // 이동 완료 후 잠시 대기 (적응 시간)
        yield return new WaitForSeconds(delayInBlack);

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
}