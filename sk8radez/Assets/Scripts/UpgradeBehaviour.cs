using UnityEngine;

public class UpgradeBehaviour : MonoBehaviour
{
    private GameBehaviour _game;
    private bool _isGot;

    private void Update()
    {
        var t = transform;
        var p = t.position;

        if (_isGot)
        {
            var s = t.localScale;
            if (s.x < 0) Destroy(this);
            var tick = Time.deltaTime;
            p += new Vector3(tick, -tick);
            ;
            t.localScale = new Vector3(s.x -= tick, s.y -= tick);
        }

        var speed = _game.GetSpeed();
        var next = new Vector3(p.x - speed * Time.deltaTime, p.y);
        t.position = next;

        const float x0 = -(ScreenConfiguration.XCount * ScreenConfiguration.Unit / 2);
        if (next.x < x0) Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        _game.SpeedUp();
        _isGot = true;
    }

    public void SetGame(GameBehaviour game)
    {
        _game = game;
    }
}