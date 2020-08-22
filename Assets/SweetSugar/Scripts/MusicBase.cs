using SweetSugar.Scripts.Core;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace SweetSugar.Scripts
{
    /// <summary>
    /// Music manager
    /// </summary>
    public class MusicBase : MonoBehaviour
    {

        public static MusicBase Instance;
        public AudioClip[] music;
        private AudioSource audioSource;
        public AudioMixer audioMixer;

        ///MusicBase.Instance.audio.PlayOneShot(MusicBase.Instance.music[0]);


        // Use this for initialization
        void Awake()
        {
     
            audioSource = GetComponent<AudioSource>();
            audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
            audioSource.loop = true;
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetInt("Music"));
        }

        private void OnEnable()
        {
            LevelManager.OnMapState += OnMapState;
            LevelManager.OnEnterGame += OnGameState;
        }

        private void OnDisable()
        {
            LevelManager.OnMapState -= OnMapState;
            LevelManager.OnEnterGame -= OnGameState;
        }

        private void OnGameState()
        {
            if (audioSource.clip == music[0])
            {
                audioSource.clip = music[Random.Range(1, 3)];
            }
            if(!audioSource.isPlaying)
                audioSource.Play();
        }

        private void OnMapState()
        {
            if (audioSource.clip != music[0])
            {
                audioSource.clip = music[0];
            }
            if(!audioSource.isPlaying)
                audioSource.Play();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
