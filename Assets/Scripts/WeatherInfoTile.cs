using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherInfoTile : MonoBehaviour
{
    [SerializeField] private WeatherInfo weatherTile;

    public void UpdateWeatherIconData(string dayInfo, Sprite weatherIcon, string weatherDescription)
    {
        weatherTile.dayInfo.text = dayInfo;
        weatherTile.weatherIcon.sprite = weatherIcon;
        weatherTile.weatherDescription.text = weatherDescription;
    }
    
    public void SetAsCurrentDay(bool isCurrentDay)
    {
        weatherTile.border.SetActive(isCurrentDay);
    }
}

[Serializable]
public class WeatherInfo 
{
    public TextMeshProUGUI dayInfo;
    public Image weatherIcon;
    public TextMeshProUGUI weatherDescription;
    public GameObject border;
}
