using UnityEngine;

public class Arena : Building
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Interact()
    {
        base.Interact();
    }

    private void OnMouseDown()
    {
        Debug.Log("Arena Clicked");
    }
}
