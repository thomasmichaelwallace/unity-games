using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerBehaviour : MonoBehaviour
{
    private static readonly int WinAnimation = Animator.StringToHash("Win");
    private static readonly int JumpAnimation = Animator.StringToHash("Jump");
    private static readonly int DieAnimation = Animator.StringToHash("Die");
    [SerializeField] private BoardBehaviour board;
    [SerializeField] private GameObject[] books;
    [SerializeField] private GameObject fader;
    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource fallSound;
    private Animator _animator;
    private CanvasGroup _fade;
    private TextMeshProUGUI _interText;
    private bool _isAnimating;
    private bool _isClosing;
    private bool _isOpening = true;
    private BoardBehaviour.TileResponse _landing;
    private int _row, _column; // from bottom left, to match ascii representation
    private int _weight = 1; // player weights 1 + no. books

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _fade = fader.GetComponentInChildren<CanvasGroup>();
        _interText = fader.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        // board parses current level on awake
        var start = board.GetStartingPosition();
        _row = start[0];
        _column = start[1];

        var t = transform;
        var position = t.position;
        position.x = _column * BoardBehaviour.TileSize;
        position.z = _row * BoardBehaviour.TileSize;
        t.position = position;

        // explicitly disable books, just in case these are left active in design mode testing
        ClearBooks();

        // set loader text
        _interText.text = $"Level {LevelManager.LevelNo}";
    }

    private void Update()
    {
        // fader
        if (_isClosing)
        {
            _fade.alpha += Time.deltaTime / fadeOutTime;
            if (_fade.alpha >= 1)
            {
                var target = LevelManager.LevelNo <= LevelManager.Levels.Length
                    ? SceneManager.GetActiveScene().buildIndex
                    : 0;
                SceneManager.LoadScene(target);
            }

            return;
        }

        if (_isOpening)
        {
            _fade.alpha -= Time.deltaTime / fadeInTime;
            if (_fade.alpha <= 0)
            {
                _isOpening = true;
                _interText.text = "";
            }

            // allow game play when apparently light enough
            if (_fade.alpha > 0.8f) return;
        }

        if (_isAnimating) return;


        var rotation = transform.rotation;
        var nextColumn = _column;
        var nextRow = _row;

        // prefer horizontal movement in deadlock
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if (horizontal > 0)
        {
            rotation.SetLookRotation(Vector3.right); // movement via. animation root motion
            nextColumn += 1;
        }
        else if (horizontal < 0)
        {
            rotation.SetLookRotation(Vector3.left);
            nextColumn -= 1;
        }
        else if (vertical > 0)
        {
            rotation.SetLookRotation(Vector3.forward);
            nextRow += 1;
        }
        else if (vertical < 0)
        {
            rotation.SetLookRotation(Vector3.back);
            nextRow -= 1;
        }
        else
        {
            return;
        }

        var response = board.StepOn(nextRow, nextColumn, _weight);
        switch (response)
        {
            case BoardBehaviour.TileResponse.WinAndReset:
            {
                ClearBooks();
                _animator.SetTrigger(WinAnimation);
                _isAnimating = true;
                return;
            }
            case BoardBehaviour.TileResponse.EmptyAndBlock:
            {
                ClearBooks();
                return;
            }
            // in all other cases, player moves, then responds:
            case BoardBehaviour.TileResponse.Support:
            case BoardBehaviour.TileResponse.Fall:
            case BoardBehaviour.TileResponse.CollectBook:
            {
                transform.rotation = rotation;
                _animator.SetTrigger(JumpAnimation);
                _isAnimating = true;

                _landing = response;

                board.StepOff(_row, _column, _weight);
                _row = nextRow;
                _column = nextColumn;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnAnimationEnded(string animationName)
    {
        // called as player animation event
        switch (animationName)
        {
            case "Jump" when _landing == BoardBehaviour.TileResponse.Fall:
                fallSound.Play();
                _animator.SetTrigger(DieAnimation);
                return;
            case "Jump":
            {
                walkSound.pitch = Random.Range(0.9f, 1.1f);
                walkSound.Play();
                _isAnimating = false;
                if (_landing == BoardBehaviour.TileResponse.CollectBook)
                {
                    _weight += 1;
                    var bookIndex = _weight - 2; // - player weight, to zero index
                    // out of bounds shouldn't happen on a well designed level :tm:
                    if (bookIndex >= 0 && bookIndex < books.Length && books[bookIndex])
                        books[bookIndex].SetActive(true);
                }

                break;
            }
            case "Die":
                _isClosing = true;
                break;
            case "Win":
                LevelManager.LevelNo += 1;
                _isClosing = true;
                break;
        }
    }


    private void ClearBooks()
    {
        _weight = 1;
        foreach (var t in books)
            t.SetActive(false);
    }
}