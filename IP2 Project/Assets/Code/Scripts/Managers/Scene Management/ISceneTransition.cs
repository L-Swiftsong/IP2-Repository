using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneTransition
{
    public SceneIndexes[] ScenesToUnload { get; }
    public SceneIndexes[] ScenesToLoad { get; }
    public SceneIndexes NewActiveScene { get; }

    public bool UseCustomEntrancePos { get; }
    public Vector2 EntrancePosition { get; }
}
