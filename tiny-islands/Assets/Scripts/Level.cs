using UnityEngine;

public class Level : MonoBehaviour
{
    private const float TowerY0 = 3.5f;
    private const float IslandY0 = 0.0f;
    private const float BuildingY0 = 1.0f;
    private const float BridgeY0 = 0.8f;

    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private float gridSize = 20f;
    [SerializeField] private int gridSideCount = 10;
    [SerializeField] private float gridNoise = 5f;
    private GameObject[] _prefabs;
    private float[] _y0S;

    private void Start()
    {
        _y0S = new[] {TowerY0, IslandY0, BuildingY0, BridgeY0};
        _prefabs = new[] {towerPrefab, islandPrefab, buildingPrefab, bridgePrefab};
        Generate();
    }

    private void Generate()
    {
        var offset = -(gridSize * gridSideCount / 2);

        for (var xn = 0; xn < gridSideCount; xn++)
        for (var zn = 0; zn < gridSideCount; zn++)
        {
            var i = Random.Range(0, 6);
            if (i > 3) continue;
            
            var x = xn * gridSize + offset + Random.Range(-gridNoise, gridNoise);
            var z = zn * gridSize + offset + Random.Range(-gridNoise, gridNoise);

            var y = _y0S[i];
            var prefab = _prefabs[i];

            var v = new Vector3(x, y, z);
            var d = Random.Range(0, 360f);
            var q = Quaternion.Euler(0, d, 0);

            Instantiate(prefab, v, q, transform);
        }
    }
}