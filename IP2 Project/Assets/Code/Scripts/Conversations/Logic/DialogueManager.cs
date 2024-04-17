using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public static bool IsInitialised => Instance != null;

    // Setup the Singleton Instance.
    private void Awake() => Instance = this;


    [SerializeField] private GameObject _conersationUIRoot;
    [SerializeField] private ConversationUI _conversationUI;

    private ConversationSO _currentConversation;
    private int _dialogueIndex;


    private void Start()
    {
        // Hide the conversation UI.
        _conersationUIRoot.SetActive(false);
    }
    


#if UNITY_EDITOR
    [SerializeField] private ConversationSO _testConversation;

    [ContextMenu("Testing/Play Test Conversation")]
    private void PlayTestConversation() => PlayConversation(_testConversation);
#endif

    public void PlayConversation(ConversationSO conversation)
    {
        _currentConversation = conversation;

        StartCoroutine(HandleConversation());
    }
    private IEnumerator HandleConversation()
    {
        // Diable Other Logic.
        GameManager.Instance.PauseLogic();

        // Show the dialogue UI.
        _conersationUIRoot.SetActive(true);


        // Find the first dialogue to display on the left & the first on the right.
        Dialogue? firstLeft = _currentConversation.Conversation.FirstOrDefault(t => t.DisplayOnLeft);
        Dialogue? firstRight = _currentConversation.Conversation.FirstOrDefault(t => !t.DisplayOnLeft);

        _conversationUI.InitialiseConversation(
            firstLeft: firstLeft,
            firstRight: firstRight,
            firstIsLeft: _currentConversation.Conversation[0].DisplayOnLeft
        );


        _dialogueIndex = 0;
        int conversationLength = _currentConversation.Conversation.Length;
        while(_dialogueIndex < conversationLength)
        {
            // Instruct the UI to update.
            _conversationUI.DisplayDialogue(_currentConversation.Conversation[_dialogueIndex]);


            // Wait until the current text has completed.
            yield return new WaitUntil(() => _conversationUI.HasCompletedCurrentText || CheckForValidKey());
            _conversationUI.SkipDialogue();

            // Wait a frame to prevent skipping twice.
            yield return null;

            
            // Wait until we press a button.
            yield return new WaitUntil(() => CheckForValidKey());
            
            // Wait a frame to prevent skipping twice.
            yield return null;


            // Increment the conversation.
            _dialogueIndex++;
        }


        // Hide the dialogue UI.
        _conersationUIRoot.SetActive(false);
        _currentConversation = null;

        // Enable Other Logic.
        GameManager.Instance.ResumeLogic();
    }

    private bool CheckForValidKey()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) // Keyboard (Any Key).
        {
            return true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) // Mouse (Left Click).
        {
            return true;
        }
        else if (Gamepad.current != null && Gamepad.current.allControls.Any(x => x is ButtonControl button && button.wasPressedThisFrame && !x.synthetic)) // Gamepad (Any Button, NOT Sticks).
        {
            return true;
        }

        // No valid key was pressed this frame.
        return false;
    }
}
