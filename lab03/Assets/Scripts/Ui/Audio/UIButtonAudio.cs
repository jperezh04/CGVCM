using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("Audio Clips")]
    [SerializeField] protected AudioClip hoverSound;
    [SerializeField] protected AudioClip clickSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] protected float hoverVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] protected float clickVolume = 0.5f;

    protected AudioSource audioSource;
    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D UI sound
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            PlaySound(hoverSound, hoverVolume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
            PlaySound(clickSound, clickVolume);
    }

    protected void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}
