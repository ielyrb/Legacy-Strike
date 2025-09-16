using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRewardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp_reward;
    [SerializeField] private Outline _outline;
    [SerializeField] private Image _icon;

    public DiceReward reward { get; private set; }

    public void SetReward(DiceReward newReward)
    {
        reward = newReward;
        tmp_reward.SetText($"{reward.amount}");
        _icon.sprite = newReward.icon;
    }

    public void SetHighlight(bool value) => _outline.enabled = value;
}
