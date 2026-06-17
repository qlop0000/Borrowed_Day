using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PuzzleReaction : MonoBehaviour
{
    [Header("결과가 일어나기 위해 필요한 트리거 목록")]
    public List<PuzzleTrigger> requiredTriggers;

    [Header("모든 조건이 충족되었을 때 일어날 행동 (UnityEvent)")]
    public UnityEvent onPuzzleSolved;

    private bool isSolved = false; // 중복 실행 방지

    private void Start()
    {
        // 게임이 시작될 때 조건이 풀려있는지 체크
        CheckConditions();
    }

    // 신호를 보낼 때마다 실행되는 검사 함수
    public void CheckConditions()
    {
        if (isSolved) return; // 이미 퍼즐이 풀려서 결과가 나왔다면 무시
        if (requiredTriggers == null || requiredTriggers.Count == 0) return;

        // 등록된 트리거 검사
        foreach (PuzzleTrigger trigger in requiredTriggers)
        {
            if (trigger == null || !trigger.IsCompleted)
            {
                // false가 있으면 검사 종료.
                return;
            }
        }

        // 반복문 통과 이후
        isSolved = true;
        Debug.Log($"{gameObject.name}에 연결된 모든 조건 완료");

        // 등록해둔 행동 실행
        onPuzzleSolved?.Invoke();
    }
}