using System;

public class Globals 
{
    public const string ResourcesKey = "Resources";
    public const string BuildingKey = "Buildings";
    public const string StatsKey = "Stats";
    public const string InventoryKey = "Inventory";
    //public const string GoldKey = "Gold";
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
    ShieldToken,
    GymToken,
    ArenaToken
}

public class Stats
{
    public int attack = 1;
    public int defense = 1;
    public float critChance = 10;
}

public enum StatType
{
    Attack,
    Defense,
    Critchance
}