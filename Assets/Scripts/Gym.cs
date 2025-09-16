using UnityEngine;

public class Gym : Building
{
    private string _buildingName;
    public int buildingLevel { get; private set; }
    public int upgradeCost { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
    }

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
        Debug.Log("Gym Clicked");
    }
}
