using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public static event Action<bool, int> OnGoldUpdated;
    public static event Action OnShieldsUpdated;
    public static event Action<int> OnStaminaRechargeTimeUpdated;
    public static event Action OnStaminaUpdated;
    public static event Action OnBuildingsUpdated;
    public static event Action OnStatsUpdated;
    public static event Action OnInventoryUpdated;
    public Player player { get; private set; } = new Player();

    private bool _isLoggedIn;
    private bool _rechargingStamina;
    public int staminaRechargeTime;
    public int staminaRechargeTimeRemaining {  get; private set; }
    public int maxStamina {  get; private set; }
    public int winMultiplier { get; private set; }

    public float glancingBlowChance;

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
        LoadResources();
        LoadBuildings();
        LoadStats();
        LoadInventory();
        glancingBlowChance = GameManager.Instance.settings.glancingBlowChance;
    }

    public void ReloadStats()
    {
        OnStatsUpdated?.Invoke();
    }

    void LoadResources()
    {
        string defaultValue = JsonConvert.SerializeObject(player.resources);
        string res = PlayerPrefs.GetString(Globals.ResourcesKey, defaultValue);
        player.resources = JsonConvert.DeserializeObject<Dictionary<ResourceType, int>>(res);
    }

    void LoadBuildings()
    {
        string defaultValue = JsonConvert.SerializeObject(player.buildings);
        string res = PlayerPrefs.GetString(Globals.BuildingKey, defaultValue);
        player.buildings = JsonConvert.DeserializeObject<Dictionary<BuildingType, int>>(res);
    }

    void LoadInventory()
    {
        string defaultValue = JsonConvert.SerializeObject(player.items);
        string res = PlayerPrefs.GetString(Globals.InventoryKey, defaultValue);
        player.items = JsonConvert.DeserializeObject<List<Item>>(res);
    }

    private void LoadStats()
    {
        maxStamina = GameManager.Instance.settings.baseStamina + (GetBuilding(BuildingType.Gym) * GameManager.Instance.settings.gymStaminaMultiplier);
        staminaRechargeTime = GameManager.Instance.settings.baseStaminaRechargeTime - (GetBuilding(BuildingType.Gym) * GameManager.Instance.settings.gymRechargeTimeMultiplier);
        player.stats.attack = GameManager.Instance.settings.baseAttack + (GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaAttackMultiplier);
        player.stats.defense = GameManager.Instance.settings.baseDefense + (GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaDefenseMultiplier);
        winMultiplier = GetBuilding(BuildingType.Arena) == 0 ? 1 : GetBuilding(BuildingType.Arena) * GameManager.Instance.settings.arenaWinMultiplier;
        OnStatsUpdated?.Invoke();
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

    public int GetBuilding(BuildingType buildingType)
    {
        return player.buildings[buildingType];
    }

    public void UpdateBuildingLevel(BuildingType buildingType, int newLevel)
    {
        player.buildings[buildingType] = newLevel;
        OnBuildingsUpdated?.Invoke();
        LoadStats();
    }

    public int GetResource(ResourceType type)
    {
        return player.resources[type];
    }

    public void AddResource(ResourceType type, int amount)
    {
        player.resources[type] += amount;

        switch (type)
        {
            case ResourceType.Gold:
                OnGoldUpdated?.Invoke(true, amount);
                break;

            case ResourceType.Stamina:
                OnStaminaUpdated?.Invoke();
                break;

            case ResourceType.AttackToken:
                OnInventoryUpdated?.Invoke();
                break;

            case ResourceType.ShieldToken:
                OnShieldsUpdated?.Invoke();
                OnInventoryUpdated?.Invoke();
                break;

            case ResourceType.GymToken:
                OnInventoryUpdated?.Invoke();
                break;

            case ResourceType.ArenaToken:
                OnInventoryUpdated?.Invoke();
                break;
        }
        NotificationManager.Instance.ShowResourceUpdate(type, amount);
        Save();
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (player.resources[type] < amount)
        {
            NotificationManager.Instance.ShowMessage($"Not enough {type}, you need {amount - player.resources[type]} more");
            return false;
        }

        if(amount < 0)
            amount = Mathf.Abs(amount);

        player.resources[type] -= amount;

        switch(type)
        {
            case ResourceType.Gold:
                OnGoldUpdated?.Invoke(false, amount);
                break;

            case ResourceType.Stamina:
                OnStaminaUpdated?.Invoke();

                if (GetResource(ResourceType.Stamina) < maxStamina)
                    StartResourceRecharge();

                break;
        }
        Save();

        if (type == ResourceType.Stamina) //Don't show stamina popup because it's weird
            return true;

        NotificationManager.Instance.ShowResourceUpdate(type, amount * -1);
        return true;
    }

    public void Save()
    {
        player.Save();
    }
}
