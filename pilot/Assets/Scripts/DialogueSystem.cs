﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public TextAsset Data;
    public string LinkColour;
    public TextMeshProUGUI DialogueText;
    public GameObject DialogueBox;

    private Dictionary<string, string> script = new Dictionary<string, string>();
    private string pointer = "_init_";

    private void Start()
    {
        script = ParseScript(Data.text);
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
                    Regex.Replace(value, @"\*\*(.*)\*\*", "<b>$1</b>");
                    Regex.Replace(value, @"\*(.*)\*", "<i>$1</i>");
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
                if (id == "*") id = "_end_";

                value += $"\n<link=\"{id}\"><color=\"{LinkColour}\">{label}</color></link>";
            }
            else
            {
                value += $"\n{line}";
            }
        }
        return stack;
    }

    public void SetPointer(string id)
    {
        if (id == "_close_")
        {
            DialogueBox.SetActive(false);
        }
        else if (id == "_end_")
        {
            DialogueBox.SetActive(true);
            DialogueText.SetText("_you_win_");
        }
        else
        {
            DialogueBox.SetActive(true);
            pointer = id;
            DialogueText.SetText(script[pointer]);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}