using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text_amount;
    [SerializeField] private Sprite shield;
    [SerializeField] private Sprite attack;
    [SerializeField] private Sprite gym;
    [SerializeField] private Sprite arena;

    public void Initialize(ResourceType resource,int amount)
    {
        Sprite icon = null;
        switch(resource)
        {
            case ResourceType.AttackToken:
                icon = attack;
                break;

                case ResourceType.GymToken:
                icon = gym;
                break;

                case ResourceType.ArenaToken:
                icon = arena;
                break;

            case ResourceType.ShieldToken:
                icon = shield;
                break;
        }
        image.sprite = icon;
        text_amount.text = amount.ToString("N0");
    }
}
