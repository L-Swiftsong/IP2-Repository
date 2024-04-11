using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private ConversationSO _conversation;

    [Space(5)]
    [SerializeField] private LayerMask _triggeringLayers = 1 << 3; // Defaults: Player.
    [SerializeField] private bool _singleTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger for layers set to trigger.
        if (_triggeringLayers != (_triggeringLayers | (1 << collision.gameObject.layer)))
            return;


        if (DialogueManager.IsInitialised)
            DialogueManager.Instance.PlayConversation(_conversation);
        

        if (_singleTrigger)
            Destroy(this.gameObject);
    }
}
