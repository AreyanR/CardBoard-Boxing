using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    private static MusicManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            PlayMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (IsFightScene())
        {
            StopMusic();
        }
        else
        {
            PlayMusic();
        }
    }

    private bool IsFightScene()
    {
        return SceneManager.GetActiveScene().name == "fight";
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
