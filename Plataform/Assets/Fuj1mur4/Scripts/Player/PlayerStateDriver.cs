using System;
using UnityEngine;

public class PlayerStateDriver : MonoBehaviour
{
    public PlayerContext PlayerContext = new PlayerContext();

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundCheckLayerMask;

    [Header("State Machine")]
    private StateMachine _stateMachine;
    private State _rootState;
    private string _lastPath;

    [Header("Gizmos")]
    [SerializeField] private bool _isGizmosVisible = true;


    private void Awake()
    {
        _rootState = new RootState(_stateMachine, PlayerContext);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerContext.IsGrounded = GroundCheck();

        if (_stateMachine != null ) 
            _stateMachine.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        
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

}

[Serializable]
public class PlayerContext
{
    [Header("Unity Scripts")]
    public Rigidbody2D rigidbody;

    [Header("Ground Check")]
    public bool IsGrounded;

    [Header("Movement")]
    public Vector2 MoveDirection;
    public Vector2 Velocity;
    public float WalkSpeed;

    [Header("Jump")]
    public bool IsJumpPressed;
    public float JumpSpeed;
    
}
