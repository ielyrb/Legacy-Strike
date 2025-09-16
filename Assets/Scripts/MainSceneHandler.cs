using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Cinemachine;

public class MainSceneHandler : MonoBehaviour
{
    [SerializeField] private RewardPreview _rewardPreview;
    [SerializeField] private TextMeshProUGUI tmp_gold;
    [SerializeField] private TextMeshProUGUI tmp_diceRoll;
    [SerializeField] private TextMeshProUGUI tmp_diceRollTimer;

    [SerializeField] private DiceRoll _diePrefab;
    [SerializeField] private List<Vector3> _diceSpawnPoints;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _diceRollCamera;

    [SerializeField] private List<GameObject> _shieldsIcon;
    [SerializeField] private GameObject _diceUI;
    [SerializeField] private GameObject _buildingUI;
    [SerializeField] private GameObject _upgradeBuildingUI;

    private List<DiceRoll> _dice = new List<DiceRoll>();
    private List<int> _rollResults = new List<int>();

    private bool _isRollingDice;

    private void Awake()
    {
        PlayerManager.OnDiceRechargeTimeUpdated += OnDiceRechargeTimeUpdated;
        PlayerManager.OnDiceRollUpdated += OnDiceRollUpdated;
        PlayerManager.OnGoldUpdated += OnGoldUpdated;
        PlayerManager.OnShieldsUpdated += OnShieldsUpdated;
        ChangeCameraState(true);
    }
    private void Start()
    {
        PlayerManager.Instance.StartResourceRecharge();
        OnGoldUpdated();
        OnDiceRollUpdated();
        OnShieldsUpdated();
    }

    #region Public Methods

    public void UpgradeBuildings()
    {
        if (_isRollingDice)
            return;

        ChangeUIState(false);
        ChangeCameraState(true);
    }

    public void ExitBuildingsUI()
    {
        if (!_buildingUI.activeSelf)
            return;

        ChangeUIState(true);
    }

    public void RollDice()
    {
        if (_isRollingDice)
            return;

        if (PlayerManager.Instance.GetResource(ResourceType.DiceRoll) < 1)
            return;

        ChangeUIState(true);
        StartCoroutine(OnRollDice());
    }

    public void RollFinished(DiceRoll die)
    {
        if (!_dice.Contains(die))
        {
            _dice.Add(die);
            _rollResults.Add(die.rollResult);

            if(_dice.Count >= 3)
            {                
                StartCoroutine(DestroyDice());
            }
        }
    }
    #endregion

    #region Private Methods

    private void ChangeUIState(bool useDiceUI)
    {
        _diceUI.SetActive(useDiceUI);
        _upgradeBuildingUI.SetActive(useDiceUI);
        _buildingUI.SetActive(!useDiceUI);
    }

    private void ChangeCameraState(bool useMainCamera)
    {
        _diceRollCamera.gameObject.SetActive(!useMainCamera);
        _mainCamera.gameObject.SetActive(useMainCamera);
    }

    private void CheckReward()
    {
        //if 3 dice roll same value, x2 reward
        //if 2 dice roll same value
        //6 = Free 10 rolls
        //5 = Jackpot
        //4 = Random Card
        //3 = Shield
        //2 = Attack
        //1 = Steal

        //if one no same value
        //100 gold x amount of the dice,
        //example 1,4,5
        //100x1 + 100x4 + 100x5 gold
        //reward = 1000 gold

        int d1 = _rollResults[0];
        int d2 = _rollResults[1];
        int d3 = _rollResults[2];

        var counts = new Dictionary<int, int>();
        foreach (int value in _rollResults)
        {
            if (!counts.ContainsKey(value))
                counts[value] = 0;
            counts[value]++;
        }

        if (counts.ContainsValue(3))
        {
            int face = d1;
            _rewardPreview.ShowReward(GetReward(face), 2);
            return;
        }

        if (counts.ContainsValue(2))
        {
            int face = counts.First(x => x.Value == 2).Key;
            RewardType reward = GetReward(face);
            _rewardPreview.ShowReward(GetReward(face));
            return;
        }

        int gold = 0;
        foreach (int value in _rollResults)
            gold += 100 * value;

        _rewardPreview.ShowReward(RewardType.Gold, gold);        
    }

    private RewardType GetReward(int face)
    {
        switch (face)
        {
            case 6: return RewardType.FreeRolls;
            case 5: return RewardType.Jackpot;
            case 4: return RewardType.Random;
            case 3: return RewardType.Shield;
            case 2: return RewardType.Attack;
            case 1: return RewardType.Steal;
            default: return RewardType.Gold;
        }
    }
    #endregion

    #region Enumerators
    IEnumerator OnRollDice()
    {
        if (!_diceRollCamera.gameObject.activeSelf)
        {
            ChangeCameraState(false);
            yield return new WaitForSeconds(0.6f); //Wait for camera transition so it doesn't look weird
        }

        PlayerManager.Instance.SpendResource(ResourceType.DiceRoll, 1);
        _isRollingDice = true;
        _rollResults.Clear();
        _dice.Clear();
        for (int i = 0; i < 3; i++)
        {
            DiceRoll die = Instantiate(_diePrefab);
            die.transform.position = _diceSpawnPoints[i];
            die.Roll(this);
        }
    }

    IEnumerator DestroyDice()
    {
        yield return new WaitForSeconds(0.5f);
        _dice.ForEach(x => Destroy(x.gameObject));
        CheckReward();
        yield return new WaitForSeconds(1f);
        _isRollingDice = false;

    }
    #endregion

    #region Event Callbacks

    void OnDiceRechargeTimeUpdated(int newValue)
    {
        int minutes = newValue / 60;
        int seconds = newValue % 60;
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        tmp_diceRollTimer.SetText(formattedTime);
    }

    void OnShieldsUpdated()
    {
        int shields = PlayerManager.Instance.GetResource(ResourceType.Shield);

        for (int i = 0; i < _shieldsIcon.Count; i++)
        {
            _shieldsIcon[i].SetActive(i < shields);
        }
    }

    void OnDiceRollUpdated()
    {
        tmp_diceRoll.SetText($"{PlayerManager.Instance.GetResource(ResourceType.DiceRoll)}/100");
    }

    void OnGoldUpdated()
    {
        tmp_gold.SetText(PlayerManager.Instance.GetResource(ResourceType.Gold).ToString("N0"));
    }
    #endregion

}
