using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Speed statistics")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [ReadOnlyInspector] public float currentSpeed;

    private CharacterController _characterController;
    private CinemachineCamera _cinemachineCamera;
    private Vector2 _move;
    [ReadOnlyInspector] public bool _isWalking;
    [ReadOnlyInspector] public bool _isSprinting;

    [Header("Jump statistics")]
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField][ReadOnlyInspector] private float _gravityForce = -15f;
    private Vector3 _velocity;

    [Header("Jump Enhancements")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _jumpPressed;
    private bool _isJumping;

    [Header("Ground detection")]
    [SerializeField] private Transform _checkerPosition;
    [SerializeField] private float _checkRadius = 0.4f;
    [SerializeField] private LayerMask _groundLayers;
    [ReadOnlyInspector] public bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        currentSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue inputValue)
    {
        if (inputValue.Get<Vector2>() != Vector2.zero)
        {
            _move = inputValue.Get<Vector2>();
            currentSpeed = walkSpeed;
            _isWalking = true;
        }
        else
        {
            _move = Vector2.zero;
            currentSpeed = 0;
            _isWalking = false;
        }
    }

    public void OnSprint(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            currentSpeed = sprintSpeed;
            _isSprinting = true;
        }
        else
        {
            currentSpeed = walkSpeed;
            _isSprinting = false;
        }
    }

    public void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            _jumpPressed = true;
            _jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            _jumpPressed = false;
        }
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleJumpLogic();
        ApplyGravity();
        HandleMovement();
    }

    private void HandleGroundCheck()
    {
        _isGrounded = Physics.CheckSphere(_checkerPosition.position, _checkRadius, _groundLayers);

        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        if (_isGrounded && !_jumpPressed)
        {
            _jumpBufferCounter = 0;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpLogic()
    {
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0 && !_isJumping)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravityForce);
            _jumpBufferCounter = 0;
            _coyoteTimeCounter = 0;
            _isJumping = true;
        }
    }

    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _velocity.y += _gravityForce * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float speedMultiplier = _isGrounded ? 1f : airControl;

        Vector3 horizontalMovement = (GetForward() * _move.y + GetRight() * _move.x) * currentSpeed * speedMultiplier;

        Vector3 finalMovement = new Vector3(horizontalMovement.x, _velocity.y, horizontalMovement.z);

        _characterController.Move(finalMovement * Time.deltaTime);

    }

    public Vector3 GetForward()
    {
        Vector3 forward = _cinemachineCamera.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    public Vector3 GetRight()
    {
        Vector3 right = _cinemachineCamera.transform.right;
        right.y = 0f;
        return right.normalized;
    }
}