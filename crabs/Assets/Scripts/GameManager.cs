using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float PlayerHealth = 100f;

    [SerializeField]
    private RectTransform HealthBar = null;

    [SerializeField]
    private CanvasGroup DamageOverlay = null;

    [SerializeField]
    private CanvasGroup GameOverScreen = null;

    [SerializeField]
    private TextMeshProUGUI GameOverText = null;

    private readonly float damageShowTime = 1f;

    private float health;
    private Vector2 barSize;
    private float damageTimer = 0;
    private bool isGameOver;

    private void Start()
    {
        health = PlayerHealth;
        barSize = HealthBar.sizeDelta;
    }

    private void Update()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
            DamageOverlay.alpha = Mathf.Lerp(0, 1, damageTimer / damageShowTime);
        }
    }

    public void TakeDamage(float strength)
    {
        health -= strength * Time.deltaTime;

        float barWidth = Mathf.Lerp(0, barSize.x, health / PlayerHealth);
        HealthBar.sizeDelta = new Vector2(barWidth, barSize.y);

        damageTimer = damageShowTime;

        if (health <= 0) EndGame();
    }

    public void EndGame()
    {
        isGameOver = true;
        SpawnController spawnController = FindObjectOfType<SpawnController>();
        int killCount = spawnController.Kills;
        GameOverText.text = GameOverText.text.Replace("{{kills}}", killCount.ToString());
        GameOverScreen.alpha = 1;
    }

    public void RestartGame()
    {
        if (isGameOver) SceneManager.LoadScene(0);
    }
}