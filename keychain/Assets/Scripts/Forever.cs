using UnityEngine;

public class Forever : MonoBehaviour
{
    private static bool _started;
    
    private void Start()
    {
        if (_started)
        {
            Destroy(gameObject);
        }
        else
        {
            _started = true;
            DontDestroyOnLoad(this);   
        }
    }
}