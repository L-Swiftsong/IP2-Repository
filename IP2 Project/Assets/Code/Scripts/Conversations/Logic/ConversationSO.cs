using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Conversation", order = 0)]
public class ConversationSO : ScriptableObject
{
    [SerializeField] private Dialogue[] _conversation;


    // Accessors.
    public Dialogue[] Conversation => _conversation;
}

[System.Serializable]
public struct Dialogue
{
    public string SpeakerName;
    public Sprite SpeakerImage;
    [TextArea(minLines: 3, maxLines: 10)] public string Text;

    [Space(5)]
    public bool DisplayOnLeft;
}