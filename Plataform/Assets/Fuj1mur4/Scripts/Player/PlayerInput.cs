using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, PlayerControls.IPlayerMapActions
{
    public PlayerControls inputMap;

    [Header("Move Inputs")]

    [SerializeField] private Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    [Header("Jump Inputs")]
    [SerializeField] private bool _isJumpInputPressed;
    public bool IsJumpInput => _isJumpInputPressed;

    [Header("Sprint Inputs")]
    [SerializeField] private bool _isRunInputHolded;
    [SerializeField] private bool _isRunInputPressed;
    public bool IsRunInputPressed => _isRunInputPressed;
    public bool IsRunInputHolded => _isRunInputHolded;

    [Header("Crouch Inputs")]
    [SerializeField] private bool _isCrouchInputHolded;
    [SerializeField] private bool _isCrouchInputPressed;
    public bool IsCrouchInputPressed => _isCrouchInputPressed;
    public bool IsCrouchInputHolded => _isCrouchInputHolded;


    private void Awake()
    {
        inputMap = new PlayerControls();
        inputMap.Enable();

        inputMap.PlayerMap.Enable();
        inputMap.PlayerMap.SetCallbacks(this);

    }

    private void OnDisable()
    {
        inputMap.PlayerMap.RemoveCallbacks(this);
        inputMap.PlayerMap.Disable();
    }

    private void LateUpdate()
    {
        _isCrouchInputPressed = false;
        _isJumpInputPressed = false;
        _isRunInputPressed = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isJumpInputPressed = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isRunInputPressed = true;
        }

        _isRunInputHolded = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCrouchInputPressed = true;
        }

        _isCrouchInputHolded = context.ReadValueAsButton();

        return;
    }
}
