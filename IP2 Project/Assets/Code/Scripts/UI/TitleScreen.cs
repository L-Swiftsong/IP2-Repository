using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TitleScreen : MonoBehaviour
{
    [Header("Other References")]
    [SerializeField] private RectTransform _titleText;
    
    [Space(5)]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _startOffset;
    [SerializeField] private float _endOffset;
    [SerializeField] private AnimationCurve _curve;


    [Header("Screen Object References")]
    [SerializeField] private GameObject _initialScreenGO;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _tutorialModalGO;


    private void Start()
    {
        // Ensure that the correct screen is showing.
        _initialScreenGO.SetActive(true);
        _mainMenuGO.SetActive(false);
        _tutorialModalGO.SetActive(false);

        // Ensure that the title is in the correct position.
        _titleText.anchoredPosition = new Vector2(0f, _startOffset);


        // Link the input to the ShowMainFromInitial function.
        StartCoroutine(AwaitInput(ShowMainFromInitial, initialDelay: 0.1f));
    }

    private IEnumerator AwaitInput(System.Action callback, float initialDelay = 0f, float inputDelay = 0f)
    {
        yield return new WaitForSeconds(initialDelay);

        // Listen for any button press.
        bool shouldContinue = false;
        InputSystem.onAnyButtonPress.CallOnce(ctrl => shouldContinue = true);
        yield return new WaitUntil(() => shouldContinue);

        yield return new WaitForSeconds(inputDelay);

        // Invoke the callback once any button has been pressed.
        callback.Invoke();
    }


    private void ShowMainFromInitial() => StartCoroutine(MoveTitleToTop());
    private IEnumerator MoveTitleToTop()
    {
        // Hide the Initial Screen.
        _initialScreenGO.SetActive(false);

        // Cache Values.
        float cachedAnchoredX = _titleText.anchoredPosition.x;

        // Lerp the title text towards its end position.
        float elapsedDuration = 0f;
        while(elapsedDuration < _animationDuration)
        {
            float lerpPercentage = Mathf.Clamp01(elapsedDuration / _animationDuration);
            float currentOffset = Mathf.Lerp(_startOffset, _endOffset, _curve.Evaluate(lerpPercentage));
            _titleText.anchoredPosition = new Vector2(cachedAnchoredX, currentOffset);

            yield return null;
            elapsedDuration += Time.deltaTime;
        }

        // Ensure that the title text ends in the end position.
        _titleText.anchoredPosition = new Vector2(cachedAnchoredX, _endOffset);

        // Show the Main Menu.
        _mainMenuGO.SetActive(true);
    }


    public void ShowTutorialModal()
    {
        _mainMenuGO.SetActive(false);
        _tutorialModalGO.SetActive(true);
    }
    public void HideTutorialModal()
    {
        _mainMenuGO.SetActive(true);
        _tutorialModalGO.SetActive(false);
    }



    public void StartTutorial() => GameManager.Instance.LoadTutorialSceneFromMenu();
    public void StartGame() => GameManager.Instance.LoadFirstSceneFromMenu();


    public void QuitGame() => Application.Quit();
}
