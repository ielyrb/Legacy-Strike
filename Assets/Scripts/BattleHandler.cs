using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    private AttackType _attackType;
    private DefenseType _defenseType;

    private Stats _attacker;
    private Stats _defender;

    private bool _attackSet;

    private static readonly Dictionary<(AttackType, DefenseType), FightResult> _fightMatrix =
        new Dictionary<(AttackType, DefenseType), FightResult>
        {
            { (AttackType.Jab,      DefenseType.Parry), FightResult.DefenderWins },
            { (AttackType.Jab,      DefenseType.Catch), FightResult.FullHit },
            { (AttackType.Jab,      DefenseType.Roll),  FightResult.PartialHit },

            { (AttackType.Hook,     DefenseType.Catch), FightResult.DefenderWins },
            { (AttackType.Hook,     DefenseType.Roll),  FightResult.FullHit },
            { (AttackType.Hook,     DefenseType.Parry), FightResult.PartialHit },

            { (AttackType.Uppercut, DefenseType.Roll),  FightResult.DefenderWins },
            { (AttackType.Uppercut, DefenseType.Parry), FightResult.FullHit },
            { (AttackType.Uppercut, DefenseType.Catch), FightResult.PartialHit },
        };

    private void Start()
    {
        SetupPlayers(GameManager.Instance.attacker, GameManager.Instance.defender);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ExitScene());
        }
    }

    public void SetAttackType(int value)
    {
        _attackType = (AttackType)value;
        _attackSet = true;
        //NotificationManager.Instance.ShowMessage($"Attack type set to {_attackType}");
        StartFight();
    }

    public void SetupPlayers(Stats attacker, Stats defender)
    {
        _attacker = attacker;
        _defender = defender;
        _defenseType = (DefenseType)Random.Range(0, 3);
    }

    public void StartFight()
    {
        if(!_attackSet)
        {
            NotificationManager.Instance.ShowMessage("Select an attack type first");
            return;
        }

        FightResult res = FightResult.DefenderWins;
        bool glancingBlow = false;
        int damage = OnStartFight(out res, out glancingBlow);
        int reward = 0;

        if (damage > 0)
        {
            reward = damage * GameManager.Instance.settings.fightBaseGoldReward;
            reward = Mathf.RoundToInt(res == FightResult.PartialHit ? reward / 2 : reward);
        }

        switch (res)
        {
            case FightResult.PartialHit:
                if (glancingBlow)
                {
                    NotificationManager.Instance.ShowMessage($"Glancing Blow! Defender used {_defenseType} attacker used {_attackType}. Partial damage taken and won {reward}");
                }
                else
                {
                    NotificationManager.Instance.ShowMessage($"Mismatch! Defender used {_defenseType} but attacker used {_attackType}. Partial damage taken and won {reward}");
                }
                break;

            case FightResult.FullHit:
                int rand = Random.Range(0, 100);
                bool crit = PlayerManager.Instance.player.stats.critChance >= 100 - rand;
                if (crit)
                {
                    NotificationManager.Instance.ShowMessage($"Attacker's {_attackType} landed clean! Defender failed with {_defenseType} and won {reward}!");
                }
                else
                {
                    NotificationManager.Instance.ShowMessage($"Attacker's {_attackType} landed clean! Defender failed with {_defenseType} and won {reward}!");
                }
                break;

            default:
                NotificationManager.Instance.ShowMessage($"Defender successfully blocked {_attackType} with {_defenseType}!");
                break;
        }
        PlayerManager.Instance.AddResource(ResourceType.Gold, reward);
        StartCoroutine(ExitScene());
    }

    IEnumerator ExitScene()
    {
        yield return new WaitForSeconds(2.5f);
        LoadingScreenManager.Instance.LoadScene("Main");
    }

    public int OnStartFight(out FightResult res, out bool glancingBlow)
    {
        int damage = 0;
        res = GetFightResult();
        glancingBlow = false;
        switch (res)
        {
            case FightResult.PartialHit:
                int rand = Random.Range(0, 100);
                //glancingBlow = GameManager.Instance.settings.glancingBlowChance >= 100 - rand;
                glancingBlow = PlayerManager.Instance.glancingBlowChance >= 100 - rand;
                int attackPower = glancingBlow ? _attacker.attack : _attacker.attack / 2;
                damage = Mathf.Max(1, attackPower - _defender.defense);
                break;

            case FightResult.FullHit:
                damage = Mathf.Max(1, _attacker.attack - _defender.defense);
                break;
        }

        return damage;
    }

    public FightResult GetFightResult()
    {
        return _fightMatrix[(_attackType, _defenseType)];
    }

}

public enum AttackType { Jab, Hook, Uppercut }
public enum DefenseType { Parry, Catch, Roll }
public enum FightResult { DefenderWins, PartialHit, FullHit }
