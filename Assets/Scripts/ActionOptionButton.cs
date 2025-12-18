using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionOptionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button actionButton;
    [SerializeField] private Image iconImage;

    private MenuOptionData optionData;

    public void Initialise(
          MenuOptionData data,
          Sprite icon,
          System.Action<MenuOptionData> onSelected)
    {
        optionData = data;

        labelText.text = data.label;
        descriptionText.text = data.description;
        costText.text = $"€{data.cost}";
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;

        actionButton.interactable = data.enabled;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onSelected?.Invoke(optionData));
    }
}

[Serializable]
public class MenuOptionData
{
    public string id;
    public string label;
    public string description;
    public string cost;
    public int actionType;
    public bool enabled;
}
