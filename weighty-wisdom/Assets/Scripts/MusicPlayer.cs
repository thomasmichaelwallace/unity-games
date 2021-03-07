using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);

            var audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }
        else
        {
            if (this != _instance)
                Destroy(gameObject);
        }
    }
}