using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class StructureBuildingHandler : MonoBehaviour
{
    [SerializeField] private GameObject _gymPrefab;
    [SerializeField] private GameObject _arenaPrefab;

    [SerializeField] private Transform _gymPosition;
    [SerializeField] private Transform _arenaPosition;

    [SerializeField] private GameObject _buildingFX;

    [SerializeField] private GameObject _gymCamera;
    [SerializeField] private GameObject _arenaCamera;

    private MainSceneHandler _sceneHandler;

    private void Start()
    {
        _sceneHandler = FindObjectOfType<MainSceneHandler>();
    }

    public void InitializeBuilding(BuildingType building, int level)
    {
        GameObject camera = _arenaCamera;
        GameObject prefab = _arenaPrefab;
        Transform prefabPos = _arenaPosition;
        if (building == BuildingType.Gym)
        {
            prefab = _gymPrefab;
            camera = _gymCamera;
            prefabPos = _gymPosition;
        }
        Instantiate(prefab, prefabPos);
    }

    public void UpgradeBuilding(BuildingType building, int level, string message)
    {
        StartCoroutine(CreateStructure(building, level, message));
    }

    IEnumerator CreateStructure(BuildingType building, int level, string message)
    {
        ParticleSystem fx = Instantiate(_buildingFX).GetComponent<ParticleSystem>();
        Transform fxPos = _arenaPosition;
        GameObject camera = _arenaCamera;
        GameObject prefab = _arenaPrefab;
        Transform prefabPos = _arenaPosition;
        if (building == BuildingType.Gym)
        {
            prefab = _gymPrefab;
            fxPos = _gymPosition;
            camera = _gymCamera;
            prefabPos = _gymPosition;
        }

        camera.SetActive(true);
        fx.transform.SetParent(fxPos, false);
        yield return new WaitForSeconds(0.5f);

        fx.Play();
        yield return new WaitForSeconds(1.5f); //Run the particle effect for 1.5 seconds
        NotificationManager.Instance.ShowMessage($"Upgrade completed! Your {building} is now level {level}");
        fx.Stop();
        Destroy(fx.gameObject);

        if (level == 1)
        {
            Instantiate(prefab, prefabPos);
        }

        yield return new WaitForSeconds(0.5f);
        camera.SetActive(false);
        if (building == BuildingType.Gym)
            _sceneHandler.OnStaminaUpdated();
        PlayerManager.Instance.player.buildings[building] = level;
        PlayerManager.Instance.Save();
    }
}
