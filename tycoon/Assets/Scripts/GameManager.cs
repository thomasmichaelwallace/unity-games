using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text uiMoney;
    public Water water;
    public Land landPrefab;
    public Transform landParent;

    private readonly int square = 10;
    private readonly float waterSpeed = 0.1f;

    private float level = 0.2f;
    private float money = 1000f;
    private Land[,] lands;

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
        if (height < level) return;
        money += (height - level) * (float) type * t;
    }

    public bool ChargeBuild(float height, int type)
    {
        var cost = (height < level ? 2f : 1f) * 50f * type;
        if (cost > money) return false;
        money -= cost;
        return true;
    }

    public bool ChargeChange(float height, int type)
    {
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
    }
}
