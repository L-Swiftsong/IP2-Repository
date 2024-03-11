using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Chosen Values")]
public class ChosenOptionsSO : ScriptableObject
{
    [Header("Difficulty")]
    public int MaxPlayerHealthMultiplier = 1;

    public int MaxEnemyHealthMultiplier = 1;


    [Header("Accessibility")]
    public FontType UIFont;
}

[System.Serializable]
public enum FontType
{
    DEFAULT = 0,
    TIMES_NEW_ROMAN = 1
}