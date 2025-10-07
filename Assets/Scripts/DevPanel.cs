using TMPro;
using UnityEngine;

public class DevPanel : MonoBehaviour
{
    [SerializeField] bool showPanel = true;

    [SerializeField] GameObject activeCanvas;
    [SerializeField] GameObject inactiveCanvas;

    [SerializeField] TMP_InputField goldInput;
    [SerializeField] TMP_InputField staminaInput;
    [SerializeField] TMP_InputField attackInput;
    [SerializeField] TMP_InputField defenseInput;
    [SerializeField] TMP_InputField critInput;
    [SerializeField] TMP_InputField glancingInput;

    bool isHidden = true;

    private void Awake()
    {
        if (!showPanel)
        {
            activeCanvas.SetActive(false);
            inactiveCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isHidden)
                Show();
            else
                FullHide();
        }
    }

    public void Show()
    {
        activeCanvas.SetActive(true);
        inactiveCanvas.SetActive(false);
        isHidden = false;
    }

    public void Hide()
    {
        //activeCanvas.SetActive(false);
        //inactiveCanvas.SetActive(true);
        FullHide();
    }

    void FullHide()
    {
        activeCanvas.SetActive(false);
        inactiveCanvas.SetActive(false);
        isHidden = true;
    }

    public void TriggerFight()
    {
        LoadingScreenManager.Instance.LoadScene("Battle");
    }

    public void AddGold()
    {
        if (int.Parse(goldInput.text) > 0)
        {
            PlayerManager.Instance.AddResource(ResourceType.Gold, int.Parse(goldInput.text));
        }
        else
        {
            PlayerManager.Instance.SpendResource(ResourceType.Gold, int.Parse(goldInput.text));
        }
    }

    public void AddStamina()
    {
        if (int.Parse(staminaInput.text) > 0)
        {
            PlayerManager.Instance.AddResource(ResourceType.Stamina, int.Parse(staminaInput.text));
        }
        else
        {
            PlayerManager.Instance.SpendResource(ResourceType.Stamina, int.Parse(staminaInput.text));
        }
    }

    public void ModifyAttack()
    {
        PlayerManager.Instance.player.stats.attack = int.Parse(attackInput.text);
        PlayerManager.Instance.ReloadStats();
    }

    public void ModifyDefense()
    {

        PlayerManager.Instance.player.stats.defense = int.Parse(defenseInput.text);
        PlayerManager.Instance.ReloadStats();
    }

    public void ModifyCriticalChance()
    {

        PlayerManager.Instance.player.stats.critChance = int.Parse(critInput.text);
        PlayerManager.Instance.ReloadStats();
    }

    public void ModifyGlancingBlowChance()
    {
        PlayerManager.Instance.glancingBlowChance = int.Parse(glancingInput.text);
        PlayerManager.Instance.ReloadStats();
    }
}
