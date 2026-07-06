using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button ExitButton;
    public Toggle BKMusicToggle;
    public Toggle SoundToggle;
    public Slider BKMusicSlider;
    public Slider SoundSlider;
    public override void Init()
    {
        MusicData musicData = GameDataManager.Instance.musicData;
        BKMusicToggle.isOn = musicData.BKMusicOn;
        SoundToggle.isOn = musicData.SoundOn;
        BKMusicSlider.value = musicData.BKMusicVolume;
        SoundSlider.value = musicData.SoundVolume;

        ExitButton.onClick.AddListener(() =>
        {
            UIManager.Instance.hidePanel<SettingPanel>();
        });
        BKMusicToggle.onValueChanged.AddListener((isOn) =>
        {
            BKMusic.Instance.SetIsOpen(isOn);
            GameDataManager.Instance.musicData.BKMusicOn = isOn;
            GameDataManager.Instance.SaveMusicData();
        });
        SoundToggle.onValueChanged.AddListener((isOn) =>
        {
            //Sound.Instance.SetIsOpen(isOn);
            GameDataManager.Instance.musicData.SoundOn = isOn;
            GameDataManager.Instance.SaveMusicData();
        });
        BKMusicSlider.onValueChanged.AddListener((value) =>
        {
            BKMusic.Instance.ChangeValue(value);
            GameDataManager.Instance.musicData.BKMusicVolume = value;
            GameDataManager.Instance.SaveMusicData();
        });
        SoundSlider.onValueChanged.AddListener((value) =>
        {
            //Sound.Instance.ChangeValue(value);
            GameDataManager.Instance.musicData.SoundVolume = value;
            GameDataManager.Instance.SaveMusicData();
        });
    }
}
