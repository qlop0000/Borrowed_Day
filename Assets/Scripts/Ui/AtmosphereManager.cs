using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AtmosphereManager : MonoBehaviour
{
    [System.Serializable]
    public struct ToneData
    {
        public string toneName;
        public Color color;         // 빛의 색상
        public float intensity;     // 빛의 밝기
    }

    [Header("글로벌 라이트 2D 연결")]
    public Light2D globalLight;

    [Header("조명 데이터베이스")]
    public List<ToneData> toneDatabase = new List<ToneData>();

    private Dictionary<string, ToneData> toneDict = new Dictionary<string, ToneData>();
    private Coroutine toneTransitionCoroutine;

    void Start()
    {
        foreach (var tone in toneDatabase)
        {
            if (!toneDict.ContainsKey(tone.toneName))
                toneDict.Add(tone.toneName, tone);
        }
    }

    public void ChangeTone(string toneName, float duration = 1.0f)
    {
        if (!toneDict.ContainsKey(toneName)) return;

        ToneData targetTone = toneDict[toneName];

        if (toneTransitionCoroutine != null) StopCoroutine(toneTransitionCoroutine);

        if (duration <= 0f)
        {
            // 즉시 변경
            globalLight.color = targetTone.color;
            globalLight.intensity = targetTone.intensity;
        }
        else
        {
            // 부드럽게 변경
            toneTransitionCoroutine = StartCoroutine(ToneTransitionRoutine(targetTone, duration));
        }
    }

    private IEnumerator ToneTransitionRoutine(ToneData targetTone, float duration)
    {
        Color startColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            globalLight.color = Color.Lerp(startColor, targetTone.color, progress);
            globalLight.intensity = Mathf.Lerp(startIntensity, targetTone.intensity, progress);
            yield return null;
        }

        globalLight.color = targetTone.color;
        globalLight.intensity = targetTone.intensity;
    }
}