using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } = null;

    public AudioSource audioSource;
    public Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        // ** 초기화
        audioSource = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();
    }

    public void PlaySound(string _key)
    {
        if (!soundDictionary.ContainsKey(_key))
        {
            Debug.Log("없음..");
            return;
        }

        var clip = soundDictionary[_key];
        audioSource.PlayOneShot(clip);
    }

    public void PlayBGM(string _key)
    {
        if (!soundDictionary.ContainsKey(_key))
        {
            Debug.Log("BGM 없음..");
            return;
        }

        audioSource.clip = soundDictionary[_key];
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void AddSoundClip(AudioClip _clip)
    {
        soundDictionary[_clip.name] = _clip;
    }


    void Start()
    {
        // ** 사운드 불러오기.


    }
}