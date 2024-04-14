using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private GameObject _respawnPanel;
    [SerializeField] private GameObject _firstSelectedButton;

    [Space(5)]
    [SerializeField] private CanvasGroup _panelGroup;
    [SerializeField] private float _fadeDuration;


    private void Awake() => _respawnPanel.SetActive(false);
    private void OnEnable() => PlayerRespawning.OnRespawnReady += OpenRespawnUI;
    private void OnDisable() => PlayerRespawning.OnRespawnReady -= OpenRespawnUI;


    public void OpenRespawnUI()
    {
        // Show the UI Panel.
        _respawnPanel.SetActive(true);

        // Select the first button.
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);

        // Fade in the UI over time.
        StartCoroutine(FadeInUI(1f / _fadeDuration));
    }
    private IEnumerator FadeInUI(float fadeRate)
    {
        // Ensure we start completely transparent.
        _panelGroup.alpha = 0;

        // Fade in over time.
        while(_panelGroup.alpha < 1)
        {
            _panelGroup.alpha += fadeRate * Time.deltaTime;
            yield return null;
        }

        // Ensure we end completely opaque.
        _panelGroup.alpha = 1;
    }


    public void Respawn() => GameManager.Instance.RespawnPlayer();
    public void ExitToMenu() => GameManager.Instance.ReturnToMenu();
    public void ExitToDesktop() => Application.Quit();
}
