using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public static EditorManager Manager;
    
    [SerializeField] private TMP_InputField levelCode;
    [SerializeField] private EditorBoardBehaviour editorBoard;
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider completeInSlider;
    [SerializeField] private TextMeshProUGUI completeInText;
    [SerializeField] private GameObject editorUiGroup;
    [SerializeField] private TextMeshProUGUI playButtonText;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject gameUiGroup;

    private int _completeIn = 1;
    private int _turns = 0;

    public bool IsEditing { get; private set; } = true;

    public int ColourIndex { get; set; }

    private void Awake()
    {
        if (Manager != null && Manager != this)
        {
            Destroy(this);
            return;
        }
        Manager = this;

        ColourIndex = 0;
    }

    public static Color GetColorFromIndex(int index)
    {
        var colors = new[]
        {
            new Color32(0xFF, 0xFF, 0xFF, 255),
            new Color32(0xEF, 0xEB, 0xEA, 255),
            new Color32(0xE4, 0xDB, 0XD6, 255),
            new Color32(0xF3, 0xB4, 0x86, 255),
            new Color32(0xD4, 0x71, 0x5D, 255),
            new Color32(0x4D, 0x23, 0x4A, 255),
        };
        return colors[index];
    }

    private const byte A = 65;
    private const byte Z = 90;
    private const byte AzLen = (Z - A) + 1;
    private const byte Zero = 48;
    private const byte TotalLen = AzLen + 10; // 10 numbers
    
    private static string IntToCode(int i)
    {
        if (i is < 0 or >= TotalLen)
        {
            throw new ArgumentOutOfRangeException($"-1 > ${i} > ${TotalLen}");
        }
        
        var code = i < AzLen ? A + i : Zero + (i - AzLen);
        var ascii = Encoding.ASCII.GetString(new []{ (byte)code });
        return ascii;
    }
    
    private static int CodeToInt(char code)
    {
        var bytes = Encoding.ASCII.GetBytes(new [] { code });
        if (bytes.Length != 1)
        {
            throw new ArgumentOutOfRangeException(nameof(code), code, "Must be exactly one char");
        }
        
        var ascii = (int)bytes[0];
        if (ascii is < Zero or > Zero + 10 and < A or > Z)
        {
            throw new ArgumentOutOfRangeException(nameof(code), code, "Must be A-Z/0-9");
        }
        
        var value = ascii >= A ? ascii - A : (ascii - Zero + AzLen);
        return value;
    }

    private static string CoupleToCode(int lhs, int rhs)
    {
        if (lhs is < 0 or > 6 || rhs is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException($"(0 < {lhs} > 6?) * (0 < {rhs} > 6?)");   
        }
        
        var couple = (lhs * 6) + rhs;
        return IntToCode(couple);
    }
    
    private static (int, int) CodeToCouple(char code)
    {
        var combined = CodeToInt(code);
        var rhs = combined % 6;
        var lhs = (combined - rhs) / 6;
        return (lhs, rhs);
    }

    public (int, List<int>) FromLevelCode(string code)
    {
        var upper = code.ToUpper();
        var chars = upper.ToCharArray();
        
        _completeIn = CodeToInt(chars[1]);
        completeInSlider.value = _completeIn;
        completeInText.text = $"{_completeIn} moves";

        var colourIndexes = new List<int>();
        for (var i = 2; i < chars.Length; i += 1)
        {
            var (lhs, rhs) = CodeToCouple(chars[i]);
            colourIndexes.Add(lhs);
            colourIndexes.Add(rhs);
        }

        var height = CodeToInt(chars[0]);
        if (height == 0) // height is 1 but with half slot
        {
            var last = colourIndexes.Count - 1;
            height = 1;
            colourIndexes.RemoveAt(last);
        }
        if (colourIndexes.Count % height == 1)
        {
            var last = colourIndexes.Count - 1;
            // remove padding
            if (colourIndexes[last] != 0)
            {
                throw new ArgumentException($"Expected padded ending, got {colourIndexes[last]}");
            }
            colourIndexes.RemoveAt(last);
        }
        
        var width = colourIndexes.Count / height;
        widthSlider.value = width;
        heightSlider.value = height;

        return (height, colourIndexes);
    }
    
    
    public void UpdateLevelCode()
    {
        var tiles = editorBoard.Tiles;

        var height = editorBoard.BoardHeight;
        if (height == 1 && editorBoard.BoardWidth % 2 == 1) height = 0;
        
        var buffer = IntToCode(height) + IntToCode(_completeIn);

        for (var i = 0; i < tiles.Count; i+=2)
        {
            var lhs = tiles[i].ColourIndex;
            var rhs = (i + 1 != tiles.Count) ? tiles[i + 1].ColourIndex : 0;
            var code = CoupleToCode(lhs, rhs);
            buffer += code;
        }

        levelCode.text = buffer;
        completeInText.text = $"{_completeIn} moves";
    }
    
    public void SetCompleteIn(float value)
    {
        _completeIn = (int) value;
        UpdateLevelCode();
    }

    public void ToggleEdit()
    {
        if (IsEditing)
        {
            // start playing!
            IsEditing = false;
            editorUiGroup.SetActive(false);
            gameUiGroup.SetActive(true);
            playButtonText.text = "Edit";
            _turns = _completeIn + 1;
            CanGo();
        }
        else
        {
            // start editing.
            IsEditing = true;
            editorUiGroup.SetActive(true);
            gameUiGroup.SetActive(false);
            playButtonText.text = "Play";
        }
    }

    public bool CanGo()
    {
        if (_turns <= 0) return false; // no go!
        _turns -= 1;
        turnText.text = $"{_turns} to complete";
        return true;
    }
}
