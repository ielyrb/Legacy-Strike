using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public static event Action OnGoldUpdated;
    public static event Action OnShieldsUpdated;
    public static event Action<int> OnDiceRechargeTimeUpdated;
    public static event Action OnDiceRollUpdated;
    public static event Action OnBuildingsUpdated;

    private Dictionary<ResourceType, Resource> _resources;
    private Dictionary<BuildingType, int> _buildings;

    private bool _isLoggedIn;
    public int diceRechargeTimeRemaining {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
            InitializeBuildings();
        }
        else
        {
            Destroy(gameObject);
        }
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
        diceRechargeTimeRemaining = 10;
        StartCoroutine(RechargeResourceLoop());
    }

    IEnumerator RechargeResourceLoop()
    {
        while (_isLoggedIn && GetResource(ResourceType.DiceRoll) < 100)
        {
            yield return new WaitForSeconds(1f);
            diceRechargeTimeRemaining--;
            if (diceRechargeTimeRemaining <= 0)
            {
                diceRechargeTimeRemaining = 10;
                AddResource(ResourceType.DiceRoll, 1);
                OnDiceRollUpdated?.Invoke();
            }
            OnDiceRechargeTimeUpdated?.Invoke(diceRechargeTimeRemaining);
        }
        Debug.Log("Player logged out, loop stopped");
    }

    private void InitializeResources()
    {
        _resources = new Dictionary<ResourceType, Resource>
        {
            { ResourceType.Gold,        new Resource(ResourceType.Gold, 100) },
            { ResourceType.DiceRoll,    new Resource(ResourceType.DiceRoll, 3) },
            { ResourceType.AttackToken, new Resource(ResourceType.AttackToken) },
            { ResourceType.Shield,      new Resource(ResourceType.Shield) },
            { ResourceType.Energy,      new Resource(ResourceType.Energy) }
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

    public int GetBuilding(BuildingType buildingType)
    {
        return _buildings[buildingType];
    }

    public void UpdateBuildingLevel(BuildingType buildingType, int newLevel)
    {
        _buildings[buildingType] = newLevel;
        OnBuildingsUpdated?.Invoke();
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

            case ResourceType.DiceRoll:
                OnDiceRollUpdated?.Invoke();
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
            //Debug.Log($"Not enough {type} to spend. Needed {amount}, have {_resources[type].Amount}.");
            return false;
        }

        _resources[type].Amount -= amount;

        switch(type)
        {
            case ResourceType.Gold:
                OnGoldUpdated?.Invoke();
                break;

            case ResourceType.DiceRoll:
                OnDiceRollUpdated?.Invoke();
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
    DiceRoll,
    AttackToken,
    Shield,
    Energy
}
