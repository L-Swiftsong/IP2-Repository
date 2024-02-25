using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(menuName = "Animations/Animation Container")]
public class AnimationContainerSO : ScriptableObject
{
    [SerializedDictionary(keyName: "Entity Type", valueName: "Animation")]
    public SerializedDictionary<EntityType, Sprite[]> PairedAnimations;
}
