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
    [SerializeField] private GameObject _statsUI;

    [SerializeField] private GoldUpdateAnimation _goldUpdatePrefab;
    [SerializeField] private Transform _goldUpdateParent;

    [SerializeField] private StructureBuildingHandler _buildingHandler;

    private List<DiceRoll> _dice = new List<DiceRoll>();
    private List<int> _rollResults = new List<int>();

    private bool _isRollingDice;

    private void Awake()
    {
        PlayerManager.OnStaminaRechargeTimeUpdated += OnStaminaRechargeTimeUpdated;
        PlayerManager.OnStaminaUpdated += OnStaminaUpdated;
        PlayerManager.OnGoldUpdated += OnGoldUpdated;
        ChangeCameraState(true);
    }
    private void Start()
    {
        PlayerManager.Instance.StartResourceRecharge();
        OnGoldUpdated(true, 0);
        OnStaminaUpdated();
        SetupBuildings();
    }

    #region Public Methods

    public void SetStatsUIState(bool value)
    {
        _statsUI.SetActive(value);
    }

    public void ShowInventory()
    {
        View.GetView<InventoryView>().ShowView();
    }

    public void ShowShop()
    {
        View.GetView<ShopViewUI>().ShowView();
    }

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

        if (PlayerManager.Instance.GetResource(ResourceType.Stamina) < 1)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartRandomFight();
    }

    private void UpdateGoldAmount(bool add, int amount)
    {
        if (amount == 0)
            return;

        GoldUpdateAnimation prefab = Instantiate(_goldUpdatePrefab, _goldUpdateParent);
        prefab.Init(add, amount);
    }

    private void StartRandomFight()
    {
        LoadingScreenManager.Instance.LoadScene("Battle");
    }

    private void SetupBuildings()
    {
        foreach (KeyValuePair<BuildingType, int> kvp in PlayerManager.Instance.player.buildings)
        {
            if (kvp.Value > 0)
            {
                _buildingHandler.InitializeBuilding(kvp.Key, kvp.Value);
            }
        }
    }

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
        //if one no matches
        //100 gold x total value of the dice

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

            int amount = 2;
            RewardType reward = GetReward(face);

            if (reward == RewardType.Gold)
                amount = Random.Range(2500, 10000);

            _rewardPreview.ShowReward(reward, amount);
            return;
        }

        if (counts.ContainsValue(2))
        {
            int face = counts.First(x => x.Value == 2).Key;
            int amount = 1;
            RewardType reward = GetReward(face);

            if(reward == RewardType.Gold)
                amount = Random.Range(5000, 20000);

            _rewardPreview.ShowReward(reward, amount);
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
            case 6: return RewardType.Attack;
            case 5: return RewardType.Arena;
            case 4: return RewardType.Shield;
            case 3: return RewardType.Gym;
            case 2: return RewardType.Gloves;
            case 1: return RewardType.Gold;
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

        PlayerManager.Instance.SpendResource(ResourceType.Stamina, 1);
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

    void OnStaminaRechargeTimeUpdated(int newValue)
    {
        int minutes = newValue / 60;
        int seconds = newValue % 60;
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        tmp_diceRollTimer.SetText(formattedTime);
    }

    public void OnStaminaUpdated()
    {
        tmp_diceRoll.SetText($"{PlayerManager.Instance.GetResource(ResourceType.Stamina)}/{PlayerManager.Instance.maxStamina}");
    }

    void OnGoldUpdated(bool add, int amount)
    {
        tmp_gold.SetText(PlayerManager.Instance.GetResource(ResourceType.Gold).ToString("N0"));
        //UpdateGoldAmount(add, amount);
    }
    #endregion

}
