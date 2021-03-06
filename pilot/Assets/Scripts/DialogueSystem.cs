﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public GameObject TitleBox;
    public TextMeshProUGUI TitleText;
    public TextAsset Data;
    public string LinkColour;
    public TextMeshProUGUI DialogueText;
    public GameObject DialogueBox;

    public Texture2D CursorTexture;
    public Material HoverMaterial;

    public bool CanSelect => !DialogueBox.activeSelf;

    private Dictionary<string, string> script = new Dictionary<string, string>();
    private string pointer = "_init_";
    private OpenHyperlinks hyperlinks;

    private void Start()
    {
        script = ParseScript(Data.text);
        hyperlinks = FindObjectOfType<OpenHyperlinks>();
        SetPointer("_init_");
    }

    private Dictionary<string, string> ParseScript(string text)
    {
        Dictionary<string, string> stack = new Dictionary<string, string>();

        string key = "";
        string value = "";
        var lines = (text + "\n# eof").Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith("#"))
            {
                if (key.Length > 0)
                {
                    value = Regex.Replace(value, @"\*\*(.*)\*\*", "<b>$1</b>");
                    value = Regex.Replace(value, @"\*(.*)\*", "<i>$1</i>");
                    stack[key] = value.Trim();
                }

                key = line.Replace("#", "").Trim();
                value = "";
            }
            else if (line.StartsWith("["))
            {
                // link
                Regex link = new Regex(@"\[(.*)\]\((.*)\)");
                var matches = link.Matches(line);

                string label = matches[0].Groups[1].Value;
                if (label == "!") label = "-- x --";

                string id = matches[0].Groups[2].Value;
                if (id == "!") id = "_close_";

                value += $"\n<link=\"{id}\"><color=\"{LinkColour}\">{label}</color></link>";
            }
            else
            {
                value += $"\n{line}";
            }
        }
        return stack;
    }

    public void TriggerPointer(string id)
    {
        if (DialogueBox.activeSelf) return;
        SetPointer(id);
    }

    public void SetPointer(string id)
    {
        if (id == "_close_")
        {
            DialogueBox.SetActive(false);
        }
        else if (id.EndsWith("!"))
        {
            TitleBox.SetActive(true);
            TitleText.text = script[id] + "\n\n<color=#59405c><size=80%>click to restart</size></color>";
            TitleText.SetAllDirty();
            TitleText.ForceMeshUpdate(true);
            DialogueBox.SetActive(false);
        }
        else
        {
            DialogueBox.SetActive(true);
            pointer = id;
            DialogueText.text = script[pointer];
            DialogueText.SetAllDirty();
            DialogueText.ForceMeshUpdate(true);
            hyperlinks.SkipOnce = true;
        }
    }
}