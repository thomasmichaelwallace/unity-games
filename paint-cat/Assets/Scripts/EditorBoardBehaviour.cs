using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorBoardBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;

    public List<EditorTileBehaviour> Tiles { get; } = new();
    public int BoardWidth { get; private set; } = 1;
    public int BoardHeight { get; private set; } = 1;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        SetBoardCode("FN7VVJ39WSAVZHK22O0M");
    }

    private void SetupBoard(int height, IReadOnlyList<int> colours)
    {
        BoardHeight = height;
        BoardWidth = colours.Count / height;
        
        // clear old tiles.
        foreach (var tile in Tiles)
        {
            Destroy(tile.gameObject);        
        }
        Tiles.Clear();

        // update tiles
        for (var r = 0; r < BoardHeight; r++)
        {
            for (var c = 0; c < BoardWidth; c++)
            {
                var prefab = Instantiate(tilePrefab, new Vector3(c, -r, 0), Quaternion.identity, transform);
                var tile = prefab.GetComponent<EditorTileBehaviour>();
                var i = r * BoardWidth + c;
                if (i < colours.Count)
                {
                    tile.SilentSetColourIndex(colours[i]);
                }
                Tiles.Add(tile);
            }
        }
        
        // refocus camera
        var x = (BoardWidth / 2f) - 0.5f;
        var y = -((BoardHeight / 2f) - 0.5f);
        _camera.transform.position = new Vector3(x, y, -10f);

        var aspect = _camera.aspect;
        var verticalSize = (float)BoardHeight * 0.5f;
        var horizontalSize = (BoardWidth / aspect) * 0.5f;
        float size;
        const float border = 0.2f;
        if (verticalSize > horizontalSize)
        {
            size = verticalSize;
            size += border * size;
        }
        else
        {
            size = horizontalSize;
            size += ((border * size) / aspect);
        }
        // Debug.Log($"do_size {horizontalSize}h / ${verticalSize}v -> ${size} + ${border}");
        _camera.orthographicSize = size + border;

        // reset code
        EditorManager.Manager.UpdateLevelCode();
    }

    private void ResizeBoard(int height, int width)
    {
        var colours = new List<int>();
        for (var r = 0; r < height; r++)
        {
            for (var c = 0; c < width; c++)
            {
                if (r < BoardHeight && c < BoardWidth)
                {
                    // preserve colours
                    var i = r * BoardWidth + c;
                    if (i < Tiles.Count)
                    {
                        var tile = Tiles[i];
                        colours.Add(tile.ColourIndex);                        
                    }
                    else
                    {
                        colours.Add(0);   
                    }
                }
                else
                {
                    colours.Add(0);
                }
            }
        }
        
        SetupBoard(height, colours);
    }

    public void SetBoardWidth(float value)
    {
        ResizeBoard(BoardHeight, (int)value);
    }
    
    public void SetBoardHeight(float value)
    {
        ResizeBoard((int)value, BoardWidth);
    }

    public void SetBoardCode(string code)
    {
        try
        {
            var (height, colours) = EditorManager.Manager.FromLevelCode(code);
            SetupBoard(height, colours);
        }
        catch (Exception err)
        {
            Debug.LogError(err);
            EditorManager.Manager.UpdateLevelCode();
        }
    }
}
