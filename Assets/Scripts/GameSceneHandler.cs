using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameSceneHandler : MonoBehaviour
{
    public static Action<int> OnDiceRolled;

    [SerializeField] private TextMeshProUGUI _centerText;
    [SerializeField] private DiceRewardItem[] _rewards;

    private void Start()
    {
        OnDiceRolled += DiceRolled;
        UpdateRewards();
    }

    void DiceRolled(int value)
    {
        StartCoroutine(ShowText(value));
    }

    void UpdateRewards()
    {        
        int[] rewards = GameManager.Instance.goldRewards;

        for (int i = 0; i < rewards.Length; i++)
            _rewards[i].SetRewardAmount(rewards[i]);
    }

    IEnumerator ShowText(int value)
    {
        int index = value - 1;
        int reward = _rewards[index].amount;

        _rewards[index].SetHighlight(true);

        _centerText.transform.parent.gameObject.SetActive(true);
        _centerText.SetText("You rolled a {0} and got {1} gold!", value, reward);

        yield return new WaitForSeconds(1f);

        _rewards[index].SetHighlight(false);
        _centerText.transform.parent.gameObject.SetActive(false);

        GameManager.Instance.ShuffleRewards();
        UpdateRewards();
    }

}
