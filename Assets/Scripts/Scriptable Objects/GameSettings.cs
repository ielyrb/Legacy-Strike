using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Legacy Strike/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public int baseStamina = 20;
    public int staminaRechargeAmount = 5;
    public int baseStaminaRechargeTime = 60;

    public int gymStaminaMultiplier = 2;
    public int gymRechargeTimeMultiplier = 10;

    public int arenaAttackMultiplier = 5;
    public int arenaDefenseMultiplier = 5;

    public int fightBaseGoldReward = 1000;
    public int arenaWinMultiplier = 2;
}