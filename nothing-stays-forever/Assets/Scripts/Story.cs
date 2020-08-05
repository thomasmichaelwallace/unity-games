using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    private class Sentence
    {
        public string Template;
        public string[] Words;
        public string[] Colours = { "#ff0000", "#00ff00", "#0000ff" };

        private int wordIndex = 0;
        private int colourIndex = 0;

        public Sentence(string template, string[] words)
        {
            Template = template;
            Words = words;
        }

        public string NextWord()
        {
            wordIndex += 1;
            if (wordIndex >= Words.Length) wordIndex = 0;
            colourIndex = wordIndex % Colours.Length;
            return GetText();
        }

        public string GetText()
        {
            string word = Words[wordIndex];
            string colour = Colours[colourIndex];
            string substitution = $"<color={colour}>{word}</color>";
            string text = Template.Replace("{{ word }}", substitution);
            return text;
        }
    }

    public float AdvanceTime = 5f;
    public float DegradeTime = 3f;

    private readonly Sentence[] sentences = {
        new Sentence("My best friend was called {{ word }}", new string[] { "John", "Jill", "James" }),
        new Sentence("The loved {{ word }}", new string[] { "Dancing", "Walking", "Singing" }),
    };

    private TextMeshProUGUI text;
    private int sentenceIndex = 0;
    private float sentenceTime = 0f;
    private Vector3 drift;
    private bool isDegrading = false;
    private float degradeTime = 0f;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        SetText();
        SetDrift();
    }

    private void Update()
    {
        transform.position += drift * Time.deltaTime;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (
            (screenPosition.x > Screen.width && drift.x > 0)
            || (screenPosition.x < 0 && drift.x < 0))
        {
            drift.x = -drift.x;
        }
        if (
            (screenPosition.y > Screen.height && drift.y > 0)
            || (screenPosition.y < 0 && drift.y < 0))
        {
            drift.y = -drift.y;
        }

        sentenceTime += Time.deltaTime;
        if (sentenceTime > AdvanceTime)
        {
            sentenceIndex += 1;
            if (sentenceIndex >= sentences.Length) sentenceIndex = 0;
            SetText();
            sentenceTime = 0f;

            if (sentenceIndex == 0)
            {
                isDegrading = true;
            }

            SetDrift();
        }

        if (isDegrading)
        {
            degradeTime += Time.deltaTime;
            if (degradeTime > DegradeTime)
            {
                degradeTime = 0;
                sentences[sentenceIndex].NextWord();
                SetText();
            }
        }
    }

    public void OnClick()
    {
        NextWord();
    }

    private void SetDrift()
    {
        drift = Random.insideUnitSphere;
        // drift *= 10;
        drift *= 0.1f;
        drift.z = 0;
    }

    private void NextWord()
    {
        sentences[sentenceIndex].NextWord();
        SetText();
    }

    private void SetText()
    {
        string sentence = sentences[sentenceIndex].GetText();
        text.SetText(sentence);
    }
}