using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElevatorNotification : MonoBehaviour
{
    [SerializeField] private GameObject _notificationRoot;
    [SerializeField] private TMP_Text _notificationText;

    [Space(5)]
    [SerializeField] private float _notificationDuration;
    private Coroutine _hideCoroutine;

    private const string DEFAULT_NOTIFICATION_MESSAGE = "ACCESS DENIED\n{0} more kills required";

    private void Start() => _notificationRoot.SetActive(false);

    private void OnEnable() => ElevatorManager.OnFailedPress += ShowNotification;
    private void OnDisable() => ElevatorManager.OnFailedPress -= ShowNotification;


    private void ShowNotification(int killsRequired)
    {
        _notificationRoot.SetActive(true);
        _notificationText.text = string.Format(DEFAULT_NOTIFICATION_MESSAGE, killsRequired);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);
        _hideCoroutine = StartCoroutine(HideAfterDuration());
    }
    private IEnumerator HideAfterDuration()
    {
        yield return new WaitForSeconds(_notificationDuration);
        _notificationRoot.SetActive(false);
    }
}
