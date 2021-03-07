using UnityEngine;

public class ShelfBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] books;
    private int _bookCount;
    private AudioSource _shelfSoundEffect;

    private void Awake()
    {
        _shelfSoundEffect = GetComponent<AudioSource>();
    }

    public void AddBooks(int no)
    {
        _shelfSoundEffect.Play();
        for (var i = 0; i < no; i++)
        {
            // this shouldn't happen so long as levels are properly designed :tm:
            if (_bookCount < books.Length && books[_bookCount]) books[_bookCount].SetActive(true);
            _bookCount += 1;
        }
    }
}