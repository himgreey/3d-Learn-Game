using UnityEngine;

public static class GameCursor
{
    public static void UnlockForUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void TryLockForGameplay(bool noModalPanels)
    {
        if (!noModalPanels || !ShouldUseLockedCursor())
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static bool ShouldUseLockedCursor()
    {
        if (PlayerObject.Instance == null || PlayerObject.Instance.IsDead)
        {
            return false;
        }

        if (GamePauseManager.IsPaused)
        {
            return false;
        }

        return true;
    }
}
