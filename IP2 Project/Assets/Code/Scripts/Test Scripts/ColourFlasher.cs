using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourFlasher : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Space(5)]
    [SerializeField] private float _flashDuration;
    [SerializeField] private Color _defaultColour;
    [SerializeField] private Color _flashColour;

    private Coroutine _flashCoroutine;

    public void StartFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        StartCoroutine(Flash());
    }
    private IEnumerator Flash()
    {
        _spriteRenderer.color = _flashColour;
        yield return new WaitForSeconds(_flashDuration);
        _spriteRenderer.color = _defaultColour;
    }
}
