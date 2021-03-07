using System;
using System.Linq;
using UnityEngine;

public class BoardBehaviour : MonoBehaviour
{
    public enum TileResponse
    {
        Support,
        Fall,
        CollectBook,
        EmptyAndBlock,
        WinAndReset
    }

    public const float TileSize = 2f;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject shelfPrefab;
    private BookBehaviour[,] _books;
    private int _booksHeld, _booksCollected, _bookTotal;
    private Camera _camera;
    private ShelfBehaviour[,] _shelves;
    private int[] _start = {0, 0};
    private TileBehaviour[,] _tiles;

    private void Awake()
    {
        _camera = Camera.main;
        Setup();
    }

    private void FitSceneToCamera()
    {
        // width and height with additional border
        var width = TileSize * (2 + _tiles.GetLength(1));
        var height = TileSize * (2 + _tiles.GetLength(0));

        var rads = _camera.transform.rotation.eulerAngles.x * Mathf.Deg2Rad;
        var screenHeight = Mathf.Sin(rads) * height;

        _camera.orthographicSize = Mathf.Max(width / (2f * _camera.aspect), screenHeight / 2f);

        var position = _camera.transform.position;

        // 0,0 is at tile (1.5, 1.5)
        var centerX = width / 2f - 1.5f * TileSize;
        var centerZ = height / 2f - position.y / Mathf.Tan(rads) - 1.5f * TileSize;

        position.x = centerX; // allow border
        position.z = centerZ;
        _camera.transform.position = position;
    }

    /// <summary>
    ///     Gets player starting position [row, column].
    ///     Only accurate after Awake()
    /// </summary>
    public int[] GetStartingPosition()
    {
        return new[]
        {
            _start[0], _start[1]
        };
    }

    private void Setup()
    {
        var layoutText = LevelManager.Levels[LevelManager.LevelNo - 1]; // allow level no. to be more natural
        var rows = layoutText.Split('\n');

        _start = new[] // first line of map is starting position
        {
            int.Parse(rows[0].Split(',')[0]),
            int.Parse(rows[0].Split(',')[1])
        };

        rows = rows.Reverse().ToArray(); // reverse so that grid matches visual

        var height = rows.Length - 2; // ignore top and bottom padding rows
        var width = 0;
        for (var r = 1; r < rows.Length - 1; r++) width = Math.Max(width, rows[r].Length);
        _tiles = new TileBehaviour[height, width];
        _books = new BookBehaviour[height, width];
        _shelves = new ShelfBehaviour[height, width];

        for (var r = 0; r < height; r++)
        {
            var columns = rows[r + 1]; // offset row to avoid padding

            for (var c = 0; c < columns.Length; c++) // row data is variable length
            {
                var str = columns[c];
                // [ ] empty space 
                if (str == ' ') continue;

                var center = new Vector3(TileSize * c, 0, TileSize * r);

                if (str == '*')
                {
                    // [*] book + invincible
                    var book = Instantiate(bookPrefab, center, Quaternion.identity, transform);
                    var bookBehaviour = book.GetComponent<BookBehaviour>();
                    _books[r, c] = bookBehaviour;
                    _bookTotal += 1;
                    str = '0';
                }
                else if (str == '#')
                {
                    // [#] shelf + invincible
                    var shelf = Instantiate(shelfPrefab, center, Quaternion.identity, transform);
                    var shelfBehaviour = shelf.GetComponent<ShelfBehaviour>();
                    _shelves[r, c] = shelfBehaviour;
                    str = '0';
                }

                // [9] number of steps
                // [0] invincible
                var hasSteps = int.TryParse(str.ToString(), out var steps);

                if (!hasSteps) continue; // fail gracefully
                if (steps == 0) steps -= 1; // internally invincible is actually -1

                var tile = Instantiate(tilePrefab, center, Quaternion.identity, transform);
                var tileBehaviour = tile.GetComponent<TileBehaviour>();
                tileBehaviour.SetSteps(steps);

                _tiles[r, c] = tileBehaviour;
            }
        }

        FitSceneToCamera();
    }

    public TileResponse StepOn(int row, int column, int weight)
    {
        // always fall if out of bounds
        if (row < 0 || column < 0) return TileResponse.Fall;
        if (row >= _tiles.GetLength(0) || column >= _tiles.GetLength(1)) return TileResponse.Fall;

        var tile = _tiles[row, column];
        // fall if no tile
        if (tile is null) return TileResponse.Fall;
        // fall if weight breaks tile on landing
        if (!tile.StepOn(weight))
        {
            _tiles[row, column] = null;
            return TileResponse.Fall;
        }

        var book = _books[row, column];
        if (book)
        {
            _booksCollected += 1;
            _booksHeld += 1;
            book.Collect();
            _books[row, column] = null;
            return TileResponse.CollectBook;
        }

        var shelf = _shelves[row, column];
        if (!shelf) return TileResponse.Support;

        if (_booksHeld > 0)
        {
            shelf.AddBooks(_booksHeld);
            _booksHeld = 0;
        }

        return _booksCollected < _bookTotal ? TileResponse.EmptyAndBlock : TileResponse.WinAndReset;
    }

    public void StepOff(int row, int column, int weight)
    {
        var tile = _tiles[row, column];
        if (tile is null) return; // this shouldn't happen

        var remains = tile.StepOff(weight);
        if (!remains) _tiles[row, column] = null;
    }
}