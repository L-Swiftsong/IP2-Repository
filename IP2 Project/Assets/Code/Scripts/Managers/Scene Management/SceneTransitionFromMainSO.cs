using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Transitions/New Transition From Main Menu")]
public class SceneTransitionFromMainSO : ScriptableObject, ISceneTransition
{
    [Header("Scene Loading")]
    [SerializeField] private bool _loadPlayerScene = true;
    [SerializeField] private SceneIndexes _sceneToLoad;


    [Header("Player Spawning")]
    [SerializeField] private bool _useCustomEntrancePos = true;
    [SerializeField] private Vector2 _entrancePosition;


    // Accessors.
    public SceneIndexes[] ScenesToUnload => new SceneIndexes[1] { SceneIndexes.TITLE_SCREEN };
    public SceneIndexes[] ScenesToLoad
    {
        get
        {
            if (_loadPlayerScene)
                return new SceneIndexes[2] { SceneIndexes.PLAYER_SCENE, _sceneToLoad };
            else
                return new SceneIndexes[1] { _sceneToLoad };
        }
    }
    public SceneIndexes NewActiveScene => _sceneToLoad;

    public bool UseCustomEntrancePos => _useCustomEntrancePos;
    public Vector2 EntrancePosition => _entrancePosition;
}
