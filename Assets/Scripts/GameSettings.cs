using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Legacy Strike/Game Settings", order = 1)]
public class GameSettings : ScriptableObject
{
    public int maxRolls = 50;
    public int rollRechargeAmount = 5;
    public int rollRechargeTime = 600;
}