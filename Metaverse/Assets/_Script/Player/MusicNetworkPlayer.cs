using FishNet.Object;
using UnityEngine;

public class MusicNetworkPlayer : NetworkBehaviour
{
    public AudioClip musicClip; // Public variable to assign music clip in editor

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();  // Add an AudioSource dynamically
        audioSource.clip = musicClip; // Assign the music clip
        audioSource.loop = true; // Set to loop
        audioSource.playOnAwake = false; // Do not play on awake
        audioSource.spatialBlend = 0; // Set spatial blend to 0 for 2D sound
        audioSource.volume = 0.3f; // Start at 0.3 volume
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        audioSource.Play();  // Play music when the server starts
    }
}
