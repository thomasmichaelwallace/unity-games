using UnityEngine;

public class MusicBehavbiour : MonoBehaviour
{
    private static MusicBehavbiour _instance;


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