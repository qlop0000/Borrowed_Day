using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance { get; private set; }

    [System.Serializable]
    public struct VisionRoomData
    {
        public string roomName;
        public GameObject normalObjects;
        public GameObject reaperObjects;
    }

    [Header("방별 비전 데이터 등록")]
    public List<VisionRoomData> visionRoomList = new List<VisionRoomData>();

    [Header("입력 키 설정")]
    public KeyCode visionToggleKey = KeyCode.Q;

    [Header("시각 연출 (Dimmer)")]
    public Image screenDimmer;
    [Range(0f, 1f)] public float dimAlpha = 0.4f;
    public float fadeDuration = 0.2f;

    private string currentRoomName = "";
    private bool isReaperVisionOn = false;
    private bool canToggleVision = true;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();

        if (screenDimmer != null)
        {
            Color c = screenDimmer.color;
            c.a = 0f;
            screenDimmer.color = c;
            screenDimmer.gameObject.SetActive(false);
        }

        InitializeVisionStates();

        if (visionRoomList != null && visionRoomList.Count > 0)
        {
            currentRoomName = visionRoomList[0].roomName;
        }

        InitializeVisionStates();
    }

    private void Update()
    {
        if (playerMovement != null && !playerMovement.canMove) return;

        if (Input.GetKeyDown(visionToggleKey) && canToggleVision)
        {
            ToggleVision();
        }
    }

    public void SetCurrentRoom(string roomName)
    {
        currentRoomName = roomName;

        if (isReaperVisionOn)
        {
            isReaperVisionOn = false;
            if (screenDimmer != null) screenDimmer.gameObject.SetActive(false);
        }

        ApplyVisionState();
    }

    private void ToggleVision()
    {
        isReaperVisionOn = !isReaperVisionOn;

        StopAllCoroutines();
        StartCoroutine(FadeDimmerRoutine(isReaperVisionOn));

        ApplyVisionState();
    }

    private void ApplyVisionState()
    {
        foreach (var room in visionRoomList)
        {
            if (room.roomName == currentRoomName)
            {
                if (room.normalObjects != null) room.normalObjects.SetActive(!isReaperVisionOn);
                if (room.reaperObjects != null) room.reaperObjects.SetActive(isReaperVisionOn);
            }
            else
            {
                if (room.normalObjects != null) room.normalObjects.SetActive(false);
                if (room.reaperObjects != null) room.reaperObjects.SetActive(false);
            }
        }
    }

    private void InitializeVisionStates()
    {
        foreach (var room in visionRoomList)
        {
            if (room.normalObjects != null) room.normalObjects.SetActive(true);
            if (room.reaperObjects != null) room.reaperObjects.SetActive(false);
        }
    }

    private IEnumerator FadeDimmerRoutine(bool turnOn)
    {
        if (screenDimmer == null) yield break;

        if (turnOn) screenDimmer.gameObject.SetActive(true);

        float targetAlpha = turnOn ? dimAlpha : 0f;
        Color color = screenDimmer.color;
        float startAlpha = color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            screenDimmer.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        screenDimmer.color = color;

        if (!turnOn) screenDimmer.gameObject.SetActive(false);
    }
}