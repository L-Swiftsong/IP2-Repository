using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    [SerializeField] private Sprite _fullHeartSprite, _halfHeartSprite, _emptyHeartSprite;

    [Space(5)]
    [SerializeField] private Image _heartImage;


    public void SetHeartStatus(HeartStatus newStatus)
    {
        switch (newStatus)
        {
            case HeartStatus.Empty:
                _heartImage.sprite = _emptyHeartSprite;
                break;
            case HeartStatus.Half:
                _heartImage.sprite = _halfHeartSprite;
                break;
            case HeartStatus.Full:
                _heartImage.sprite = _fullHeartSprite;
                break;
        }
    }
}

public enum HeartStatus { Empty, Half, Full }