using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IP2_Scripts
{
    [System.Serializable]
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TabGroup _tabGroup;
        private Image _backgroundImage;

        [Space(5)]
        [SerializeField] private GameObject _associatedSection;


        private void Awake()
        {
            _backgroundImage = GetComponent<Image>();
            _tabGroup.Subscribe(this);
        }


        public void OnPointerEnter(PointerEventData eventData) => _tabGroup.OnTabEnter(this);
        public void OnPointerExit(PointerEventData eventData) => _tabGroup.OnTabExit(this);
        public void OnPointerClick(PointerEventData eventData) => _tabGroup.OnTabSelected(this);


        public void SetBackgroundSprite(Sprite newSprite) => _backgroundImage.sprite = newSprite;
        public void SetBackgroundColor(Color newColour) => _backgroundImage.color = newColour;


        public GameObject GetAssociatedSection() => _associatedSection;
    }
}