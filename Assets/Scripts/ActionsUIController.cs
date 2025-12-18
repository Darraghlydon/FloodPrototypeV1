using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionsUIController : MonoBehaviour
{
    [SerializeField] private ActionOptionButton optionPrefab;
    [SerializeField] private Transform optionParent;
    [SerializeField] private TextAsset textAsset;

    [Header("Icons")]
    [SerializeField] private ActionIconEntry[] actionIcons;

    private Dictionary<ResilienceActionType, Sprite> iconLookup;
    private InteractiveMenuRoot menuData;

    private void Awake()
    {
        BuildIconLookup();
    }

    private void Start()
    {
        InitialiseMenu(textAsset.text);
    }

    private void BuildIconLookup()
    {
        iconLookup = new Dictionary<ResilienceActionType, Sprite>();

        if (actionIcons == null) return;

        foreach (var entry in actionIcons)
        {
            if (entry == null) continue;

            iconLookup[entry.actionType] = entry.icon;
        }
    }

    public void InitialiseMenu(string json)
    {
        menuData = JsonUtility.FromJson<InteractiveMenuRoot>(json);

        foreach (Transform child in optionParent)
            Destroy(child.gameObject);

        foreach (var option in menuData.options)
        {
            var tile = Instantiate(optionPrefab, optionParent);

            var actionType = (ResilienceActionType)option.actionType;
            Sprite icon = GetIcon(actionType);

            tile.Initialise(option, icon, OnOptionSelected);
        }
    }

    private void OnOptionSelected(MenuOptionData option)
    {
        var actionType = (ResilienceActionType)option.actionType;
        Debug.Log($"Selected action: {actionType}");
    }

    private Sprite GetIcon(ResilienceActionType type)
    {
        if (iconLookup == null) BuildIconLookup();

        return iconLookup.TryGetValue(type, out var sprite) ? sprite : null;
    }
}


[Serializable]
public class InteractiveMenuRoot
{
    public string mode;
    public string menuTitle;
    public MenuOptionData[] options;
}



public enum ResilienceActionType
{
    BridgeInspection = 1,
    ScheduleRepairs = 2,
    RoadDiversion = 3,
    IncreaseMonitoring = 4
}

[Serializable]
public class ActionIconEntry
{
    public ResilienceActionType actionType;
    public Sprite icon;
}
