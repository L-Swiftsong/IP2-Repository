using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /*// Instance Reference.
    private static UIManager s_instance;
    public static UIManager GetInstance()
    {
        if (s_instance == null)
            s_instance = new UIManager();

        return s_instance;
    }*/
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("ERROR: There are multiple instances of the UIManager class active in the hierarchy");
    }

    // A private constructor to prevent creation from other scripts.
    private UIManager() { }
    ~UIManager()
    {
        if (Instance = this)
            Instance = null;
    }


    [SerializeField] private GameObject _player;
    public GameObject GetPlayerObject() => _player;


    [SerializeField] private Transform _healthBarParent;
    public void CreateBossHealthBar(HealthComponent connectedComponent, GameObject customBarPrefab)
    {
        // Ensure that healthBarParent exists, setting ourself to it if it doesn't already.
        if (_healthBarParent == null)
        {
            Debug.LogError("WARNING: HealthBarParent was unassigned. Defaulting to using this UIManager instance instead");
            _healthBarParent = this.transform;
        }


        // Create an instance for the boss's health bar.
        HealthBar bossBarInstance;
        if (customBarPrefab != null) // Custom Bar.
            bossBarInstance = Instantiate<GameObject>(customBarPrefab, _healthBarParent).GetComponent<HealthBar>();
        else // Default LinearProgressBar.
        {
            // Instantiate the Boss's Health Bar from the base LinearProgressBar, adding a HealthBar component to it.
            GameObject linearProgressBar = Instantiate<GameObject>(Resources.Load<GameObject>("UI/Linear Progress Bar"), _healthBarParent);
            bossBarInstance = linearProgressBar.AddComponent<HealthBar>();
            bossBarInstance.Init();
        }

        // Connect the Boss's Health Bar to the passed in healthComponent's events.
        connectedComponent.OnHealthChanged.AddListener(bossBarInstance.UpdateHealth);
        connectedComponent.OnDeath.AddListener(bossBarInstance.OnDead);
    }
}
