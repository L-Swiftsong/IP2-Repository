using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private bool _isPaused;
    [SerializeField] private InputActionReference[] _pauseActions;

    private const string DEFAULT_ACTION_MAP = "Player";
    private const string PAUSE_MENU_ACTION_MAP = "UI";

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenuHolder;


    private void Start()
    {
        foreach (InputActionReference pauseAction in _pauseActions)
            pauseAction.action.performed += OnPausePressed;

        Unpause();
    }

    public void OnPausePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            TogglePause();
    }


    private void TogglePause()
    {
        if (_isPaused)
            Unpause();
        else
            Pause();
    }


    private void Pause()
    {
        // Pause the Game.
        PlayerManager.Instance.PlayerInput.SwitchCurrentActionMap(PAUSE_MENU_ACTION_MAP);
        Time.timeScale = 0f;
        _isPaused = true;

        // Show the Pause Menu.
        _pauseMenuHolder.SetActive(true);
    }
    public void Unpause()
    {
        // Hide the Pause Menu.
        _pauseMenuHolder.SetActive(false);

        // Unpause the Game.
        PlayerManager.Instance.PlayerInput.SwitchCurrentActionMap(DEFAULT_ACTION_MAP);
        Time.timeScale = 1f;
        _isPaused = false;
    }


    public void ExitToMainMenu()
    {
        Unpause();

        GameManager.Instance.ReturnToMenu();
    }

    public void ExitToDesktop() => Application.Quit();
}
