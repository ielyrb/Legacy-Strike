using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingItem : MonoBehaviour
{
    [SerializeField] private BuildingType _type;
    public int level { get; private set; }
    public int upgradeCost { get; private set; }

    [SerializeField] private List<int> _upgradeCostList;
    [SerializeField] private List<GameObject> _crowns;
    [SerializeField] private TextMeshProUGUI _costText;

    StructureBuildingHandler _handler;

    private void Awake()
    {
        _handler = FindObjectOfType<StructureBuildingHandler>();
    }

    private void Start()
    {
        Initialize(PlayerManager.Instance.GetBuilding(_type));
    }

    private void Initialize(int _level)
    {
        level = _level;
        upgradeCost = _upgradeCostList[level];
        UpdateUI();
        UpdateCrownsUI();
    }

    private void UpdateUI()
    {
        _costText.text = upgradeCost.ToString("N0");
        UpdateCrownsUI();
    }

    void UpdateCrownsUI()
    {
        for (int i = 0; i < level; i++)
        {
            if (i > 5)
                break;

            _crowns[i].SetActive(i < level);
        }
    }

    public void UpgradeBuilding()
    {
        if(level >= 5)
        {
            NotificationManager.Instance.ShowMessage("You already reached the max level");
            return;
        }

        if(PlayerManager.Instance.SpendResource(ResourceType.Gold, upgradeCost))
        {            
            level++;
            string message = $"Upgrade completed! Your {_type} is now level {level}";
            PlayerManager.Instance.UpdateBuildingLevel(_type, level);
            _handler.UpgradeBuilding(_type, level, message);
            upgradeCost = _upgradeCostList[Mathf.Min(4,level)];
            UpdateUI();
        }
    }
}

public enum BuildingType
{
    Gym,
    Arena
}
