using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text uiMoney;
    public Water water;
    public Land landPrefab;
    public Transform landParent;
    public CanvasGroup gameOverScreen;

    private readonly int square = 10;
    private readonly float waterSpeed = 0.1f;
    private readonly float maxHeight = 10.1f; // 10f is last allowable
    
    private float level = 0.2f;
    private float money = 1000f;
    private Land[,] lands;
    private float landHeight = 1f;
    private bool isGameOver = false;

    private void Awake()
    {
        lands = new Land[square + 1, square + 1];

        int t = square / 2;
        for (int x = -t; x <= t; x += 1)
        {
            for (int z = -t; z <= t; z += 1)
            {
                Vector3 position = new Vector3(x, 0, z);
                Land land = Instantiate(landPrefab, position, Quaternion.identity, landParent);
                land.Game = this;
                lands[x + t, z + t] = land;
            }
        }
    }

    public void Earn(float height, int type, float t)
    {
        if (isGameOver) return;
        
        if (height < level) return;
        money += (height - level) * (float) type * t;
    }

    public bool ChargeBuild(float height, int type)
    {
        if (isGameOver) return false;
        
        if (height > maxHeight) return false;
        
        var cost = (height < level ? 2f : 1f) * 50f * type;
        if (cost > money) return false;
        money -= cost;
        landHeight = Mathf.Max(height, landHeight);
        return true;
    }
    
    public bool ChargeChange(float height, int type)
    {
        if (isGameOver) return false;
        
        var cost = 10f * type;
        if (cost > money) return false;
        money -= cost;
        return true;        
    }

    void Update()
    {
        level += waterSpeed * Time.deltaTime;
        water.SetLevel(level);
        
        int dollar = Mathf.RoundToInt(money);
        uiMoney.text = $"<b>${dollar}</b>";

        if (level > (landHeight + 1f)) isGameOver = true;
        if (isGameOver && gameOverScreen.alpha < 1) gameOverScreen.alpha += (0.5f * Time.deltaTime);
        if (isGameOver && gameOverScreen.alpha >= 1 && Input.GetMouseButton(0)) GameOverClick();
    }

    void GameOverClick()
    {
        SceneManager.LoadScene(0);
    }
}
