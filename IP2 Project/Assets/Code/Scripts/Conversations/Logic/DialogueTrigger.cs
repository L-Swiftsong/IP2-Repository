using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private ConversationSO _conversation;
    private bool _canTrigger = false;

    [Space(5)]
    [SerializeField] private LayerMask _triggeringLayers = 1 << 3; // Defaults: Player.
    [SerializeField] private bool _singleTrigger = true;


    #region Trigger Prevention while Paused
    // Prevent triggering while the game is paused (Or the scene is loading).
    private void OnEnable()
    {
        GameManager.OnHaultLogic += PreventTrigger;
        GameManager.OnResumeLogic += EnableTrigger;
    }
    private void OnDisable()
    {
        GameManager.OnHaultLogic -= PreventTrigger;
        GameManager.OnResumeLogic -= EnableTrigger;
    }

    private void PreventTrigger() => _canTrigger = false;
    private void EnableTrigger() => _canTrigger = true;
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger if we canTrigger.
        if (!_canTrigger)
            return;

        // Only trigger for layers set to trigger.
        if (_triggeringLayers != (_triggeringLayers | (1 << collision.gameObject.layer)))
            return;


        if (DialogueManager.IsInitialised)
            DialogueManager.Instance.PlayConversation(_conversation);
        

        if (_singleTrigger)
            Destroy(this.gameObject);
    }
}
