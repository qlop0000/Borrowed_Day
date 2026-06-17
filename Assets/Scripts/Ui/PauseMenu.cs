using UnityEngine;
using Yarn.Unity;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panel 연결")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;
    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
                return; 

            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // 🟢 게임으로 돌아가기
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("게임 계속하기");
    }

    // 🔴 게임 일시정지
    void Pause()
    {
        pauseMenuPanel.SetActive(true); 
        Time.timeScale = 0f;            //게임 흐름 멈춤
        isPaused = true;
        Debug.Log("게임 일시정지");
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit(); // 게임 플레이 종료
    }
}