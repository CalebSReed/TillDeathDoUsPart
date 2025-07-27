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

        audioSource.clip = s.clip;
        audioSource.loop = s.loop;
        audioSource.dopplerLevel = 0;

        switch (s.soundType)
        {
            case Sound.SoundType.SoundEffect:
                audioSource.volume = s.volumeMult;                
                audioSource.pitch = Random.Range(.75f, 1.25f);
                s.soundMode = Sound.SoundMode.ThreeDimensional;
                break;
            case Sound.SoundType.Ambience:
                audioSource.volume = s.volumeMult;
                audioSource.pitch = Random.Range(.75f, 1.25f);
                s.soundMode = Sound.SoundMode.TwoDimensional;
                break;
            case Sound.SoundType.Music:
                audioSource.volume = s.volumeMult;
                s.soundMode = Sound.SoundMode.TwoDimensional;
                audioSource.pitch = 1;
                break;
        }
        
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

    public void ChangeMusicVolume(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            SoundPrefab spf = poolParent.GetChild(i).GetComponent<SoundPrefab>();//in the future for optimizations, initialize a list of the references and cache it. add extras if poolSize changes
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                spf.gameObject.GetComponent<AudioSource>().volume = spf.volMult;
            }
        }
    } 

    public void ChangeAmbienceVolume(string name)
    {
        for (int i = 0; i < poolParent.childCount; i++)
        {
            SoundPrefab spf = poolParent.GetChild(i).GetComponent<SoundPrefab>();//in the future for optimizations, initialize a list of the references and cache it. add extras if poolSize changes
            if (poolParent.GetChild(i).GetComponent<SoundPrefab>().soundName == name)
            {
                spf.gameObject.GetComponent<AudioSource>().volume = spf.volMult;
            }
        }
    }

    public void DestroySound(GameObject obj)
    {
        soundPool.DespawnObject(obj);
    }
}
