using TMPro;
using UnityEngine;

public class GoldUpdateAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI value;

    public void Init(bool add, int amount)
    {
        if (add)
            value.color = Color.green;
        else
            value.color = Color.red;

        value.text = add ? $"+{amount.ToString("N0")}" : $"-{amount.ToString("N0")}";
        Destroy(gameObject, 1.5f);
    }
}
