using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHandler : MonoBehaviour
{
    [Header("Text Results")]
    [SerializeField] TextMeshProUGUI _crit;
    [SerializeField] TextMeshProUGUI _result;
    [SerializeField] TextMeshProUGUI _reward;

    [Header("Attack BUttons")]
    [SerializeField] private Button _jab;
    [SerializeField] private Button _hook;
    [SerializeField] private Button _uppercut;

    [Header("Defense Buttons")]
    [SerializeField] private Button _parry;
    [SerializeField] private Button _catch;
    [SerializeField] private Button _roll;

    private AttackType _attackType;
    private DefenseType _defenseType;

    private Stats _attacker;
    private Stats _defender;

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
        StartCoroutine(StartFight());
    }

    public void SetupPlayers(Stats attacker, Stats defender)
    {
        _attacker = attacker;
        _defender = defender;
        _defenseType = (DefenseType)Random.Range(0, 3);
    }

    IEnumerator StartFight()
    {
        FightResult res = FightResult.DefenderWins;
        bool glancingBlow = false;
        bool crit = false;
        int damage = OnStartFight(out res, out crit, out glancingBlow);
        int reward = 0;

        if (damage > 0)
        {
            reward = damage * GameManager.Instance.settings.fightBaseGoldReward;
            reward = Mathf.RoundToInt(res == FightResult.PartialHit ? reward / 2 : reward);
        }

        _jab.interactable = _attackType == AttackType.Jab;
        _hook.interactable = _attackType == AttackType.Hook;
        _uppercut.interactable = _attackType == AttackType.Uppercut;
        _parry.interactable = _defenseType == DefenseType.Parry;
        _catch.interactable = _defenseType == DefenseType.Catch;
        _roll.interactable = _defenseType == DefenseType.Roll;

        yield return new WaitForSeconds(.5f);

        if (crit)
        {
            _crit.SetText("Critical Hit!");
            _crit.gameObject.SetActive(true);
        }
        else if (glancingBlow)
        {
            _crit.SetText("Glancing Blow!");
            _crit.gameObject.SetActive(true);
        }

        _result.SetText($"{AddSpaces(res.ToString())}");
        _reward.SetText($"{reward.ToString("N0")} Gold");
        _result.gameObject.SetActive(true);
        _reward.gameObject.SetActive(true);

        PlayerManager.Instance.AddResource(ResourceType.Gold, reward);
        yield return new WaitForSeconds(2f);
        StartCoroutine(ExitScene());
    }

    IEnumerator ExitScene()
    {
        yield return new WaitForSeconds(0.5f);
        LoadingScreenManager.Instance.LoadScene("Main");
    }

    public int OnStartFight(out FightResult res, out bool crit, out bool glancingBlow)
    {
        int damage = 0;
        res = GetFightResult();
        crit = false;
        glancingBlow = false;
        switch (res)
        {
            case FightResult.PartialHit:
                int randGlance = Random.Range(0, 100);
                //glancingBlow = GameManager.Instance.settings.glancingBlowChance >= 100 - rand;
                glancingBlow = PlayerManager.Instance.glancingBlowChance >= 100 - randGlance;
                int attackPower = glancingBlow ? _attacker.attack : _attacker.attack / 2;
                damage = Mathf.Max(1, attackPower - _defender.defense);
                break;

            case FightResult.FullHit:
                damage = Mathf.Max(1, _attacker.attack - _defender.defense);
                int randCrit = Random.Range(0, 100);
                crit = PlayerManager.Instance.player.stats.critChance >= 100 - randCrit;
                if (crit)
                    damage *= 2;
                break;
        }

        return damage;
    }

    public FightResult GetFightResult()
    {
        return _fightMatrix[(_attackType, _defenseType)];
    }

    public string AddSpaces(string camelCaseString)
    {
        if (string.IsNullOrEmpty(camelCaseString))
        {
            return camelCaseString;
        }
        return Regex.Replace(camelCaseString, "([A-Z])", " $1").Trim();
    }

}

public enum AttackType { Jab, Hook, Uppercut }
public enum DefenseType { Parry, Catch, Roll }
public enum FightResult { DefenderWins, PartialHit, FullHit }
