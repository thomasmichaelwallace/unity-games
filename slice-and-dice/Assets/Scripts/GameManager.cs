using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public long Score { get; private set; }
    public float Power { get; set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            EndGame();
        }
        else
        {
            if (this != _instance)
                Destroy(gameObject);
        }
    }

    public long SetScore(string points)
    {
        if (points == "") return Score;
        Score += long.Parse(points);
        return Score;
    }

    public void EndGame()
    {
        Score = 0;
        Power = 0.9f;
    }
}