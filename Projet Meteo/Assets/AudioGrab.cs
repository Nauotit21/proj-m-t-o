using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class GrabSound : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip grabSound;
    public AudioClip releaseSound;

    [Header("Paramètres")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.8f, 1.2f)] public float pitchVariation = 0.1f;

    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Configuration de l'AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // Son 3D

        // Abonnement aux événements de grab
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        // Désabonnement pour éviter les fuites mémoire
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        PlaySound(grabSound);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        PlaySound(releaseSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        // Variation légère du pitch pour éviter la répétition monotone
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip, volume);
    }
}
