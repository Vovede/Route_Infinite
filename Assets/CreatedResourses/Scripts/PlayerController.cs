using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Speed statistics")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float airControl = 0.5f; // How much control you have in air

    private CharacterController _characterController;
    private CinemachineCamera _cinemachineCamera;
    private Vector2 _move;

    [Header("Jump statistics")]
    [SerializeField] private float _jumpHeight = 3f;
    private Vector3 _velocity;
    [SerializeField][ReadOnlyInspector] private float _gravityForce = -15f;

    [Header("Jump Enhancements")]
    [SerializeField] private float coyoteTime = 0.2f; // Allows jumping slightly after leaving edge
    [SerializeField] private float jumpBufferTime = 0.2f; // Allows pressing jump before landing

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _jumpPressed;
    private bool _isJumping;

    [Header("Ground detection")]
    [SerializeField] private Transform _checkerPosition;
    [SerializeField] private float _checkRadius = 0.4f;
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField][ReadOnlyInspector] private bool _isGrounded;

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
        _move = inputValue.Get<Vector2>();
    }

    public void OnSprint(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
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

        // Coyote time logic
        if (_isGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
            _isJumping = false; // Reset jumping state when grounded
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }

        // Reset jump buffer if we're grounded and not trying to jump
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
        // Reset velocity when grounded
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // Perform jump if conditions are met
        if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0 && !_isJumping)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravityForce);
            _jumpBufferCounter = 0;
            _coyoteTimeCounter = 0;
            _isJumping = true;

            // Debug jump
            // Debug.Log("Jump executed!");
        }
    }

    private void ApplyGravity()
    {
        // Apply gravity if not grounded
        if (!_isGrounded)
        {
            _velocity.y += _gravityForce * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        // Calculate speed multiplier (less control in air)
        float speedMultiplier = _isGrounded ? 1f : airControl;

        // Calculate movement vectors
        Vector3 horizontalMovement = (GetForward() * _move.y + GetRight() * _move.x) * currentSpeed * speedMultiplier;

        // Combine horizontal and vertical movement
        Vector3 finalMovement = new Vector3(horizontalMovement.x, _velocity.y, horizontalMovement.z);

        // Apply movement
        _characterController.Move(finalMovement * Time.deltaTime);

        // Debug info (optional)
        // Debug.Log($"Grounded: {_isGrounded}, VelocityY: {_velocity.y:F2}, Coyote: {_coyoteTimeCounter:F2}, Buffer: {_jumpBufferCounter:F2}");
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

    // Optional: Visualize ground check sphere in editor
    private void OnDrawGizmosSelected()
    {
        if (_checkerPosition != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_checkerPosition.position, _checkRadius);
        }
    }
}