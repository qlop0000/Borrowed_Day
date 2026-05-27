using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;

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

    private DialogueRunner dialogueRunner;
    private PlayerMovement playerMovement;

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

    // 워프 실행 함수
    public void ExecuteWarp(string targetRoomName, float x, float y)
    {
        // 플레이어 위치 이동
        if (playerMovement != null)
        {
            playerMovement.transform.position = new Vector3(x, y, 0);
        }

        // 방 on/off
        foreach (var room in roomList)
        {
            if (room.roomName == targetRoomName)
            {
                room.roomObject.SetActive(true);  // 목적지 방 켜기
                Debug.Log($"{targetRoomName} 활성화");
            }
            else
            {
                room.roomObject.SetActive(false); // 나머지 방 끄기
            }
        }
    }
}