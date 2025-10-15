using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryViewUI : View
{
    [SerializeField] private InventoryItemUI _prefab;
    [SerializeField] private Transform _container;

    protected override void Start()
    {
        base.Start();
        PlayerManager.OnInventoryUpdated += RefreshInventoryUI;
        RefreshInventoryUI();
    }

    private void OnDestroy()
    {
        PlayerManager.OnInventoryUpdated -= RefreshInventoryUI;
    }

    public void RefreshInventoryUI()
    {
        StartCoroutine(OnRefreshInventory());
    }

    private IEnumerator OnRefreshInventory()
    {
        foreach (Transform child in _container)
        {
            Destroy(child.gameObject);
        }

        yield return null;

        foreach (KeyValuePair<ResourceType, int> kvp in PlayerManager.Instance.player.resources)
        {
            if (kvp.Key == ResourceType.Gold || kvp.Key == ResourceType.Stamina)
                continue;

            InventoryItemUI item = Instantiate(_prefab, _container);
            item.Initialize(kvp.Key, kvp.Value);
        }
    }
}
