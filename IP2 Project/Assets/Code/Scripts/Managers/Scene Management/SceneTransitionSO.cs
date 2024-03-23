using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Transitions/New Transition")]
public class SceneTransitionSO : ScriptableObject, ISceneTransition
{
    [Header("Scene Loading")]
    [SerializeField] private SceneIndexes[] _scenesToUnload;
    [SerializeField] private SceneIndexes[] _scenesToLoad;
    [SerializeField] private SceneIndexes _newActiveScene = SceneIndexes.PERSISTENT_SCENE;

    [Header("Player Spawning")]
    [SerializeField] private bool _useCustomEntrancePos = true;
    [SerializeField] private Vector2 _entrancePosition;


    // Accessors.
    public SceneIndexes[] ScenesToUnload => _scenesToUnload;
    public SceneIndexes[] ScenesToLoad => _scenesToLoad;
    public SceneIndexes NewActiveScene => _newActiveScene;

    public bool UseCustomEntrancePos => _useCustomEntrancePos;
    public Vector2 EntrancePosition => _entrancePosition;
}