using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    [Header("Speed statistics")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    private CharacterController _characterController;
    private CinemachineCamera _cinemachineCamera;
    private Vector2 _move;

    [Header("Jump statistics")]
    [SerializeField] private float _maxJumpTime;
    [SerializeField] private float _maxJumpHeight;
    [SerializeField][ReadOnlyInspector] private float _gravityForce = -9.8f;
    private float _startJumpVelocity;

    [Header("Ground detection")]
    [SerializeField] private Transform _checkerPosition;
    [SerializeField] private float _checkRadius;
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
        if (inputValue.Get<float>() > 0.5f)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!_isGrounded) return;

        _startJumpVelocity = (2 * _maxJumpHeight) / (_maxJumpTime / 2);
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_checkerPosition.position, _checkRadius, _groundLayers);
        Debug.Log(_isGrounded);

        _characterController.Move((GetForward() * _move.y + GetRight() * _move.x) * Time.deltaTime * currentSpeed);
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
