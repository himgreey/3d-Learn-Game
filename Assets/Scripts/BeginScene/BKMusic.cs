using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKMusic : MonoBehaviour
{
    private static BKMusic instance;
    public static BKMusic Instance => instance;

    private AudioSource bkSource;

    private void Awake()
    {
        instance = this;
        bkSource = GetComponent<AudioSource>();
        MusicData musicData = GameDataManager.Instance.musicData;
        SetIsOpen(musicData.BKMusicOn);
        ChangeValue(musicData.BKMusicVolume);
    }
    
    public void SetIsOpen(bool isOpen)
    {
        bkSource.mute = !isOpen;
    }

    public void ChangeValue(float value)
    { 
        bkSource.volume = value;
    }

}
