using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IP2_Scripts
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> _tabButtons;
        private TabButton _selectedTab;
        

        [Header("Tab Selection Variables")]
        [SerializeField] private bool _useSprites;

        [Space(5)]
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _hoveredSprite;
        [SerializeField] private Sprite _selectedSprite;

        [Space(5)]
        [SerializeField] private Color _idleColor = Color.white;
        [SerializeField] private Color _hoveredColor = Color.grey;
        [SerializeField] private Color _selectedColor = Color.red;



        private void Awake() => InitialiseTabs();
        private void OnEnable()
        {
            // If we have no selected button, select the first button.
            if (_selectedTab == null)
                OnTabSelected(_tabButtons[0]);
            // Otherwise, ensure that the selected tab is selected.
            else
                OnTabSelected(_selectedTab);
        }

        private void InitialiseTabs()
        {
            foreach (TabButton button in _tabButtons)
                button.SetParentGroup(this);
        }



        public void OnTabEnter(TabButton button)
        {
            ResetTabs();

            // If we don't have a selected tab OR the selected tab is not this button, set the tab to the hovered sprite/color.
            if (_selectedTab == null || _selectedTab != button)
                UpdateTab(button, _hoveredSprite, _hoveredColor);
        }
        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }
        public void OnTabSelected(TabButton button)
        {
            // Select this tab.
            _selectedTab = button;

            ResetTabs();

            // Set the tab to the selected sprite.color
            UpdateTab(button, _selectedSprite, _selectedColor);


            // Select this tab.
            foreach (TabButton tab in _tabButtons)
            {
                tab.GetAssociatedSection().SetActive(tab == button);
            }
        }

        public void ResetTabs()
        {
            foreach (TabButton button in _tabButtons)
            {
                if (button == _selectedTab)
                    continue;
                
                UpdateTab(button, _idleSprite, _idleColor);
            }
        }

        private void UpdateTab(TabButton button, Sprite sprite, Color colour)
        {
            if (_useSprites)
                button.SetBackgroundSprite(sprite);
            else
                button.SetBackgroundColor(colour);
        }
    }
}