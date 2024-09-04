using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource whooshSource;
    public AudioSource bonusSource;
    public AudioSource freezeSource;
    public AudioSource fireSource;
    public AudioSource winSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayWhoosh()
    {
        whooshSource.Play();
    }

    public void PlayBonus()
    {
        bonusSource.Play();
    }

    public void PlayFreeze()
    {
        freezeSource.Play();
    }

    public void PlayFire()
    {
        fireSource.Play();
    }

    public void PlayWin()
    {
        winSource.Play();
    }
}