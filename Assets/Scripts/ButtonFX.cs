using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonFX : MonoBehaviour
{
    private AudioClip _clip;
    private Button _button;

    void Awake()
    {
        _clip = Resources.Load<AudioClip>("Audio/Sound/ButtonFX");
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayFX);
    }

    void PlayFX()
    {
        AudioManager.instance.audioSource.PlayOneShot(_clip);
    }
}
