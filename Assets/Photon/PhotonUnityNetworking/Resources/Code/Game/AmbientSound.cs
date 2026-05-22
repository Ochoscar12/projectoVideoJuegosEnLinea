using UnityEngine;
using System.Collections;


public class AmbientSound : MonoBehaviour
{
    public AudioClip[] ambientClips;

    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlayRandomSounds());
    }

    IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            // Wait random amount of time
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Pick random clip
            int randomIndex = Random.Range(0, ambientClips.Length);

            // Play it
            audioSource.PlayOneShot(ambientClips[randomIndex]);
        }
    }
}
