using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatController : NetworkBehaviour
{
    private bool isRecording = false;
    private AudioClip microphoneInput;
    public Button microphoneButton;
    private AudioSource audioSource;

    private void Start()
    {
        // Ensure there's an AudioSource component to play back received audio
        audioSource = gameObject.AddComponent<AudioSource>();
        SetupAudioSource(); // Configure the AudioSource for 3D sound

        // Assign the ToggleRecording method to the microphone button
        if (microphoneButton != null)
            microphoneButton.onClick.AddListener(ToggleRecording);
    }

    private void SetupAudioSource()
    {
        audioSource.spatialBlend = 1.0f; // Make the audio source 3D.
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Use linear rolloff
        audioSource.minDistance = 1.0f; // Minimum distance for full volume
        audioSource.maxDistance = 50.0f; // Max distance where the sound can be heard
    }

    public void ToggleRecording()
    {
        isRecording = !isRecording;

        if (isRecording)
        {
            // Start recording
            microphoneInput = Microphone.Start(null, true, 1, 44100);
            InvokeRepeating("SendAudio", 0, 1.0f); // Adjust the repeat rate as needed
        }
        else
        {
            // Stop recording and sending audio
            Microphone.End(null);
            CancelInvoke("SendAudio");
        }
    }

    private void SendAudio()
    {
        if (!isRecording || microphoneInput == null) return;

        float[] samples = new float[microphoneInput.samples * microphoneInput.channels];
        microphoneInput.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 4];
        System.Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
        Vector3 senderPosition = transform.position; // Assuming this object's position represents the player
        SendAudioToAllClientsRpc(bytes, senderPosition);
    }

    [ObserversRpc]
    private void SendAudioToAllClientsRpc(byte[] audioData, Vector3 senderPosition)
    {
        if (IsOwner) return; // The sender doesn't play their own audio

        // Convert byte array back into float array and create AudioClip
        float[] samples = new float[audioData.Length / 4];
        System.Buffer.BlockCopy(audioData, 0, samples, 0, audioData.Length);
        AudioClip clip = AudioClip.Create("ReceivedAudio", samples.Length, 1, 44100, false);
        clip.SetData(samples, 0);

        // Instead of playing the audio clip directly, instantiate an AudioSource at the sender's position
        AudioSource.PlayClipAtPoint(clip, senderPosition, 1f); // Volume can be adjusted
    }
}
