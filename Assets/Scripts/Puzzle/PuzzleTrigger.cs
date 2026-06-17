using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("트리거의 이름")]
    public string triggerName;

    // 조건이 달성 여부
    [SerializeField] private bool isCompleted = false;

    // 외부에서 체크
    public bool IsCompleted => isCompleted;

    // 퍼즐 조건이 완료되었을 때 외부에서 호출
    // (예: 양동이가 발판에 올라왔을 때 SetTriggerComplete(true) 가 실행됨)
    public void SetTriggerComplete(bool state)
    {
        isCompleted = state;
        Debug.Log($"트리거 {triggerName} 상태 {state} 변경");

        PuzzleReaction[] reactions = FindObjectsByType<PuzzleReaction>(FindObjectsSortMode.None);
        foreach (var reaction in reactions)
        {
            reaction.CheckConditions();
        }
    }
}