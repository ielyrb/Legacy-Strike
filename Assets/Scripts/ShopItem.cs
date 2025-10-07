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
    [SerializeField] private Sprite _goldIcon;
    [SerializeField] private Sprite _staminaIcon;

    private void Awake()
    {
        button?.onClick.AddListener(Buy);
    }

    private void Start()
    {
        _image.sprite = type == ResourceType.Gold ? _goldIcon : _staminaIcon;
        _txtAmount.SetText($"x{amount}");
    }

    public void Buy()
    {
        PlayerManager.Instance.AddResource(type, amount);
    }

}
