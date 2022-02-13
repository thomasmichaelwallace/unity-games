using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite[] numbers;
    [SerializeField] private SpriteRenderer[] renderers;

    private void Start()
    {
        SetScore(0);
    }

    public void SetScore(int score)
    {
        var exp = 1;
        for (var n = 0; n < renderers.Length; n += 1)
        {
            var r = renderers[n];
            if (score < exp)
            {
                r.sprite = n == 0 ? numbers[0] : null;
            }
            else
            {
                var index = score / exp;
                index %= 10;
                r.sprite = numbers[index];
            }
            exp *= 10;
        }

        if (score >= exp)
        {
            // max!
            foreach (var r in renderers) r.sprite = numbers[10];
        }
    }
}