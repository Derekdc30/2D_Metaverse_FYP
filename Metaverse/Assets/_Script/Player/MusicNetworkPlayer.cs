using FishNet.Object;
using UnityEngine;

public class MusicNetworkPlayer : NetworkBehaviour
{
    public AudioClip musicClip; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();  
        audioSource.clip = musicClip; 
        audioSource.loop = true; 
        audioSource.playOnAwake = false; 
        audioSource.spatialBlend = 0; 
        audioSource.volume = 0.3f; 
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        audioSource.Play();  
    }
}
