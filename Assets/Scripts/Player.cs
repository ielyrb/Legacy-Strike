using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Dictionary<ResourceType, int> resources { get;  set; } 
    public Dictionary<BuildingType, int> buildings { get; set; }
    public List<Item> items { get; set; }
    public Stats stats { get; set; }

    public void Save()
    {
        string _resources = JsonConvert.SerializeObject(resources);
        string _buildings = JsonConvert.SerializeObject(buildings);
        string _stats = JsonConvert.SerializeObject(stats);
        PlayerPrefs.SetString(Globals.ResourcesKey, _resources);
        PlayerPrefs.SetString(Globals.BuildingKey, _buildings);
        PlayerPrefs.SetString(Globals.StatsKey, _stats);
    }

    public Player()
    {
        resources = new Dictionary<ResourceType, int> 
        {
            {   ResourceType.Gold, 0 },
            {   ResourceType.Stamina, 5 },
            {   ResourceType.ShieldToken, 2 },
            {   ResourceType.GymToken, 2 },
            {   ResourceType.ArenaToken, 2 },
            {   ResourceType.AttackToken, 2}
        };
        buildings = new Dictionary<BuildingType, int>
        {
            { BuildingType.Gym, 0},
            { BuildingType.Arena, 0 }
        };
        stats = new Stats();
        items = new List<Item>();
    }
}
