using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    private int currentClipIndex;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (audioClips.Length > 0)
        {
            currentClipIndex = Random.Range(0, audioClips.Length);
            PlayClip(currentClipIndex);
        }
        audioSource.volume = 0.05f;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentClipIndex = (currentClipIndex + 1) % audioClips.Length;
            PlayClip(currentClipIndex);
        }

        if (Input.GetKeyDown(KeyCode.M)){
            audioSource.mute = !audioSource.mute;
        }

    }

    private void PlayClip(int index)
    {
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
