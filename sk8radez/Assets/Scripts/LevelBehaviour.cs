using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    public enum Tracks
    {
        Rail,
        Empty
    }

    private const float RailChance = 0.6f;
    [SerializeField] private GameBehaviour game;
    private int _distance;

    private Tracks[,] _level;

    private void Awake()
    {
        _level = new Tracks[ScreenConfiguration.YCount, ScreenConfiguration.XCount];

        // init level as one long rail

        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        for (var c = 0; c < ScreenConfiguration.XCount; c += 1)
            _level[r, c] = r == 1 ? c < ScreenConfiguration.EmptyXCount ? Tracks.Empty : Tracks.Rail : Tracks.Empty;
    }

    public Tracks[] GetNext()
    {
        var next = new Tracks[ScreenConfiguration.YCount];
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
            next[r] = Random.value < RailChance ? Tracks.Rail : Tracks.Empty; // TODO: actual track

        // shift
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1)
        for (var c = 0; c < ScreenConfiguration.XCount - 1; c += 1)
            _level[r, c] = _level[r, c + 1];
        for (var r = 0; r < ScreenConfiguration.YCount; r += 1) _level[r, ScreenConfiguration.XCount - 1] = next[r];

        _distance += 1;
        game.SetDistance(_distance);

        return next;
    }

    public Tracks GetAt(int row, int column)
    {
        return _level[row, column];
    }
}