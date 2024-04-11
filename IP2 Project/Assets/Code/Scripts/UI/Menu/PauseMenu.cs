using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    private bool _isPaused;
    [SerializeField] private InputActionReference[] _pauseActions;

    private const string DEFAULT_ACTION_MAP = "Player";
    private const string PAUSE_MENU_ACTION_MAP = "UI";

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenuHolder;


    [Header("Menu Swapping")]
    [SerializeField] private GameObject _pauseMenuGO;
    [SerializeField] private GameObject _optionsMenuGO;


    [Header("Selection References")]
    [SerializeField] private GameObject _pauseMenuFirst;
    [SerializeField] private GameObject _optionsMenuFirst;


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

        // Select the first button of the pause menu.
        EventSystem.current.SetSelectedGameObject(_pauseMenuFirst);
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


    public void ShowOptionsMenu()
    {
        // Hide the Pause Menu.
        _pauseMenuGO.SetActive(false);

        // Show the Options Menu & Select the first button.
        _optionsMenuGO.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_optionsMenuFirst);
    }
    public void HideOptionsMenu()
    {
        // Hide the Options Menu.
        _optionsMenuGO.SetActive(false);

        // Show the Pause Menu & Select the first button.
        _pauseMenuGO.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_pauseMenuFirst);
    }


    public void ExitToMainMenu()
    {
        Unpause();

        GameManager.Instance.ReturnToMenu();
    }

    public void ExitToDesktop() => Application.Quit();
}
