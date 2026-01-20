using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerStateDriver : MonoBehaviour
{
    private PlayerInput playerInput;
    private StateMachineBuilder _machineBuilder;

    public PlayerContext ctx = new PlayerContext();

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundCheckLayerMask;

    [Header("State Machine")]
    public StateMachine _stateMachine;
    private State _rootState;
    private string _lastPath;

    [Header("Gizmos")]
    [SerializeField] private bool _isGizmosVisible = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ctx.rbPlayer = GetComponent<Rigidbody2D>();

        // Initialize State Machine
        _rootState = new RootState(_stateMachine, ctx);
        _machineBuilder = new StateMachineBuilder(_rootState);
        _stateMachine = _machineBuilder.Build();
        _stateMachine.Start();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ctx.isGrounded = GroundCheck();

        if (_stateMachine != null ) 
            _stateMachine.Tick(Time.deltaTime);

        ctx.UpdateInputs(playerInput);

        ctx.UpdateJumpTimers(Time.deltaTime);

        // Intents
        ctx.ResolveMoveIntent();
        ctx.ResolveJumpIntent();
        ctx.ResolveCrouchIntent();
        ctx.ResolveRunIntent();
        ctx.ResolveLocomotionIntenet();

        var path = StateToPath(_stateMachine.rootState.GetLeaf());

        if (path != _lastPath)
        {
            _lastPath = path;
            Debug.Log($"Path: {path}");
        }
    }

    private void FixedUpdate()
    {
        var velocity = ctx.rbPlayer.linearVelocity;
        velocity.x = ctx.velocity.x;
        ctx.rbPlayer.linearVelocity = velocity;

        ctx.velocity.x = ctx.rbPlayer.linearVelocity.x;
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(_groundCheckTransform.position, _groundCheckRadius, _groundCheckLayerMask);
    }

    private void OnDrawGizmos()
    {
        if (!_isGizmosVisible) return;

        if(_groundCheckTransform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_groundCheckTransform.position, _groundCheckRadius);
    }

    private string StateToPath(State s)
    {
        return string.Join(" > ", s.GetPathToRoot().Reverse().Select(n => n.GetType().Name));
    }
}

[Serializable]
public class PlayerContext
{
    [Header("Unity Scripts")]
    public Rigidbody2D rbPlayer;
    public CapsuleCollider2D colPlayer;

    [Header("Sprite")]
    public GameObject bodySprite;

    [Header("Ground Check")]
    public bool isGrounded;

    [Header("Movement")]
    public bool isMoveRequested;
    public Vector2 move;
    public Vector2 velocity;

    [Header("Walk Settings")]
    public float walkSpeed;
    public float walkAcceleration;
    public float walkDeceleration;

    [Header("Run Settings")]
    public bool isRunRequested;
    public bool isRunPressed;
    public bool isRunHolded;
    public bool isRunning;

    public float runSpeed;
    public float runAcceleration;
    public float runDeceleration;

    [Header("Crouch Settings")]
    public bool isCrouchRequested;
    public bool isCrouchPressed;
    public bool isCrouchToggle;
    public bool isCrouchHolded;
    public bool isCrouching;

    public float crouchSpeed;
    public float crouchAcceleration;
    public float crouchDeceleration;

    [Header("Jump")]
    // Inputs
    public bool isJumpRequested;
    public bool isJumpPressed;
    public bool isJumping;

    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.1f;

    public float jumpBufferCounter;
    public float coyoteCounter;

    public float jumpHeight;
    public float airSpeed;
    public float airAcceleration;
    public float airDeceleration;

    #region Inputs
    public void UpdateInputs(PlayerInput input)
    {
        move = input.MoveInput;
        isJumpPressed = input.IsJumpInput;
        isRunPressed = input.IsRunInputPressed;
        isRunHolded = input.IsRunInputHolded;
        isCrouchPressed = input.IsCrouchInputPressed;
        isCrouchHolded = input.IsCrouchInputHolded;
    }
    #endregion

    #region Move
    public void ResolveMoveIntent()
    {
        isMoveRequested = false;

        if (Mathf.Abs(move.x) > 0.01f || Mathf.Abs(rbPlayer.linearVelocityX) > 0.01f)
        {
            isMoveRequested = true;
        }
        else if (Mathf.Abs(rbPlayer.linearVelocityX) <= 0.01f && Mathf.Abs(move.x) <= 0.01f)
        {
            isMoveRequested = false;
        }
    }

    public void ResolveLocomotionIntenet()
    {
        if (isRunPressed)
        {
            isRunRequested = true;
            isCrouchRequested = false;
        }
        else if (isCrouchRequested)
        {
            isRunRequested = false;
        }
    }
    #endregion

    #region Jump
    public void UpdateJumpTimers(float dt)
    {
        if (isJumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= dt;
        }

        if(isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= dt;
        }
    }

    public void ResolveJumpIntent()
    {
        isJumpRequested = false;

        if (isJumping)
        {
            return;
        }

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            isJumpRequested = true;

            // Consume buffer
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }
    }

    public void Jump()
    {
        isJumping = true;
        var rb = rbPlayer;

        if (rb == null)
            return;

        rb.linearVelocityY = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * jumpHeight);
        rbPlayer.linearVelocityY = rb.linearVelocityY;
    }

    public void ResetJump()
    {
        isJumping = false;
    }
    #endregion

    #region Crouch
    public void ResolveCrouchIntent()
    {
        isCrouchRequested = isCrouchToggle
            ? isCrouching
            : isCrouchHolded;

        if (isCrouchToggle && isCrouchPressed)
        {
            isCrouchRequested = !isCrouching;
        }
    }

    public void SetCrouch(bool crouch)
    {
        if (isCrouching == crouch)
            return;

        isCrouching = crouch;

        if (crouch)
        {
            OnEnterCrouch();
        }
        else
        {
            OnExitCrouch();
        }
    }

    public void Crouch() => SetCrouch(true);
    public void StandUp() => SetCrouch(false);

    private void OnEnterCrouch()
    {
        colPlayer.size = new Vector2(colPlayer.size.x, 1.5f);
        colPlayer.offset = new Vector2(colPlayer.offset.x, -.25f);
        bodySprite.transform.localScale = new Vector3(bodySprite.transform.localScale.x, .75f, bodySprite.transform.localScale.z);
        bodySprite.transform.localPosition = new Vector3(bodySprite.transform.localPosition.x, -.25f, bodySprite.transform.localPosition.z);
    }

    private void OnExitCrouch()
    {
        colPlayer.size = new Vector2(colPlayer.size.x, 2f);
        colPlayer.offset = new Vector2(colPlayer.offset.x, 0f);
        bodySprite.transform.localScale = new Vector3(bodySprite.transform.localScale.x, 1f, bodySprite.transform.localScale.z);
        bodySprite.transform.localPosition = new Vector3(bodySprite.transform.localPosition.x, 0f, bodySprite.transform.localPosition.z);
    }
    #endregion

    #region Run
    public void ResolveRunIntent()
    {
        isRunRequested = false;

        if (isRunHolded)
        {
            isRunRequested = true;
        }
    }
    #endregion

    #region Calculations
    public void CalculateGroundedVelocity(float speed, float acceleration, float deceleration, float deltaTime)
    {
        var target = move.x * speed;

        bool accelerating = Mathf.Abs(target) > Mathf.Abs(velocity.x);
        float accel = accelerating 
            ? acceleration 
            : deceleration;

        velocity.x = Mathf.MoveTowards(velocity.x, target, accel * deltaTime);
    }

    public void CalculateAirVelocity(float speed, float acceleration, float deceleration, float deltaTime)
    {
        var target = move.x * speed;

        bool accelerating = Mathf.Abs(target) > Mathf.Abs(velocity.x);
        float accel = accelerating
            ? acceleration
            : deceleration;

        velocity.x = Mathf.MoveTowards(velocity.x, target, accel * deltaTime);
    }
    #endregion
}
