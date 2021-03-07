using UnityEngine;

public class BookBehaviour : MonoBehaviour
{
    private static readonly int CollectAnimation = Animator.StringToHash("Collect");
    private Animator _animator;
    private AudioSource _pageTurnSound;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _pageTurnSound = GetComponent<AudioSource>();
        _pageTurnSound.time = 0.6f;
    }

    public void Collect()
    {
        _pageTurnSound.Play();
        _animator.SetTrigger(CollectAnimation);
        Destroy(gameObject, 1);
    }
}