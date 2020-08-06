using System.Linq;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    private class Sentence
    {
        public string Template;
        public string[] Words;
        public string[] Colours = { "#f38181", "#95e1d3", "#fce38a" };

        public bool Correct
        {
            get { return correctWord == wordIndex; }
        }

        private int wordIndex = 0;
        private readonly int correctWord;
        private int colourIndex = 0;

        public Sentence(string template, string[] words, int offset)
        {
            Template = template;
            Words = words;

            wordIndex = offset - 1;
            NextWord();
            correctWord = offset;
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

    public float AdvanceTime = 8f;
    public float DegradeTime = 10f;
    private readonly float speed = 0.10f;

    private readonly Sentence[] sentences = {
        new Sentence("My best friend at school was called {{ word }}", new string[] { "Ellis", "Sasha", "Ray" }, 0),
        new Sentence("They loved {{ word }}", new string[] { "football", "painting", "singing" }, 1),
        new Sentence("We spent the summer {{ word }}", new string[] { "on the swings", "around the town", "in the woods" }, 2),
    };

    private TextMeshProUGUI text;
    private int sentenceIndex = 0;
    private float sentenceTime = 0f;
    private Vector3 drift;
    private bool isDegrading = false;
    private float degradeTime = 0f;
    private readonly float proportion = 2.0f;

    public float Correctness { get; private set; }

    public float Weight
    {
        get { return sentences.Length * proportion; }
    }

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        SetText();
        SetDrift();

        Correctness = 1;
    }

    private void Update()
    {
        transform.position += drift * Time.deltaTime * speed;

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
                degradeTime = -UnityEngine.Random.value * DegradeTime;
                sentences[sentenceIndex].NextWord();
                SetText();
            }
        }

        Correctness = sentences.Sum(s => s.Correct ? proportion : 0f);
    }

    public void OnClick()
    {
        NextWord();
        degradeTime = -UnityEngine.Random.value * DegradeTime;
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