using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _txt;
    [SerializeField] private List<Sprite> icons;

    [SerializeField] private ResourceUpdate _resourceUpdatePrefab;

    private Queue<string> _messageQueue = new Queue<string>();
    private Dictionary<string, Sprite> spritesCollection = new Dictionary<string, Sprite>();
    private bool _isMessageShowing = false;

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
        icons.ForEach(x => spritesCollection.Add(x.name, x));
    }

    public void ShowResourceUpdate(ResourceType type, int amount)
    {
        if (amount == 0)
            return;

        string key = type.ToString().Replace("Token","").Trim();
        Sprite icon = spritesCollection.FirstOrDefault(x => x.Key == key).Value;
        ResourceUpdate go = Instantiate(_resourceUpdatePrefab, _canvas.transform.parent);
        go.Initialize(icon, amount);
    }

    public void ShowMessage(string message)
    {
        _messageQueue.Enqueue(message);

        if (!_isMessageShowing)
            StartCoroutine(ProcessMessageQueue());
    }

    private IEnumerator ProcessMessageQueue()
    {
        _isMessageShowing = true;

        while (_messageQueue.Count > 0)
        {
            string msg = _messageQueue.Dequeue();
            _txt.text = msg;
            _canvas.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);

            _canvas.gameObject.SetActive(false);
        }

        _isMessageShowing = false;
    }
}
