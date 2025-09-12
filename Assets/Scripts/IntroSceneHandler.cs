using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneHandler : MonoBehaviour
{
    public bool isLoggedIn;
    public string testUsername = "test123";

    [SerializeField] private GameObject loginGO;
    [SerializeField] private GameObject logoutGO;
    [SerializeField] private GameObject userContainerGO;
    [SerializeField] private TextMeshProUGUI tmp_user;

    [SerializeField] private Button startGameBtn;

    private void Start()
    {
        UpdateButtonsUI();
    }

    public void StartGame()
    {
        if (!isLoggedIn)
            return;

        startGameBtn.interactable = false;
        LoadingScreenManager.Instance.LoadScene("Locker");
    }

    void UpdateButtonsUI()
    {
        loginGO.SetActive(!isLoggedIn);
        logoutGO.SetActive(isLoggedIn);
        userContainerGO.SetActive(isLoggedIn);
        tmp_user.SetText($"Welcome {testUsername}");        
    }

    public void Login()
    {
        isLoggedIn = true;
        UpdateButtonsUI();
    }

    public void Logout()
    {
        isLoggedIn = false;
        UpdateButtonsUI();
    }
}
