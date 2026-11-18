using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool _isPaused = false;

    public void TogglePause()
    {
        if (_isPaused)
        {
            Time.timeScale = 1f;
            _isPaused = false;
        }
        else
        {
            Time.timeScale = 0f;
            _isPaused = true;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

}
