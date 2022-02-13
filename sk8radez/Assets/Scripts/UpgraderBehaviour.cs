using UnityEngine;

public class UpgraderBehaviour : MonoBehaviour
{
    private const float MaxUpgradeTime = 10f;
    private const float MinUpgradeTime = 5f;
    [SerializeField] private GameObject upgraderPrefab;
    [SerializeField] private GameBehaviour game;
    private float _nextUpgradeIn = MinUpgradeTime;

    private void Update()
    {
        _nextUpgradeIn -= Time.deltaTime;
        if (_nextUpgradeIn > 0) return;

        _nextUpgradeIn = Random.Range(MinUpgradeTime, MaxUpgradeTime);
        var row = Random.Range(0, ScreenConfiguration.YCount);
        var y = ScreenConfiguration.TrackTopY + (1f * ScreenConfiguration.Unit) -
                (row * ScreenConfiguration.TrackVerticalDistance);
        const float x = ScreenConfiguration.Unit * ScreenConfiguration.XCount / 2; // off screen
        var p = new Vector3(x, y);
        var g = Instantiate(upgraderPrefab, p, Quaternion.identity, transform);
        g.GetComponent<UpgradeBehaviour>().SetGame(game);
    }
}