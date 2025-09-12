using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRewardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp_reward;
    [SerializeField] private Outline _outline;
    public int amount { get; private set; }

    public void SetRewardAmount(int value)
    {
        tmp_reward.SetText($"{value}");
        amount = value;
    }

    public void SetHighlight(bool value) => _outline.enabled = value;
}
