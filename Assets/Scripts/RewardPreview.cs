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
    [SerializeField] private Sprite _shieldImage;
    [SerializeField] private Sprite _attackImage;
    [SerializeField] private Sprite _gymImage;
    [SerializeField] private Sprite _glovesImage;
    [SerializeField] private Sprite _arenaImage;

    public void ShowReward(RewardType type, int amount = 1)
    {
        bool loadBattleScene = false;
        _amount.gameObject.SetActive(amount > 1);
        switch (type)
        {
            case RewardType.Gold:
                _image.sprite = _goldImage;
                PlayerManager.Instance.AddResource(ResourceType.Gold, amount);
                break;

                case RewardType.Shield:
                _image.sprite = _shieldImage;
                PlayerManager.Instance.AddResource(ResourceType.Shield, amount);
                break;

                case RewardType.Attack:
                _image.sprite = _attackImage;
                loadBattleScene = true;
                break;

                case RewardType.Gym:
                _image.sprite = _gymImage;                
                break;

                case RewardType.Gloves:
                PlayerManager.Instance.AddResource(ResourceType.Stamina, amount * 5);
                _image.sprite = _glovesImage;
                break;

                case RewardType.Arena:
                _image.sprite = _arenaImage;
                break;
        }
        _amount.SetText($"x{amount}");
        StartCoroutine(OnShowReward(loadBattleScene));
    }

    IEnumerator OnShowReward(bool loadBattleScene = false)
    {
        _container.SetActive(true);
        yield return new WaitForSeconds(1f);
        _container.SetActive(false);
        if (loadBattleScene)
            LoadingScreenManager.Instance.LoadScene("Battle");
    }
}

public enum RewardType
{
    Gold,
    Gloves,
    Attack,
    Shield,
    Gym,
    Arena,
}