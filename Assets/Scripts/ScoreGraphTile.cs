using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGraphTile : MonoBehaviour
{
    [SerializeField] private ScoreGraphEntry graphTile;

    [Header("Animation")]
    [SerializeField] private float lerpDuration = 0.4f;

    [Header("Colour Mapping")]
    [SerializeField] private Color lowScoreColor = Color.red;
    [SerializeField] private Color highScoreColor = Color.green;

    private int[] scores;
    private Coroutine animateRoutine;

    public void SetGraphData(string label, int[] scoreData)
    {
        scores = scoreData;
        graphTile.label.text = label;

        SetImmediate(0);
    }

    public void UpdateGraphTile(int dayIndex)
    {
        if (scores == null || scores.Length == 0)
            return;

        dayIndex = Mathf.Clamp(dayIndex, 0, scores.Length - 1);

        float target = scores[dayIndex] / 100f;

        graphTile.scorePercent.text = scores[dayIndex] + "%";

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        animateRoutine = StartCoroutine(AnimateFillAndColor(target));
    }

    private IEnumerator AnimateFillAndColor(float targetFill)
    {
        float startFill = graphTile.graphFill.fillAmount;
        Color startColor = graphTile.graphFill.color;

        Color targetColor = Color.Lerp(lowScoreColor, highScoreColor, targetFill);

        float time = 0f;

        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            float t = time / lerpDuration;

            graphTile.graphFill.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            graphTile.graphFill.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        graphTile.graphFill.fillAmount = targetFill;
        graphTile.graphFill.color = targetColor;
    }

    private void SetImmediate(int dayIndex)
    {
        if (scores == null || scores.Length == 0)
            return;

        float value = scores[dayIndex] / 100f;

        graphTile.graphFill.fillAmount = value;
        graphTile.graphFill.color = Color.Lerp(lowScoreColor, highScoreColor, value);
        graphTile.scorePercent.text = scores[dayIndex] + "%";
    }
}


[Serializable]
internal class ScoreGraphEntry
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI scorePercent;
    public Image graphFill;
}