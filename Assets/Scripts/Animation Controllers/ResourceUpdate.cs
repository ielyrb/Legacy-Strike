using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUpdate : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _amount;

    public void Initialize(Sprite icon, int amount)
    {
        _image.sprite = icon;
        string prefix = amount > 0 ? "+" : "";
        Color color = amount > 0 ? Color.green : Color.red;
        _amount.color = color;
        _amount.SetText($"{prefix}{amount.ToString("N0")}");
        Destroy(gameObject, 1);
    }
}
