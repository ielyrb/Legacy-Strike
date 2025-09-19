using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public static event Action OnGoldUpdated;
    public static event Action OnShieldsUpdated;
    public static event Action<int> OnStaminaRechargeTimeUpdated;
    public static event Action OnStaminaUpdated;
    public static event Action OnBuildingsUpdated;

    private Dictionary<ResourceType, Resource> _resources;
    private Dictionary<BuildingType, int> _buildings;
    public Stats _basePlayerStats { get; private set; } = new Stats();
    public Stats _playerStats { get; private set; } = new Stats();

    private bool _isLoggedIn;
    private bool _rechargingStamina;
    public int staminaRechargeTime;
    public int staminaRechargeTimeRemaining {  get; private set; }
    public int maxStamina {  get; private set; }
    public int winMultiplier { get; private set; }
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeResources();
        InitializeBuildings();
        FinalizeStats();
    }

    public void ChangeLoggedInState(bool value)
    {
        _isLoggedIn = value;

        //if (_isLoggedIn)
        //    StartResourceRecharge();
    }

    public void StartResourceRecharge()
    {
        if (!_isLoggedIn)
            return;

        if (_rechargingStamina)
            return;

        _rechargingStamina = true;
        staminaRechargeTimeRemaining = staminaRechargeTime;
        StartCoroutine(RechargeResourceLoop());
    }

    IEnumerator RechargeResourceLoop()
    {
        while (_isLoggedIn && GetResource(ResourceType.Stamina) < maxStamina)
        {
            yield return new WaitForSeconds(1f);
            staminaRechargeTimeRemaining--;
            if (staminaRechargeTimeRemaining <= 0)
            {
                staminaRechargeTimeRemaining = staminaRechargeTime;
                AddResource(ResourceType.Stamina, GameManager.Instance.settings.staminaRechargeAmount);
                OnStaminaUpdated?.Invoke();
            }
            OnStaminaRechargeTimeUpdated?.Invoke(staminaRechargeTimeRemaining);
        }
        _rechargingStamina = false;
    }

    private void InitializeResources()
    {
        _resources = new Dictionary<ResourceType, Resource>
        {
            { ResourceType.Gold,        new Resource(ResourceType.Gold, 0) },
            { ResourceType.Stamina,    new Resource(ResourceType.Stamina, 5) },
            { ResourceType.AttackToken, new Resource(ResourceType.AttackToken, 0) },
            { ResourceType.Shield,      new Resource(ResourceType.Shield, 0) }
        };
    }

    private void InitializeBuildings()
    {
        _buildings = new Dictionary<BuildingType, int>
        {
            { BuildingType.Gym, 0},
            { BuildingType.Arena, 0},
        };
    }

    private void FinalizeStats()
    {
        maxStamina = GameManager.Instance.settings.baseStamina + (GetBuilding(BuildingType.Gym) * GameManager.Instance.settings.gymStaminaMultiplier);
        staminaRechargeTime = GameManager.Instance.settings.baseStaminaRechargeTime - (GetBuilding(BuildingType.Gym) * GameManager.Instance.settings.gymRechargeTimeMultiplier);
        _playerStats.attack = _basePlayerStats.attack + (GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaAttackMultiplier);
        _playerStats.defense = _basePlayerStats.defense + (GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaDefenseMultiplier);
        winMultiplier = GetBuilding(BuildingType.Arena) == 0 ? 1 : GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaWinMultiplier;
    }

    public int GetBuilding(BuildingType buildingType)
    {
        return _buildings[buildingType];
    }

    public void UpdateBuildingLevel(BuildingType buildingType, int newLevel)
    {
        _buildings[buildingType] = newLevel;
        OnBuildingsUpdated?.Invoke();
        FinalizeStats();
    }

    public int GetResource(ResourceType type)
    {
        return _resources[type].Amount;
    }

    public void AddResource(ResourceType type, int amount)
    {
        _resources[type].Amount += amount;

        if (type == ResourceType.Shield && _resources[type].Amount > 3)
            _resources[type].Amount = 3;

        switch (type)
        {
            case ResourceType.Gold:
                OnGoldUpdated?.Invoke();
                break;

            case ResourceType.Stamina:
                OnStaminaUpdated?.Invoke();
                break;

            case ResourceType.Shield:
                OnShieldsUpdated?.Invoke();
                break;
        }
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (_resources[type].Amount < amount)
        {
            NotificationManager.Instance.ShowMessage($"Not enough {type}, you need {amount - _resources[type].Amount} more");
            return false;
        }

        _resources[type].Amount -= amount;

        switch(type)
        {
            case ResourceType.Gold:
                OnGoldUpdated?.Invoke();
                break;

            case ResourceType.Stamina:
                OnStaminaUpdated?.Invoke();

                if (GetResource(ResourceType.Stamina) < maxStamina)
                    StartResourceRecharge();

                break;
        }
        return true;
    }
}

[Serializable]
public class Resource
{
    public ResourceType Type { get; private set; }
    public int Amount { get; set; }

    public Resource(ResourceType type, int initialAmount = 0)
    {
        Type = type;
        Amount = initialAmount;
    }
}

public enum ResourceType
{
    Gold,
    Stamina,
    AttackToken,
    Shield
}

public class Stats
{
    public int attack = 1;
    public int defense = 1;
}
