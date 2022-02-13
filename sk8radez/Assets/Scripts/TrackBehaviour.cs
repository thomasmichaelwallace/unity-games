using UnityEngine;

public class TrackBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite railSprite;
    [SerializeField] private Sprite railEndSprite;
    [SerializeField] private Sprite railStartSprite;
    [SerializeField] private Sprite railShortSprite;
    [SerializeField] private LevelBehaviour level;
    [SerializeField] private GameBehaviour game;
    private SpriteRenderer[,] _renderers;

    private void Start()
    {
        _renderers = new SpriteRenderer[ScreenConfiguration.YCount, ScreenConfiguration.XCount];

        // TODO: build array
        const float x0 = -(ScreenConfiguration.XCount * ScreenConfiguration.Unit) / 2;
        var y = ScreenConfiguration.TrackTopY;
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        {
            var x = x0 + ScreenConfiguration.XOffset * r;

            for (var c = 0; c < ScreenConfiguration.XCount; c += 1)
            {
                var p = new Vector3(x, y, 0);
                var tile = Instantiate(tilePrefab, p, Quaternion.identity, transform);
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();

                var lhs = c == 0 ? LevelBehaviour.Tracks.Empty : level.GetAt(r, c - 1);
                var track = level.GetAt(r, c);
                var rhs = c == ScreenConfiguration.XCount - 1 ? track : level.GetAt(r, c + 1);
                spriteRenderer.sprite = TrackToSprite(lhs, track, rhs);

                _renderers[r, c] = spriteRenderer;
                x += ScreenConfiguration.Unit;
            }

            y -= ScreenConfiguration.TrackVerticalDistance;
        }
    }

    private void Update()
    {
        var t = transform;
        var p = t.position;
        p.x -= Time.deltaTime * game.GetSpeed();

        if (p.x <= -ScreenConfiguration.Unit)
        {
            p.x = 0;
            Shift();
        }

        t.position = p;
    }

    private Sprite TrackToSprite(LevelBehaviour.Tracks lhs, LevelBehaviour.Tracks track, LevelBehaviour.Tracks rhs)
    {
        return track switch
        {
            LevelBehaviour.Tracks.Rail when lhs == LevelBehaviour.Tracks.Empty && rhs == LevelBehaviour.Tracks.Empty =>
                railShortSprite,
            LevelBehaviour.Tracks.Rail when lhs == LevelBehaviour.Tracks.Empty => railStartSprite,
            LevelBehaviour.Tracks.Rail when rhs == LevelBehaviour.Tracks.Empty => railEndSprite,
            _ => track == LevelBehaviour.Tracks.Rail ? railSprite : null
        };
    }

    private void Shift()
    {
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        for (var c = 0; c < ScreenConfiguration.XCount - 2; c += 1)
        {
            // shift sprites for all but last two (as rhs was unknown when first rendered)
            var spriteRenderer = _renderers[r, c];
            spriteRenderer.sprite = _renderers[r, c + 1].sprite;
        }

        var next = level.GetNext();
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        {
            // update second to-last now rhs is known
            const int c = ScreenConfiguration.XCount - 2;
            var lhs = level.GetAt(r, c - 1);
            var track = level.GetAt(r, c);
            var rhs = next[r];
            var sprite = TrackToSprite(lhs, track, rhs);
            _renderers[r, c].sprite = sprite;
        }

        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        {
            // guess last row with rhs == track
            const int c = ScreenConfiguration.XCount - 1;
            var lhs = level.GetAt(r, c - 1);
            var track = next[r];
            var sprite = TrackToSprite(lhs, track, track);
            _renderers[r, c].sprite = sprite;
        }
    }

    public LevelBehaviour.Tracks GetAtPosition(Vector3 p)
    {
        var r = -Mathf.FloorToInt((p.y - ScreenConfiguration.TrackTopY) / ScreenConfiguration.TrackVerticalDistance);

        var x0 = -(ScreenConfiguration.XCount * ScreenConfiguration.Unit) / 2; // edge of tile map
        x0 += ScreenConfiguration.XOffset * r; // parallax effect
        x0 += transform.position.x; // track slide

        var c = Mathf.FloorToInt((p.x - x0) / ScreenConfiguration.Unit);
        var t = level.GetAt(r, c);

        return t;
    }
}