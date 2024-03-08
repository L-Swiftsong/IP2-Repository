using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        // Setup the Singleton Instance.
        Instance = this;


        // Initialise the loading bar.
        _loadingBar?.SetValues(0f, max: 100f, min: 0f);
        _loadingScreen.SetActive(false);

        // Load the Title Screen additively.
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
    }


    [Header("Scene Loading")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private ProgressBar _loadingBar;


    private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();


    public void LoadTutorialSceneFromMenu()
    {
        // Unload the Main Menu & Load the Tutorial Scene.
        LoadScenesAsync(
            scenesToUnload: new int[] { (int)SceneIndexes.TITLE_SCREEN },
            scenesToLoad: new int[] { (int)SceneIndexes.TUTORIAL_SCENE });
    }
    public void LoadFirstSceneFromMenu()
    {
        // Unload the Main Menu & Load the First Scene.
        LoadScenesAsync(
            scenesToUnload: new int[] { (int)SceneIndexes.TITLE_SCREEN },
            scenesToLoad: new int[] { (int)SceneIndexes.FIRST_LEVEL });
    }
    public void LoadScenesAsync(int[] scenesToUnload, int[] scenesToLoad)
    {
        // Enable the Loading Screen.
        _loadingScreen.SetActive(true);


        // Start unloading & loading scenes.
        foreach (int sceneToUnload in scenesToUnload)
            _scenesLoading.Add(SceneManager.UnloadSceneAsync(sceneToUnload));
        foreach (int sceneToLoad in scenesToLoad)
            _scenesLoading.Add(SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive));


        // Show loading progress.
        StartCoroutine(GetSceneLoadProgress());
    }

    float _totalSceneProgress;
    private IEnumerator GetSceneLoadProgress()
    {
        // Loop through each async operation.
        for (int i = 0; i < _scenesLoading.Count; i++)
        {
            // Loop until the Async Operation has completed.
            while (!_scenesLoading[i].isDone)
            {
                // Reset the total scene progress.
                _totalSceneProgress = 0f;

                // Loop through each operation to get their progress
                foreach (AsyncOperation operation in _scenesLoading)
                {
                    _totalSceneProgress += operation.progress;
                }

                // Calculate the total percentage progress of us loading the scenes.
                _totalSceneProgress = (_totalSceneProgress / _scenesLoading.Count) * 100f;
                _loadingBar?.SetValues(_totalSceneProgress);

                // Wait 1 frame between checks so as to not crash the game.
                yield return null;
            }
        }

        // We have finished loading the scenes, but wait until the user presses a button to continue (Currently Any, but we could set it to specific buttons via a new InputActionMap).
        bool shouldContinue = false;
        InputSystem.onAnyButtonPress.CallOnce(ctrl => shouldContinue = true);
        yield return new WaitUntil(() => shouldContinue);

        // Hide the loading screen.
        _loadingScreen.SetActive(false);

        // Allow player control (How will we do this from another scene?).
    }
}
