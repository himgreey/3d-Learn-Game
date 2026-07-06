using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : BasePanel
{
    public Button ContinueButton;
    public Button CheckEndButton;
    public Button CloseButton;
    public Toggle BKMusicToggle;
    public Toggle SoundToggle;
    public Slider BKMusicSlider;
    public Slider SoundSlider;

    private bool initialized;

    public override void Show()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
        }

        IsShow = true;
        Init();
    }

    public override void Init()
    {
        if (initialized)
        {
            RefreshValues();
            return;
        }

        initialized = true;
        BindAudioSettings();
        BindButtons();
        RefreshValues();
    }

    private void BindButtons()
    {
        if (ContinueButton != null)
        {
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(() => GamePauseManager.ClosePause());
        }

        if (CloseButton != null)
        {
            CloseButton.onClick.RemoveAllListeners();
            CloseButton.onClick.AddListener(() => GamePauseManager.ClosePause());
        }

        if (CheckEndButton != null)
        {
            CheckEndButton.onClick.RemoveAllListeners();
            CheckEndButton.onClick.AddListener(OnCheckEndClicked);
        }
    }

    private void OnCheckEndClicked()
    {
        GamePauseManager.ForceClosePause();
        Time.timeScale = 1f;

        if (PlayerObject.Instance != null)
        {
            PlayerObject.Instance.SettleGame();
        }
    }

    private void BindAudioSettings()
    {
        if (BKMusicToggle != null)
        {
            BKMusicToggle.onValueChanged.RemoveAllListeners();
            BKMusicToggle.onValueChanged.AddListener(isOn =>
            {
                if (BKMusic.Instance != null)
                {
                    BKMusic.Instance.SetIsOpen(isOn);
                }

                GameDataManager.Instance.musicData.BKMusicOn = isOn;
                GameDataManager.Instance.SaveMusicData();
            });
        }

        if (SoundToggle != null)
        {
            SoundToggle.onValueChanged.RemoveAllListeners();
            SoundToggle.onValueChanged.AddListener(isOn =>
            {
                GameDataManager.Instance.musicData.SoundOn = isOn;
                GameDataManager.Instance.SaveMusicData();
            });
        }

        if (BKMusicSlider != null)
        {
            BKMusicSlider.onValueChanged.RemoveAllListeners();
            BKMusicSlider.onValueChanged.AddListener(value =>
            {
                if (BKMusic.Instance != null)
                {
                    BKMusic.Instance.ChangeValue(value);
                }

                GameDataManager.Instance.musicData.BKMusicVolume = value;
                GameDataManager.Instance.SaveMusicData();
            });
        }

        if (SoundSlider != null)
        {
            SoundSlider.onValueChanged.RemoveAllListeners();
            SoundSlider.onValueChanged.AddListener(value =>
            {
                GameDataManager.Instance.musicData.SoundVolume = value;
                GameDataManager.Instance.SaveMusicData();
            });
        }
    }

    private void RefreshValues()
    {
        MusicData musicData = GameDataManager.Instance.musicData;

        if (BKMusicToggle != null) BKMusicToggle.isOn = musicData.BKMusicOn;
        if (SoundToggle != null) SoundToggle.isOn = musicData.SoundOn;
        if (BKMusicSlider != null) BKMusicSlider.value = musicData.BKMusicVolume;
        if (SoundSlider != null) SoundSlider.value = musicData.SoundVolume;
    }
}
