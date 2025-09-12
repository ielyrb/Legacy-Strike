using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public DiceRewardsSO diceRewards;
    public int[] goldRewards;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        goldRewards = (int[])diceRewards.goldRewards.Clone();
        ShuffleRewards();
    }

    public void ShuffleRewards()
    {
        for (int i = 0; i < goldRewards.Length; i++)
        {
            int rand = Random.Range(i, goldRewards.Length);
            int temp = goldRewards[i];
            goldRewards[i] = goldRewards[rand];
            goldRewards[rand] = temp;
        }
    }
}
