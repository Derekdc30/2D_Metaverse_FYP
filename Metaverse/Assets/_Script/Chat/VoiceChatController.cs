using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatController : NetworkBehaviour
{
    private AudioClip microphoneInput;
    public Toggle microphoneToggle; // Toggle component for voice chat control
    private AudioSource audioSource;
    private bool isRecording = false;

    private void Start()
    {
        // Ensure there's an AudioSource component to play back received audio
        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        SetupAudioSource(); // Configure the AudioSource

        // Initialize the Toggle state
        if (microphoneToggle != null)
        {
            microphoneToggle.isOn = false;
            microphoneToggle.onValueChanged.AddListener(ToggleRecording);
        }
    }

    private void Update()
    {
        // Check if the F key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (microphoneToggle != null && isRecording == false)
            {
                microphoneToggle.isOn = true; // Toggle the state
                isRecording = true;
            } else if (microphoneToggle != null && isRecording == true)
            {
                microphoneToggle.isOn = false; // Toggle the state
                isRecording = false;
            }
        }
    }

    private void SetupAudioSource()
    {
        // Configure the AudioSource
        audioSource.spatialBlend = 0.0f; // Make the audio source 2D
        audioSource.loop = false; // Do not loop the audio
        audioSource.playOnAwake = false; // Do not play audio on awake
    }

    private void ToggleRecording(bool isRecording)
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        if (isRecording)
        {
            // Start recording
            microphoneInput = Microphone.Start(null, true, 1, 44100);
            InvokeRepeating("SendAudio", 0, 1.0f); // Schedule SendAudio to be called every second
        }
        else
        {
            // Stop recording
            Microphone.End(null);
            CancelInvoke("SendAudio"); // Stop the scheduled SendAudio calls
        }
        #else
        Debug.LogError("Microphone functionality is not supported in WebGL builds.");
        #endif
    }


    private void SendAudio()
    {
        // Ensure the NetworkObject is ready and the client is active
        if (!IsServer || !NetworkObject.IsSpawned)
        {
            Debug.LogWarning("Cannot send audio because the NetworkObject is not spawned or server is not active.");
            return;
        }

        if (microphoneInput == null || !Microphone.IsRecording(null))
        {
            Debug.LogWarning("No audio input or microphone is not recording.");
            return;
        }

        // Extract audio samples from the AudioClip and convert to byte array
        float[] samples = new float[microphoneInput.samples * microphoneInput.channels];
        microphoneInput.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 4];
        System.Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
        SendAudioToAllClientsRpc(bytes);
    }


    [ObserversRpc]
    private void SendAudioToAllClientsRpc(byte[] audioData)
    {
        //if (IsOwner) return; // Prevent the sender from playing their own audio

        // Convert byte array back into float array
        float[] samples = new float[audioData.Length / 4];
        System.Buffer.BlockCopy(audioData, 0, samples, 0, audioData.Length);
        AudioClip clip = AudioClip.Create("ReceivedAudio", samples.Length, 1, 44100, false);
        clip.SetData(samples, 0);

        // Play the AudioClip
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
