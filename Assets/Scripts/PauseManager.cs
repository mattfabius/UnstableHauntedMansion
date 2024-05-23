using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool useTimeScale = true;

    [HideInInspector]
    public bool IsPaused = false;

    public delegate void PauseEvent();
    public event PauseEvent OnPause;
    public delegate void UnpauseEvent();
    public event UnpauseEvent OnUnpause;

    #region Singleton
    public static PauseManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public void Pause()
    {
        IsPaused = true;

        if (useTimeScale)
        {
            Time.timeScale = 0f;
        }

        if (OnPause != null)
        {
            OnPause();
        }
    }

    public void Unpause()
    {
        IsPaused = false;

        if (useTimeScale)
        {
            Time.timeScale = 1f;
        }

        if (OnUnpause != null)
        {
            OnUnpause();
        }
    }
}
