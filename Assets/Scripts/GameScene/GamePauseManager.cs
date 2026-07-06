using UnityEngine;

public static class GamePauseManager
{
    public static bool IsPaused { get; private set; }

    public static bool CanOpenPause()
    {
        if (PlayerObject.Instance == null || PlayerObject.Instance.IsDead)
        {
            return false;
        }

        if (UIManager.Instance.HasBlockingModalExceptPause())
        {
            return false;
        }

        return true;
    }

    public static void HandleEscape()
    {
        if (IsPaused)
        {
            ClosePause();
            return;
        }

        if (UIManager.Instance.TryDismissModalOnEscape())
        {
            return;
        }

        OpenPause();
    }

    public static void TogglePause()
    {
        if (IsPaused)
        {
            ClosePause();
            return;
        }

        OpenPause();
    }

    public static void OpenPause()
    {
        if (!CanOpenPause())
        {
            return;
        }

        IsPaused = true;
        Time.timeScale = 0f;
        UIManager.Instance.showPanel<GamePausePanel>();
    }

    public static void ClosePause()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.hidePanel<GamePausePanel>();
    }

    public static void ResetForNewGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.ResetModalCursorState();
    }

    public static void ForceClosePause()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        UIManager.Instance.hidePanel<GamePausePanel>();
    }
}
