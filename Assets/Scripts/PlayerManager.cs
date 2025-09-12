using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public static event Action<ResourceType, int> OnResourceUpdated;
    public static event Action<ResourceType, int> OnRechargeTimeUpdated;

    private Dictionary<ResourceType, Resource> _resources;

    private bool _isLoggedIn;
    public Dictionary<ResourceType, int> resourceRechargeTimeRemaining {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeLoggedInState(bool value)
    {
        _isLoggedIn = value;

        if (_isLoggedIn)
            StartResourceRecharge();
    }

    public void StartResourceRecharge()
    {
        resourceRechargeTimeRemaining[ResourceType.Energy] = 10;
        resourceRechargeTimeRemaining[ResourceType.DiceRoll] = 30;
        StartCoroutine(RechargeResourceLoop());
    }

    IEnumerator RechargeResourceLoop()
    {
        while (_isLoggedIn)
        {
            yield return new WaitForSeconds(1f);
            resourceRechargeTimeRemaining[ResourceType.Energy]--;
            resourceRechargeTimeRemaining[ResourceType.DiceRoll]--;

            if (resourceRechargeTimeRemaining[ResourceType.Energy] <= 0)
            {                
                AddResource(ResourceType.Energy, 1);
                ResetResourceRechargeTimer(ResourceType.Energy);
            }

            if (resourceRechargeTimeRemaining.ContainsKey(ResourceType.DiceRoll))
            {
                AddResource(ResourceType.DiceRoll, 1);
                ResetResourceRechargeTimer(ResourceType.DiceRoll);
            }
            OnRechargeTimeUpdated?.Invoke(ResourceType.DiceRoll, resourceRechargeTimeRemaining[ResourceType.DiceRoll]);
            OnRechargeTimeUpdated?.Invoke(ResourceType.Energy, resourceRechargeTimeRemaining[ResourceType.Energy]);
        }
        Debug.Log("Player logged out, loop stopped");
    }

    void ResetResourceRechargeTimer(ResourceType type)
    {
        switch(type)
        {
            case ResourceType.Energy:
                resourceRechargeTimeRemaining[ResourceType.Energy] = 10;
                break;

                case ResourceType.DiceRoll:
                resourceRechargeTimeRemaining[ResourceType.DiceRoll] = 30;
                break;
            default:
                Debug.Log("Resource not found!");
                break;
        }
    }

    private void InitializeResources()
    {
        _resources = new Dictionary<ResourceType, Resource>
        {
            { ResourceType.Gold,        new Resource(ResourceType.Gold, 100) },
            { ResourceType.DiceRoll,    new Resource(ResourceType.DiceRoll) },
            { ResourceType.AttackToken, new Resource(ResourceType.AttackToken) },
            { ResourceType.Shield,      new Resource(ResourceType.Shield) },
            { ResourceType.Energy,      new Resource(ResourceType.Energy) }
        };
        ResetResourceRechargeTimer(ResourceType.Energy);
        ResetResourceRechargeTimer(ResourceType.DiceRoll);
    }

    public int GetResource(ResourceType type)
    {
        return _resources[type].Amount;
    }

    public void AddResource(ResourceType type, int amount)
    {
        _resources[type].Amount += amount;
        OnResourceUpdated?.Invoke(type, _resources[type].Amount);
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (_resources[type].Amount < amount)
        {
            Debug.Log($"Not enough {type} to spend. Needed {amount}, have {_resources[type].Amount}.");
            return false;
        }

        _resources[type].Amount -= amount;
        OnResourceUpdated?.Invoke(type, _resources[type].Amount);
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
