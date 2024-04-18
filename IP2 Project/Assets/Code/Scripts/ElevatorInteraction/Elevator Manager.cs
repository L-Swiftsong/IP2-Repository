using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class ElevatorManager : MonoBehaviour
{
    [Header("Elevators")]
    [SerializeField] private List<EnterElevator> _elevators;
    [SerializeField] private Animator[] _elevatorAnims;

    // References
    private GameObject _elevatorUI;
    private GameObject _playerCanvas;
    private GameObject _player;
    private PlayerCombo _playerCombo;


    // Constant Values.
    private const int FLOOR_TWO_KILLS_REQUIRED = 7;
    private const int FLOOR_THREE_KILLS_REQUIRED = 29;
    private const int FLOOR_FOUR_KILLS_REQUIRED = 35;


    public static System.Action<int> OnFailedPress;

    private void Start()
    {
        // Get Player Components.
        _player = PlayerManager.Instance.Player;
        _playerCombo = _player.GetComponent<PlayerCombo>();

        // Get Canvas Components.
        _playerCanvas = PlayerManager.Instance.PlayerCanvas;
        _elevatorUI = _playerCanvas.GetComponentInChildren<ElevatorButtons>(includeInactive: true).gameObject;
    }

    private void Update()
    {
        Floor1();
        Floor2();
        Floor3();
        Floor4();

        // Note: Inefficient, but it should work.
        if (_elevators.Any(t => t.Entered))
            ActivateTrigger(true);
        else
            ActivateTrigger(false);
    }
    private void ActivateTrigger(bool value)
    {
        for (int i = 0; i < _elevatorAnims.Length; i++)
            _elevatorAnims[i].SetBool("IsOpen", value);
    }


    public void OnFPressed(InputAction.CallbackContext context)
    {
        if(_elevators.Any(t => t.Entered))
        {
            // Show the UI.
            _elevatorUI.gameObject.SetActive(true);

            // Pause Logic.
            GameManager.Instance.PauseLogic();
        }
    }

    public void Floor1()
    {
        if (_elevatorUI.GetComponent<ElevatorButtons>().button1Pressed == true)
        {
            // Move the player.
            _player.transform.position = _elevators[0].transform.position;
            
            // Disable the UI.
            _elevatorUI.SetActive(false);
            _elevatorUI.GetComponent<ElevatorButtons>().button1Pressed = false;

            // Enable Input.
            GameManager.Instance.ResumeLogic();
        }
    }


    public void Floor2()
    {
        if (_elevatorUI.GetComponent<ElevatorButtons>().button2Pressed == true)
        {
            // Check if the player has enough kills.
            if (_playerCombo.kills >= FLOOR_TWO_KILLS_REQUIRED)
            {
                // Move the player.
                _player.transform.position = _elevators[1].transform.position;
            }
            else
            {
                FailedPress(FLOOR_TWO_KILLS_REQUIRED);
            }

            // Disable the UI.
            _elevatorUI.GetComponent<ElevatorButtons>().button2Pressed = false;
            _elevatorUI.SetActive(false);

            // Enable Input.
            GameManager.Instance.ResumeLogic();
        }
    }

    public void Floor3()
    {
        if (_elevatorUI.GetComponent<ElevatorButtons>().button3Pressed == true)
        {
            // Check if the player has enough kills.
            if (_playerCombo.kills >= FLOOR_THREE_KILLS_REQUIRED)
            {
                // Move the player.
                _player.transform.position = _elevators[2].transform.position;
            }
            else
            {
                FailedPress(FLOOR_THREE_KILLS_REQUIRED);
            }

            // Disable the UI.
            _elevatorUI.GetComponent<ElevatorButtons>().button3Pressed = false;
            _elevatorUI.SetActive(false);

            // Enable Input.
            GameManager.Instance.ResumeLogic();
        }
    }

    public void Floor4()
    {
        if(_elevatorUI.GetComponent<ElevatorButtons>().button4Pressed == true)
        {
            // Check if the player has enough kills.
            if (_playerCombo.kills >= FLOOR_FOUR_KILLS_REQUIRED)
            {
                // Move the player.
                _player.transform.position = _elevators[3].transform.position;
            }
            else
            {
                FailedPress(FLOOR_FOUR_KILLS_REQUIRED);
            }

            // Disable the UI.
            _elevatorUI.SetActive(false);
            _elevatorUI.GetComponent<ElevatorButtons>().button4Pressed = false;

            // Enable Input.
            GameManager.Instance.ResumeLogic();
        }
    }


    private void FailedPress(int killsRequired)
    {
        // Notify the UI as for how many kills you require.
        int killsTillNextLevel = killsRequired - _playerCombo.kills;
        OnFailedPress?.Invoke(killsTillNextLevel);
    }
}