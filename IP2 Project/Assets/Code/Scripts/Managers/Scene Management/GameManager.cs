using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private bool _loadTitleOnAwake = true;

    private void Awake()
    {
        // Setup the Singleton Instance.
        Instance = this;


        // Initialise the loading bar.
        _loadingBar?.SetValues(0f, max: 100f, min: 0f);
        _loadingScreen.SetActive(false);


        if (_loadTitleOnAwake)
        {
            LoadInitialScenes();
        }
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


    [Header("Cameras")]
    [SerializeField] private Camera _mainCamera;
    public static Camera MainCamera
    {
        get
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("WARNING: Game Manager not initialised. Accessing camera inefficiently via Camera.main");
                return Camera.main;
            }
            else
                return GameManager.Instance._mainCamera;
        }
    }


    [Header("Scene Loading From Main Menu")]
    [SerializeField] private SceneTransitionFromMainSO _tutorialFromMMTransition;
    [SerializeField] private SceneTransitionFromMainSO _firstLevelFromMMTransition;


    [Header("Scene Loading")]
    [SerializeField] private GameObject _loadingScreen;
    private int _activeSceneBuildIndex;

    [Space(5)]
    [SerializeField] private ProgressBar _loadingBar;
    [SerializeField] private TMP_Text _loadingText;

    [Space(5)]
    [SerializeField] private GameObject _loadingProgressGO;
    [SerializeField] private GameObject _loadingCompletedGO;


    [Header("Game Pausing")]
    public static Action OnHaultLogic;
    public static Action OnResumeLogic;


    private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();

    [Header("Rendering")]
    private bool _useBloom = true;
    public bool UseBloom
    {
        get => _useBloom;
        set
        {
            _useBloom = value;
            OnPostProcessSettingsChanged?.Invoke();
        }
    }

    public static Action OnPostProcessSettingsChanged;



    #region Scene Loading
    private void LoadInitialScenes()
    {
        // Clear all scenes but the Persistent Scene.
        ClearActiveScenes();

        // Load the Title Screen additively.
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
        _activeSceneBuildIndex = (int)SceneIndexes.TITLE_SCREEN;
    }
    
    public void LoadTutorialSceneFromMenu() => CommenceTransition(_tutorialFromMMTransition);
    public void LoadFirstSceneFromMenu() => CommenceTransition(_firstLevelFromMMTransition);
    public void ReturnToMenu() => LoadInitialScenes();
    

    public void CommenceTransition(ISceneTransition transition)
    {
        LoadScenesAsync(
            scenesToUnload: transition.ScenesToUnload,
            scenesToLoad: transition.ScenesToLoad,
            newActiveScene: transition.NewActiveScene,
            playerSpawnPosition: transition.UseCustomEntrancePos ? transition.EntrancePosition : null);
    }

    public void LoadScenesAsync(SceneIndexes[] scenesToUnload, SceneIndexes[] scenesToLoad, SceneIndexes newActiveScene = SceneIndexes.PERSISTENT_SCENE, Vector2? playerSpawnPosition = null)
    {
        // Enable the Loading Screen.
        _loadingScreen.SetActive(true);

        // Show the progress bar and hide the loading completed text.
        _loadingProgressGO.SetActive(true);
        _loadingCompletedGO.SetActive(false);


        _activeSceneBuildIndex = (int)newActiveScene;

        // Start unloading & loading scenes.
        foreach (int sceneToUnload in scenesToUnload)
            _scenesLoading.Add(SceneManager.UnloadSceneAsync(sceneToUnload));
        foreach (int sceneToLoad in scenesToLoad)
            _scenesLoading.Add(SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive));

        // Show loading progress.
        StartCoroutine(GetSceneLoadProgress(playerSpawnPosition));
    }

    float _totalSceneProgress;
    private IEnumerator GetSceneLoadProgress(Vector2? playerSpawnPosition)
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
                _loadingText.text = string.Format("Loading - {0}%", _totalSceneProgress.ToString("#.0"));

                // Wait 1 frame between checks so as to not crash the game.
                yield return null;
            }
        }

        // --We have finished loading the scenes--
        float previousDeltaTime = Time.timeScale;
        Time.timeScale = 0f;


        // Set the player's position if the PlayerManager Singleton has been loaded (Therefore the PlayerScene should be loaded).
        if (PlayerManager.IsInitialised && playerSpawnPosition.HasValue)
        {
            PlayerManager.Instance.SetPlayerPosition(playerSpawnPosition.Value);
        }

        // Wait a half second to allow for initialisation of scripts.
        yield return new WaitForSecondsRealtime(0.5f);

        // Show the loading completed text.
        _loadingBar.SetValues(100f);
        _loadingProgressGO.SetActive(false);
        _loadingCompletedGO.SetActive(true);

        // Wait until the user presses a button to continue (Currently Any, but we could set it to specific buttons via a new InputActionMap).
        bool shouldContinue = false;
        InputSystem.onAnyButtonPress.CallOnce(ctrl => shouldContinue = true);
        yield return new WaitUntil(() => shouldContinue);

        // Hide the loading screen.
        _loadingScreen.SetActive(false);

        // Revert the timeScale.
        Time.timeScale = previousDeltaTime;

        // Allow the player to interact?
    }


    private void ClearActiveScenes()
    {
        // Unload all open scenes except the persistent scene.
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene sceneToUnload = SceneManager.GetSceneAt(i);
            int sceneBuildIndex = sceneToUnload.buildIndex;

            // Don't unload the persistent scene.
            if (sceneToUnload.IsValid() && sceneBuildIndex == (int)SceneIndexes.PERSISTENT_SCENE)
                continue;

            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }


    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode sceneLoadMode)
    {
        // Set this scene to be the active scene if it is the one we are looking to have set.
        if (loadedScene.buildIndex == _activeSceneBuildIndex)
            SceneManager.SetActiveScene(loadedScene);
    }
    #endregion



    public void PauseLogic()
    {
        // Revoke Player Control.
        PlayerManager.Instance.RevokePlayerControl();

        // Tell things like enemies to stop.
        OnHaultLogic?.Invoke();
    }
    public void ResumeLogic()
    {
        // Regain Player Control.
        PlayerManager.Instance.RegainPlayerControl();

        // Tell things like entities to resume.
        OnResumeLogic?.Invoke();
    }
}
