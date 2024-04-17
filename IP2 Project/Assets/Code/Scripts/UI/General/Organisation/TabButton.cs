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
        private TabGroup _parentTabGroup;
        private Image _backgroundImage;
        private bool _initialised = false;

        [Space(5)]
        [SerializeField] private GameObject _associatedSection;
        [SerializeField] private GameObject _firstSelectedItem;



        public void SetParentGroup(TabGroup group)
        {
            _parentTabGroup = group;
            Initialise();
        }
        private void Initialise()
        {
            if (_initialised)
                return;

            _backgroundImage = GetComponent<Image>();
            _initialised = true;
        }


        public void OnPointerEnter(PointerEventData eventData) => _parentTabGroup.OnTabEnter(this);
        public void OnPointerExit(PointerEventData eventData) => _parentTabGroup.OnTabExit(this);
        public void OnPointerClick(PointerEventData eventData) => _parentTabGroup.OnTabSelected(this);


        public void SetBackgroundSprite(Sprite newSprite) => _backgroundImage.sprite = newSprite;
        public void SetBackgroundColor(Color newColour) => _backgroundImage.color = newColour;
        

        public GameObject GetAssociatedSection() => _associatedSection;
        public GameObject GetFirstSelectedItem() => _firstSelectedItem;
    }
}