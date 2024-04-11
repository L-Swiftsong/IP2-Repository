using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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


        [Header("Input System Variables")]
        [SerializeField] private bool _useInputSwitching;
        [SerializeField] private InputActionReference _selectNextTabAction;
        [SerializeField] private InputActionReference _selectPreviousTabAction;


        private void Awake() => InitialiseTabs();
        private void OnEnable()
        {
            // If we have no selected button, select the first button.
            if (_selectedTab == null)
                OnTabSelected(_tabButtons[0]);
            // Otherwise, ensure that the selected tab is selected.
            else
                OnTabSelected(_selectedTab);


            Debug.Log("Enabled");
            if (_selectNextTabAction != null)
                _selectNextTabAction.action.performed += OnSelectNextTabPressed;
            if (_selectPreviousTabAction != null)
                _selectPreviousTabAction.action.performed += OnSelectPreviousTabPressed;
        }
        private void OnDisable()
        {
            if (_selectNextTabAction != null)
                _selectNextTabAction.action.performed -= OnSelectNextTabPressed;
            if (_selectPreviousTabAction != null)
                _selectPreviousTabAction.action.performed -= OnSelectPreviousTabPressed;
        }
        #region Input
        private void OnSelectNextTabPressed(InputAction.CallbackContext context)
        {
            if (context.performed && _useInputSwitching)
                SelectNextTab();
        }
        private void OnSelectPreviousTabPressed(InputAction.CallbackContext context)
        {
            if (context.performed && _useInputSwitching)
                SelectPreviousTab();
        }
        #endregion


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
                if (tab == button)
                {
                    tab.GetAssociatedSection().SetActive(true);
                    EventSystem.current.SetSelectedGameObject(tab.GetFirstSelectedItem());
                }
                else
                {
                    tab.GetAssociatedSection().SetActive(false);
                }
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



        private void SelectNextTab()
        {
            int selectedIndex = _tabButtons.FindIndex(t => t == _selectedTab);

            if (selectedIndex < _tabButtons.Count - 1)
                OnTabSelected(_tabButtons[selectedIndex + 1]);
            else
                OnTabSelected(_tabButtons[0]);
        }
        private void SelectPreviousTab()
        {
            int selectedIndex = _tabButtons.FindIndex(t => t == _selectedTab);

            if (selectedIndex > 0)
                OnTabSelected(_tabButtons[selectedIndex - 1]);
            else
                OnTabSelected(_tabButtons[_tabButtons.Count - 1]);
        }
    }
}