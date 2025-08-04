using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour//we need multiple instances of this. store sound[] in separate GO then reference that i suppose
{
    public static AudioManager Instance;

    [SerializeField] private ObjectPool soundPool;
    [SerializeField] private Transform poolParent;
    [SerializeField] SoundsList playerSoundsList;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SoundOptions.Instance.OnMusicChanged += ChangeMusicVolume;
        SoundOptions.Instance.OnSoundFXChanged += ChangeSFXVolume;
    }

    public AudioSource Play(string name, Vector3 position, GameObject objSource = null, bool overrideTwoDimensional = false, bool followSource = false, bool forcePitch = false)//add an overload to search by mobname btw
    {
        var audioSource = soundPool.SpawnObject().GetComponent<AudioSource>();
        Sound s = null;
        
        if (IsSoundInList(playerSoundsList.sounds, name))
        {
            s = FindSoundInList(playerSoundsList.sounds, name);
        }

        if (s == null)
        {
            Debug.LogError($"Bro this the wrong got damn sound: {name}");
        }

        switch (s.soundType)
        {
            case Sound.SoundType.SoundEffect:
                audioSource.volume = SoundOptions.Instance.SoundFXVolume * s.volumeMult;
                audioSource.pitch = Random.Range(.75f, 1.25f);
                s.soundMode = Sound.SoundMode.ThreeDimensional;
                break;
            case Sound.SoundType.Ambience:
                audioSource.volume = SoundOptions.Instance.AmbienceVolume * s.volumeMult;
                audioSource.pitch = Random.Range(.75f, 1.25f);
                s.soundMode = Sound.SoundMode.TwoDimensional;
                break;
            case Sound.SoundType.Music:
                audioSource.volume = SoundOptions.Instance.MusicVolume * s.volumeMult;
                s.soundMode = Sound.SoundMode.TwoDimensional;
                audioSource.pitch = 1;
                break;
        }

        audioSource.clip = s.clip;
        audioSource.loop = s.loop;
        audioSource.dopplerLevel = 0;
        
        if (s.soundMode == Sound.SoundMode.TwoDimensional || overrideTwoDimensional)
        {
            audioSource.spatialBlend = 0;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 5;
            audioSource.maxDistance = 85;
            audioSource.spread = 0;
        }
        else
        {
            audioSource.spatialBlend = 1;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 5;
            audioSource.maxDistance = 85;
            audioSource.spread = 0;
        }

        if (forcePitch)
        {
            audioSource.pitch = 1;
        }

        var spf = audioSource.gameObject.GetComponent<SoundPrefab>();
        spf.soundName = s.name;
        spf.soundType = s.soundType;
        spf.clip = s.clip;
        spf.loops = s.loop;
        spf.follow = followSource;
        spf.source = objSource;
        spf.volMult = s.volumeMult;
        spf.StartTimer();

        audioSource.gameObject.transform.position = position;
        audioSource.Play();
        
        return audioSource;
    }

    private bool IsSoundInList(Sound[] list, string soundName)
    {
        foreach (Sound s in list)
        {
            if (s.name == soundName)
            {
                return true;
            }
        }
        return false;
    }

    private Sound FindSoundInList(Sound[] list, string soundName)
    {
        foreach(Sound s in list)
        {
            if (s.name == soundName)
            {
                return s;
            }
        }
        return null;
    }

    public void Pause(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                poolParent.GetChild(i).GetComponent<AudioSource>().Pause();
                poolParent.GetChild(i).GetComponent<SoundPrefab>().PauseTimer();
            }
        }
    }

    public void Pause(AudioSource source)
    {
        source.Pause();
        source.GetComponent<SoundPrefab>().PauseTimer();
    }

    public void UnPause(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                poolParent.GetChild(i).GetComponent<AudioSource>().UnPause();
                poolParent.GetChild(i).GetComponent<SoundPrefab>().ResumeTimer();
            }
        }
        //s.source.UnPause();
    }

    public void UnPause(AudioSource source)
    {
        source.UnPause();
        source.GetComponent<SoundPrefab>().ResumeTimer();
    }

    public void Stop(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                poolParent.GetChild(i).GetComponent<AudioSource>().Stop();
                DestroySound(poolParent.GetChild(i).gameObject);
            }
        }

        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volume / 2f, s.volume / 2f)); random volume change
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitch / 2f, s.pitch / 2f)); random pitch change

        //s.source.Stop();
    }

    public void ChangeMusicVolume(object sender, EventArgs e)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            var child = poolParent.GetChild(i).gameObject;
            
            if (child.activeSelf)
            {
                var soundPrefab = child.GetComponent<SoundPrefab>();

                if (soundPrefab.soundType == Sound.SoundType.Music)
                {
                    soundPrefab.GetComponent<AudioSource>().volume = soundPrefab.volMult * SoundOptions.Instance.MusicVolume;
                }
            }
        }
    }

    public void ChangeSFXVolume(object sender, EventArgs e)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            var child = poolParent.GetChild(i).gameObject;

            if (child.activeSelf)
            {
                var soundPrefab = child.GetComponent<SoundPrefab>();

                if (soundPrefab.soundType == Sound.SoundType.SoundEffect)
                {
                    soundPrefab.GetComponent<AudioSource>().volume = soundPrefab.volMult * SoundOptions.Instance.SoundFXVolume;
                }
            }
        }
    }

    public void ChangeAmbienceVolume(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            SoundPrefab spf = poolParent.GetChild(i).GetComponent<SoundPrefab>();
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                spf.gameObject.GetComponent<AudioSource>().volume = spf.volMult * SoundOptions.Instance.AmbienceVolume;
            }
        }
    }

    public void DestroySound(GameObject obj)
    {
        soundPool.DespawnObject(obj);
    }
}
