using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TattooSelectionOption : MonoBehaviour
{
    [SerializeField] private TattooSelectionUI _parentUI;
    [SerializeField] private Ability _relatedAbility;

    [Header("UI")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _abilityImage;
    [SerializeField] private TMP_Text _descriptionText;


    private void Start() => Init();
    public void Init()
    {
        // Setup the UI.
        _nameText.text = _relatedAbility.name;
        _abilityImage.sprite = _relatedAbility.AbilitySprite;
        _descriptionText.text = _relatedAbility.Description;
    }


    public void OnSelected() => _parentUI.SelectAbility(_relatedAbility);
}
