using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueUIController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSpriteData
    {
        public string characterName; // Yarn에서 부를 이름 (예: Enfer)
        public Sprite sprite;        // 연결할 이미지
    }

    [Header("캐릭터 스프라이트 데이터베이스")]
    // 인스펙터에서 캐릭터 이름과 스프라이트를 미리 등록해두는 리스트
    public List<CharacterSpriteData> spriteDatabase = new List<CharacterSpriteData>();

    [Header("UI 연결 (하이어라키)")]
    public Image leftIllustration;
    public Image rightIllustration;

    [Header("자동 숨김 설정 (선택지 감지)")]
    public Transform optionsContainer;     // 선택지 버튼들이 생성되는 부모 (OptionsContainer)
    public GameObject illustrationManager; // 일러스트 전체를 묶은 부모 (IllustrationManager)

    private DialogueRunner runner;
    private Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    [Header("색상 설정")]
    // 인스펙터에서 명암을 직접 조절 가능
    public Color normalColor = Color.white; // 기본 밝기 (하얀색 = 원본 색상)
    public Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1f); // 어두워졌을 때의 색상 (진한 회색)

    void Start()
    {
        runner = FindAnyObjectByType<DialogueRunner>();

        foreach (var data in spriteDatabase)
        {
            if (!spriteDict.ContainsKey(data.characterName))
                spriteDict.Add(data.characterName, data.sprite);
        }

        // Yarn Spinner 명령어 등록
        runner.AddCommandHandler<string, string>("ShowSprite", ShowSprite);
        runner.AddCommandHandler<string>("Focus", FocusCharacter);

        // 대본에서 수동으로 일러스트를 모두 지우고 싶을 때 쓸 명령어
        runner.AddCommandHandler("ClearSprites", ClearAllIllustrations);
        runner.onDialogueComplete.AddListener(ClearAllIllustrations);
    }

    void Update()
    {
        // 선택지 컨테이너와 일러스트 매니저가 잘 연결되어 있는지 확인
        if (optionsContainer != null && illustrationManager != null)
        {
            bool isOptionActive = false; // 선택지가 떠있는지 확인하는 스위치

            // OptionsContainer의 자식들을 검사해서 켜진 버튼이 있는지 확인.
            foreach (Transform child in optionsContainer)
            {
                // 조건 1. 자식 오브젝트가 현재 켜져 있는가? (활성화 상태)
                // 조건 2. 이름에 "Option" 이라는 단어가 포함되어 있는가? (Background 등 무시)
                if (child.gameObject.activeSelf && child.name.Contains("Option"))
                {
                    isOptionActive = true;
                    break;
                }
            }

            // 켜진 선택지 버튼이 있다면 일러스트를 숨김
            if (isOptionActive)
            {
                illustrationManager.SetActive(false);
            }
            // 켜진 선택지가 없고, 대화가 진행 중일 때만 일러스트를 다시 켬
            else if (runner != null && runner.IsDialogueRunning)
            {
                illustrationManager.SetActive(true);
            }
        }
    }


    // 일러스트 띄우는 명령어 (예: <<ShowSprite Left Enfer>>)
    public void ShowSprite(string position, string characterName)
    {
        if (!spriteDict.ContainsKey(characterName))
        {
            Debug.LogError($"{characterName} 일러스트를 찾을 수 없습니다.");
            return;
        }

        Sprite targetSprite = spriteDict[characterName];

        if (position.ToLower() == "left")
        {
            leftIllustration.sprite = targetSprite;
            leftIllustration.gameObject.SetActive(true);
            leftIllustration.color = normalColor;
        }
        else if (position.ToLower() == "right")
        {
            rightIllustration.sprite = targetSprite;
            rightIllustration.gameObject.SetActive(true);
            rightIllustration.color = normalColor;
        }
    }

    // 말하는 주체 강조 명령어 (예: <<Focus Left>>)
    public void FocusCharacter(string position)
    {
        if (position.ToLower() == "left")
        {
            leftIllustration.color = normalColor;  // 왼쪽 밝게
            rightIllustration.color = dimmedColor; // 오른쪽 어둡게
        }
        else if (position.ToLower() == "right")
        {
            leftIllustration.color = dimmedColor;  // 왼쪽 어둡게
            rightIllustration.color = normalColor; // 오른쪽 밝게
        }
        else if (position.ToLower() == "none" || position.ToLower() == "all")
        {
            leftIllustration.color = normalColor;
            rightIllustration.color = normalColor;
        }
    }

    //일러스트 창을 끄고 초기화, 혹은 <<ClearSprites>> 명령어 사용 시 호출
    private void ClearAllIllustrations()
    {
        leftIllustration.gameObject.SetActive(false);
        rightIllustration.gameObject.SetActive(false);
    }
}