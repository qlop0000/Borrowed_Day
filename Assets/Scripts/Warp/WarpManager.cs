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

        // Yarn Spinner에 <<Warp>> 명령어 등록하기
        // 문자열(방이름), 실수(X), 실수(Y)를 인자로 받기
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string, float, float>("Warp", ExecuteWarp);
        }
    }

    // 외부에서 호출하는 함수. 실제 실행은 내부의 코루틴을 실행
    public void ExecuteWarp(string targetRoomName, float x, float y)
    {
        if (isWarping) return; // 워프 중일 때 무시
        StartCoroutine(WarpRoutine(targetRoomName, x, y));
    }

    // 코루틴 함수
    private IEnumerator WarpRoutine(string targetRoomName, float x, float y)
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

        // fade in 상태에서 플레이어 좌표 이동 및 방 교체
        if (playerMovement != null)
        {
            playerMovement.transform.position = new Vector3(x, y, 0);
        }

        foreach (var room in roomList)
        {
            if (room.roomName == targetRoomName) room.roomObject.SetActive(true);
            else room.roomObject.SetActive(false);
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