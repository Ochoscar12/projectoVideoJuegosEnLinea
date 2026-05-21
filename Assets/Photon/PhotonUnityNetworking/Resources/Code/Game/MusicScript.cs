using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript instance;

    public AudioClip backgroundMusic;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton check
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.volume = 0.25f;
    }
}
