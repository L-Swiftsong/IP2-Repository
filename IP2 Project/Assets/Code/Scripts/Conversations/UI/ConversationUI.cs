using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationUI : MonoBehaviour
{
    private bool _hasCompletedCurrentText;
    public bool HasCompletedCurrentText => _hasCompletedCurrentText;

    private string _currentTargetText;
    private Coroutine _writeTextCoroutine;

    
    [Header("Left")]
    [SerializeField] private GameObject _leftSpeakerArea;
    [Space(5)]

    [SerializeField] private TMP_Text _leftSpeakerName;
    [SerializeField] private Image _leftSpeakerImage;
    [SerializeField] private GameObject _leftSpeakerBlur;

    [Header("Right")]
    [SerializeField] private GameObject _rightSpeakerArea;

    [Space(5)]
    [SerializeField] private TMP_Text _rightSpeakerName;
    [SerializeField] private Image _rightSpeakerImage;
    [SerializeField] private GameObject _rightSpeakerBlur;


    [Header("Middle")]
    [SerializeField] private TMP_Text _mainBodyText;
    [SerializeField] private int _mainBodyCharactersPerSecond;


    public void InitialiseConversation(Dialogue? firstLeft, Dialogue? firstRight, bool firstIsLeft)
    {
        // Set the speaker names & sprites.
        if (firstLeft.HasValue)
        {
            _leftSpeakerArea.SetActive(true);

            _leftSpeakerImage.sprite = firstLeft.Value.SpeakerSprite;
            _leftSpeakerName.text = firstLeft.Value.SpeakerName;
        }
        else
            _leftSpeakerArea.SetActive(false);

        if (firstRight.HasValue)
        {
            _rightSpeakerArea.SetActive(true);

            _rightSpeakerImage.sprite = firstRight.Value.SpeakerSprite;
            _rightSpeakerName.text = firstRight.Value.SpeakerName;
        }
        else
            _rightSpeakerArea.SetActive(false);


        // Set the first to blur.
        _leftSpeakerBlur.SetActive(firstIsLeft);
        _rightSpeakerBlur.SetActive(!firstIsLeft);
    }
    public void DisplayDialogue(Dialogue dialogue)
    {
        // Stop previous text writing,
        if (_writeTextCoroutine != null)
            StopCoroutine(_writeTextCoroutine);

        
        // Update speaker.
        if (dialogue.DisplayOnLeft)
        {
            // Set the speaker name & sprite.
            _leftSpeakerName.text = dialogue.SpeakerName;
            _leftSpeakerImage.sprite = dialogue.SpeakerSprite;

            // Enable & Disable Blur.
            _leftSpeakerBlur.SetActive(false);
            _rightSpeakerBlur.SetActive(true);
        }
        else
        {
            // Set the speaker name & sprite.
            _rightSpeakerName.text = dialogue.SpeakerName;
            _rightSpeakerImage.sprite = dialogue.SpeakerSprite;

            // Enable & Disable Blur.
            _rightSpeakerBlur.SetActive(false);
            _leftSpeakerBlur.SetActive(true);
        }


        // Display the MainBodyText.
        _currentTargetText = dialogue.Text;
        _writeTextCoroutine = StartCoroutine(WriteText());
    }

    private IEnumerator WriteText()
    {
        // Reset the Main Body Text.
        _mainBodyText.text = "";
        _hasCompletedCurrentText = false;

        // Write the text one letter at a time.
        int textIndex = 0;
        int textLength = _currentTargetText.Length;

        float characterWriteDelay = 1f / _mainBodyCharactersPerSecond;
        while(textIndex < textLength)
        {
            // Add the character at index textIndex to the mainBodyText.
            _mainBodyText.text += _currentTargetText[textIndex];
            
            yield return new WaitForSeconds(characterWriteDelay);
            textIndex++;
        }

        // Finish writing the dialogue.
        FinishDialogue();
    }
    public void SkipDialogue()
    {
        // Stop the text writing.
        if (_writeTextCoroutine != null)
            StopCoroutine(_writeTextCoroutine);

        // Finish the dialogue
        FinishDialogue();
    }


    private void FinishDialogue()
    {
        // Ensure that all the text has been written.
        _mainBodyText.text = _currentTargetText;
        _hasCompletedCurrentText = true;
    }
}