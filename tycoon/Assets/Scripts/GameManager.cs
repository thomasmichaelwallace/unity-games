using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text uiMoney;
    public Text uiBalanace;
    public Text uiHint;
    public Water water;
    public Land landPrefab;
    public Transform landParent;
    public CanvasGroup gameOverScreen;
    private readonly float maxHeight = 10.1f; // 10f is last allowable

    private readonly int square = 10;
    private readonly float waterSpeed = 0.1f;
    private readonly int initialBalance = 1000;
    
    private readonly float incomeRate = 0.25f; // rate to update income
    private bool isGameOver;
    private float landHeight = 1f;
    private Land[,] lands;

    private float level = 0.2f;
    private float money;
    private float income;
    private float incomeTimer = 0f;
    private readonly Queue<int> charges = new Queue<int>();
    private float hintTimer = 0;

    private void Awake()
    {
        lands = new Land[square + 1, square + 1];

        var t = square / 2;
        for (var x = -t; x <= t; x += 1)
        for (var z = -t; z <= t; z += 1)
        {
            var position = new Vector3(x, 0, z);
            var land = Instantiate(landPrefab, position, Quaternion.identity, landParent);
            land.Game = this;
            lands[x + t, z + t] = land;
        }
    }

    private void Start()
    {
        income = 0f;
        money = initialBalance;
    }

    private void Update()
    {
        level += waterSpeed * Time.deltaTime;
        water.SetLevel(level);

        var dollar = Mathf.RoundToInt(money);
        uiMoney.text = $"<b>${dollar}</b>";

        incomeTimer += Time.deltaTime;
        if (incomeTimer > incomeRate)
        {
            string lastIncome = income.ToString("0.00");
            income = 0;
            incomeTimer = 0;

            string lastCharges = "";
            for (int i = 0; i < 5; i++)
            {
                if (charges.Count == 0) break;
                int charge = charges.Dequeue();
                lastCharges += $"-${charge} ";
            }
            
            uiBalanace.text = $"<color=#74FF49>+${lastIncome}</color> <color=#FF5149>{lastCharges}</color>";
        }

        uiHint.transform.position = Input.mousePosition;
        if (hintTimer > 0)
        {
            hintTimer -= Time.deltaTime;
            if (hintTimer < 0) uiHint.text = "";
        }

        if (level > landHeight + 1f) isGameOver = true;
        if (isGameOver && gameOverScreen.alpha < 1) gameOverScreen.alpha += 0.5f * Time.deltaTime;
        if (isGameOver && gameOverScreen.alpha >= 1 && Input.GetMouseButton(0)) GameOverClick();
    }
    
    private float GetBuildCost(float height, int type)
    {
        if (height > maxHeight) return 0;
        return (height < level ? 2f : 1f) * 50f * type;
    }

    private float GetChangeCost(float height, int type)
    {
        return 10f * type;
    }

    private float GetEarning(float height, int type, float t)
    {
        if (height < level) return 0;
        return (height - level) * type * t;
    }

    public void Earn(float height, int type, float t)
    {
        if (isGameOver) return;

        if (height < level) return;
        float earn = GetEarning(height, type, t);
        income += earn;
        money += earn;
    }

    private void Charge(float charge)
    {
        int cost = Mathf.RoundToInt(charge);
        charges.Enqueue(cost);
        money -= charge;
    }


    public bool ChargeBuild(float height, int type)
    {
        if (isGameOver) return false;

        if (height > maxHeight) return false;

        var cost = GetBuildCost(height, type);
        if (cost > money) return false;
        Charge(cost);
        landHeight = Mathf.Max(height, landHeight);
        return true;
    }

    public bool ChargeChange(float height, int type)
    {
        if (isGameOver) return false;

        var cost = GetChangeCost(height, type);
        if (cost > money) return false;
        Charge(cost);
        return true;
    }

    public void DisplayCosts(float height, int type)
    {
        hintTimer = incomeRate; // should work.
        string earning = GetEarning(height, type, incomeRate).ToString("0.00");
        string raise = GetBuildCost(height, type).ToString("0.00");
        string change = GetChangeCost(height, type).ToString("0.00");
        uiHint.text = $"\n<b>[earns]</b>: ${earning}\n<b>[raise]</b>: ${raise}\n<b>[cycle]</b>: ${change}";
    }
    
    private void GameOverClick()
    {
        SceneManager.LoadScene(0);
    }
}