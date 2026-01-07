using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForecastSimulatorManager : MonoBehaviour
{
    [SerializeField]
    [Range(1,10)]
    [Header("Update Rate in Seconds")]
    private int simulationTickRate = 2;

    [SerializeField] private ForecastUIController forecastUIController;
    [SerializeField] private DataInsightUIController dataInsightUIController;
    //Remove this after UI Mockup Demo - DL
    [SerializeField] private RawImage townPreview;
    [SerializeField] private Texture[] townImages;
    private Coroutine routine;
    private int currentDayIndex = 0;
    

    [SerializeField] private GameObject waterObject;
    private Vector3 waterStartPos = Vector3.zero;
    private Dictionary<int, float> waterLevelByDayIndex = new Dictionary<int, float>()
    {
        { 0, 0.0f },
        { 1, 0.1f },
        { 2, 0.5f },
        { 3, 2.5f },
        { 4, 3.5f },
        { 5, 4.5f },
        { 6, 4.0f },
    };
    [SerializeField] private float waterLerpDuration = 0.5f;
    private Coroutine waterLerpRoutine;

    public void Start()
    {
        StartSimulation();
        waterStartPos = waterObject.transform.position;
    }

    public void StartSimulation()
    {
        Debug.Log("Forecast simulation started.");

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        routine = StartCoroutine(SimCoroutine());
    }

    private IEnumerator SimCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(simulationTickRate); // Simulate a step every 2 seconds
            Debug.Log($"Simulating forecast Day {currentDayIndex}");
            currentDayIndex++;
            
            //Remove this after UI Mockup Demo - DL
            townPreview.texture = townImages[currentDayIndex];
            forecastUIController.IncreaseDayTick(currentDayIndex);
            dataInsightUIController.IncreaseDayTick(currentDayIndex);
            UpdateWaterLevel(currentDayIndex);

            if(currentDayIndex >= 6) { currentDayIndex = -1; }
        }
    }

    private void UpdateWaterLevel(int dayIndex)
    {
        if (!waterLevelByDayIndex.TryGetValue(dayIndex, out float heightOffset))
            return;

        float targetY = waterStartPos.y + heightOffset;

        if (waterLerpRoutine != null)
            StopCoroutine(waterLerpRoutine);

        waterLerpRoutine = StartCoroutine(LerpWaterLevel(targetY));
    }

    private IEnumerator LerpWaterLevel(float targetY)
    {
        Vector3 startPos = waterObject.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        float elapsed = 0f;

        while (elapsed < waterLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / waterLerpDuration;

            // Smooth motion feels better for water
            t = Mathf.SmoothStep(0f, 1f, t);

            waterObject.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        waterObject.transform.position = targetPos;
    }
}
