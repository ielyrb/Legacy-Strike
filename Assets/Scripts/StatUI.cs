using TMPro;
using UnityEngine;

public class StatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statValue;

    [SerializeField] private StatType _type;

    private void Start()
    {
        PlayerManager.OnStatsUpdated += OnStatsUpdated;
        OnStatsUpdated();
    }

    void OnStatsUpdated()
    {
        switch(_type)
        {
            case StatType.Attack:
                _statValue.SetText(PlayerManager.Instance.player.stats.attack.ToString("N0"));
                break;

            case StatType.Defense:
                _statValue.SetText(PlayerManager.Instance.player.stats.defense.ToString("N0"));
                break;

            case StatType.Critchance:
                _statValue.SetText($"{(PlayerManager.Instance.player.stats.critChance).ToString("N0")}%");
                break;
        }
    }
}
