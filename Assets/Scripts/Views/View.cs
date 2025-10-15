using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    public Button closeButton;

    private static List<View> allViews = new List<View>();
    [SerializeField] private List<GameObject> myViews;

    #region Unity Delgates

    protected virtual void Awake()
    {
        allViews.Add(this);
    }
    protected virtual void Start()
    {
        closeButton?.onClick.AddListener(OnClosePressed);
    }
    private void OnDestroy()
    {
        allViews.Remove(this);
    }
    #endregion

    public virtual void ShowView()
    {
        foreach (GameObject view in myViews)
            view.SetActive(true);
    }
    public virtual void HideView()
    {
        foreach (GameObject view in myViews)
            view.SetActive(false);
    }

    public static T GetView<T>() where T : View
    {
        View view = allViews.Find(r => r.GetType() == (typeof(T)));

        if (view != null)
        {
            return (T)view;
        }
        else
        {
            return null;
        }
    }

    private void OnClosePressed()
    {
        HideView();
    }
}
