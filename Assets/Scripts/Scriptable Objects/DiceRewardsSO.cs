using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice Roll Rewards", menuName = "Legacy Strike/Dice Roll Rewards", order = 0)]
public class DiceRewardsSO : ScriptableObject
{
    public DiceReward[] gold;
    public DiceReward[] stamina;
    public DiceReward[] combatTokens;
}

[Serializable]
public class DiceReward
{
    public RewardType type;
    public int amount;
    public Sprite icon;
}