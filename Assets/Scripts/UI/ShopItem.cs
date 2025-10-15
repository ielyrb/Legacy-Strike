using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ResourceType type;
    [SerializeField] int amount;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI _txtAmount;

    [SerializeField] private Image _image;
    [SerializeField] private Sprite _gymIcon;
    [SerializeField] private Sprite _goldIcon;
    [SerializeField] private Sprite _arenaIcon;
    [SerializeField] private Sprite _attackIcon;
    [SerializeField] private Sprite _shieldIcon;
    [SerializeField] private Sprite _staminaIcon;

    private void Awake()
    {
        button?.onClick.AddListener(Buy);
    }

    private void Start()
    {
        _image.sprite = 
            type == ResourceType.GymToken ? _gymIcon :
            type == ResourceType.Gold ? _goldIcon : 
            type == ResourceType.ArenaToken ? _arenaIcon :
            type == ResourceType.AttackToken ? _attackIcon :
            type == ResourceType.ShieldToken ? _shieldIcon :
            _staminaIcon;
        _txtAmount.SetText($"x{amount}");
    }

    public void Buy()
    {
        PlayerManager.Instance.AddResource(type, amount);
    }

}
