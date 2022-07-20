using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource sliceStart;
    [SerializeField] private AudioSource sliceEnd;
    [SerializeField] private AudioSource roll;
    [SerializeField] private AudioSource success;

    public void PlayStart()
    {
        sliceStart.Play();
    }

    public void PlayEnd()
    {
        sliceEnd.Play();
    }

    public void PlayRoll()
    {
        roll.Play();
    }

    public void PlayWin()
    {
        success.Play();
    }
}