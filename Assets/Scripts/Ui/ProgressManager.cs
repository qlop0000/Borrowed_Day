using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("진도 단계")]
    [SerializeField] private int currentProgress = 1;

    private AtmosphereManager atmosphereManager;

    private void Awake()
    {
        // 싱글톤 세팅
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        atmosphereManager = FindAnyObjectByType<AtmosphereManager>();
    }

    // 진도를 가져오는 함수
    public int GetProgress()
    {
        return currentProgress;
    }

    // 대화가 아닌 곳(아이템 획득, 몬스터 처치 등)에서 진도를 올릴 때 호출할 함수
    public void SetProgress(int newProgress)
    {
        currentProgress = newProgress;
        Debug.Log($"[ProgressManager] 게임 진도 변경 : {currentProgress}");

        // 조명 새로고침.
        UpdateCurrentAtmosphere();
    }

    // 현재 진도에 맞게 조명을 입히는 헬퍼 함수
    public void UpdateCurrentAtmosphere(float duration = 1.0f)
    {
        if (atmosphereManager == null) atmosphereManager = FindAnyObjectByType<AtmosphereManager>();
        if (atmosphereManager == null) return;

        if (currentProgress >= 3) atmosphereManager.ChangeTone("Night", duration);
        else if (currentProgress == 2) atmosphereManager.ChangeTone("Evening", duration);
        else atmosphereManager.ChangeTone("Default", duration);
    }
}