using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundEffectMangager : MonoBehaviour
{
    private static SoundEffectMangager Instance;

    private static AudioSource audioSource;
    private static SoundEffectsLibary SoundEffectsLibary;
    [SerializeField] private Slider sfxSlider; 

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            SoundEffectsLibary = GetComponent<SoundEffectsLibary>(); 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = SoundEffectsLibary.GetRandomClip(soundName);
        if(audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); }); 
    }

 public static void SetVolume(float volume) 
    {
        audioSource.volume = volume; 
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value); 
    }
}
