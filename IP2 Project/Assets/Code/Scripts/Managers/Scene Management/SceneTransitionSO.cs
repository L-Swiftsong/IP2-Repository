using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scene Transitions/New Transition")]
public class SceneTransitionSO : ScriptableObject
{
    [SerializeField] private SceneIndexes[] _scenesToUnload;
    [SerializeField] private SceneIndexes[] _scenesToLoad;

    //[Space(5)]
    //[SerializeField] private Vector2 _entrancePosition;
    //[SerializeField] private float _loadDelay;


    // Accessors.
    public SceneIndexes[] ScenesToUnload => _scenesToUnload;
    public SceneIndexes[] ScenesToLoad => _scenesToLoad;
}