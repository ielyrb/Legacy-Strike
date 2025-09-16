using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPreview : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _amount;
    [SerializeField] private GameObject _container;

    [SerializeField] private Sprite _goldImage;
    [SerializeField] private Sprite _stealImage;
    [SerializeField] private Sprite _attackImage;
    [SerializeField] private Sprite _shieldImage;
    [SerializeField] private Sprite _randomImage;
    [SerializeField] private Sprite _jackpotImage;
    [SerializeField] private Sprite _freeRollsImage;

    public void ShowReward(RewardType type, int amount = 1)
    {
        _amount.gameObject.SetActive(amount > 1);
        switch (type)
        {
            case RewardType.Gold:
                _image.sprite = _goldImage;
                PlayerManager.Instance.AddResource(ResourceType.Gold, amount);
                break;

                case RewardType.Steal:
                _image.sprite = _stealImage;
                break;

                case RewardType.Attack:
                _image.sprite = _attackImage;
                break;

                case RewardType.Shield:
                _image.sprite = _shieldImage;
                PlayerManager.Instance.AddResource(ResourceType.Shield, amount);
                break;

                case RewardType.Random:
                _image.sprite = _randomImage;
                break;

                case RewardType.Jackpot:
                _image.sprite = _jackpotImage;
                int rand = Random.Range(5000, 20000);
                PlayerManager.Instance.AddResource(ResourceType.Gold, rand);
                break;

                case RewardType.FreeRolls:
                _image.sprite = _freeRollsImage;
                PlayerManager.Instance.AddResource(ResourceType.DiceRoll, 10); //10 Stamina/Dice Rolls
                break;
        }
        _amount.SetText($"x{amount}");
        StartCoroutine(OnShowReward());
    }

    IEnumerator OnShowReward()
    {
        _container.SetActive(true);
        yield return new WaitForSeconds(1f);
        _container.SetActive(false);
    }
}
public enum RewardType
{
    Gold,
    Steal,
    Attack,
    Shield,
    Random,
    Jackpot,
    FreeRolls
}