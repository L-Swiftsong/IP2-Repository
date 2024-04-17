using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Animations/New Animation")]
public class UIAnimation : ScriptableObject
{
    public Sprite[] Animation;
    public int FrameRate;
}
