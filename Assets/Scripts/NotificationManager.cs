using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _txt;

    private Queue<string> _messageQueue = new Queue<string>();
    private bool _isShowing = false;

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

    public void ShowMessage(string message)
    {
        _messageQueue.Enqueue(message);

        if (!_isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;

        while (_messageQueue.Count > 0)
        {
            string msg = _messageQueue.Dequeue();
            _txt.text = msg;
            _canvas.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);

            _canvas.gameObject.SetActive(false);
        }

        _isShowing = false;
    }
}
