using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ForecastUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private GameObject weatherInfoTilePrefab;
    [SerializeField] private Transform tileParent;

    [Header("Forecast Settings")]
    [SerializeField, Range(1, 7)]
    private int forecastDays = 5;

    [Header("Weather Icons")]
    [SerializeField] private WeatherIconEntry[] weatherIcons;

    private WeatherInfoTile[] weatherInfoTiles;
    private WeatherForecastRoot forecastData;

    private Dictionary<WeatherType, Sprite> iconLookup;
    [SerializeField] private TextAsset weatherJsonTextAsset;

    private void Awake()
    {
        BuildIconLookup();
        InitialiseUI(weatherJsonTextAsset.text);
    }

    public void InitialiseUI(string json)
    {
        forecastData = JsonUtility.FromJson<WeatherForecastRoot>(json);

        int daysToShow = Mathf.Min(forecastDays, forecastData.forecast.Length);

        weatherInfoTiles = new WeatherInfoTile[daysToShow];

        locationText.text = forecastData.location;

        for (int i = 0; i < daysToShow; i++)
        {
            var tileGO = Instantiate(weatherInfoTilePrefab, tileParent);
            var tile = tileGO.GetComponent<WeatherInfoTile>();

            WeatherDayData dayData = forecastData.forecast[i];
            Sprite icon = GetIcon((WeatherType)dayData.weatherType);

            tile.UpdateWeatherIconData(
                dayData.dayInfo,
                icon,
                dayData.weatherDescription
            );

            weatherInfoTiles[i] = tile;
            var isFirstDay = i == 0;
            tile.SetAsCurrentDay(isFirstDay);
        }
    }

    private void BuildIconLookup()
    {
        iconLookup = new Dictionary<WeatherType, Sprite>();

        foreach (var entry in weatherIcons)
        {
            if (!iconLookup.ContainsKey(entry.type))
                iconLookup.Add(entry.type, entry.sprite);
        }
    }

    private Sprite GetIcon(WeatherType type)
    {
        return iconLookup.TryGetValue(type, out var sprite)
            ? sprite
            : null;
    }

    public void IncreaseDayTick(int currentDayIndex)
    {
        if (currentDayIndex >= weatherInfoTiles.Length)
            return;

        for (int i = 0; i < weatherInfoTiles.Length; i++)
        {
            var isCurrDay = i == currentDayIndex;
            weatherInfoTiles[i].SetAsCurrentDay(isCurrDay);
        }
    }
}

[Serializable]
public class WeatherForecastRoot
{
    public string location;
    public string units;
    public WeatherDayData[] forecast;
}

[Serializable]
public class WeatherDayData
{
    public string dayInfo;
    public string weatherDescription;
    public int weatherType;
}

public enum WeatherType
{
    Sunny = 1,
    PartlyCloudy = 2,
    Rain = 3,
    Thunderstorms = 4,
    ExtremeFlooding = 5
}

[Serializable]
public class WeatherIconEntry
{
    public WeatherType type;
    public Sprite sprite;
}