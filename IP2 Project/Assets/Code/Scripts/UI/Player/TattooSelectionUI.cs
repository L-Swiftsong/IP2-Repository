using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TattooSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject _tattooSelectionRoot;
    [SerializeField] private List<TattooSelectionOption> _tattooSelectionButtons;



    private void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneLoaded;


    bool waitingToShow = false;
    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!waitingToShow)
            StartCoroutine(WaitToShow());
    }
    private IEnumerator WaitToShow()
    {
        waitingToShow = true;

        // Wait until the loading screen had been disabled.
        bool logicEnabled = false;
        GameManager.OnScenesLoaded += () => logicEnabled = true;
        yield return new WaitUntil(() => logicEnabled == true);

        // Show the screen.
        ShowScreen();
        waitingToShow = false;
    }

    public void ShowScreen()
    {
        // Pause Logic.
        GameManager.Instance.PauseLogic();

        // Show the UI.
        _tattooSelectionRoot.SetActive(true);

        // Initialise all the TattooSelectionOptions.
        for (int i = 0; i < _tattooSelectionButtons.Count; i++)
        {
            _tattooSelectionButtons[i].Init();
        }
    }

    public void SelectAbility(Ability ability)
    {
        // Set the player's ability.
        if (PlayerManager.Instance.Player.TryGetComponent<AbilityHolder>(out AbilityHolder playerAbilityHolder))
        {
            playerAbilityHolder.SetAbility(ability);
        }

        // Hide the screen
        HideScreen();
    }
    private void HideScreen()
    {
        // Hide the screen.
        _tattooSelectionRoot.SetActive(false);

        // Re-enable logic.
        GameManager.Instance.ResumeLogic();
    }
}
