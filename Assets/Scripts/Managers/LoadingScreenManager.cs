using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField] private TextMeshProUGUI tmp_progress;
    [SerializeField] private Slider _slider;
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private TextMeshProUGUI tmp_loadingText;
    [SerializeField] private Animator _ani;

    [Header("Timings")]
    [SerializeField] private float _loadingDuration;
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;

    private AsyncOperation asyncOperation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadTargetSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            _slider.value = progress;
            tmp_progress.text = $"Loading: {Mathf.RoundToInt(progress * 100)}%";
            yield return null;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(OnShowLoadingScreen(sceneName));
    }

    private IEnumerator OnShowLoadingScreen(string sceneName)
    {
        float elapsed = 0f;
        _ani.enabled = true;
        _slider.value = 0f;
        tmp_progress.text = "0%";

        //Fade In
        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _fadeInDuration);
            _canvas.alpha = 0 + t;

            yield return null;
        }

        elapsed = 0;
        _canvas.alpha = 1;
        
        while (elapsed < _loadingDuration)
        {
            elapsed += Time.deltaTime;
        
            float progress = Mathf.Clamp01(elapsed / _loadingDuration);
        
            _slider.value = progress;
            tmp_progress.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        
            yield return null;
        }
                
        _slider.value = 1f;
        tmp_progress.text = "100%";
        _ani.enabled = false;
        Color32 color = new Color32(255, 255, 255, 255);
        tmp_loadingText.color = color;
        tmp_loadingText.text = "Loading Completed!";
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);

        elapsed = 0f;
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
        
            float t = Mathf.Clamp01(elapsed / _fadeOutDuration);
            _canvas.alpha = 1f - t;
        
            yield return null;
        }
        _canvas.alpha = 0f;        
    }
}
