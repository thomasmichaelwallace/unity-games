using UnityEngine;

public class Forever : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}