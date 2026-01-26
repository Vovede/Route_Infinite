using Unity.VisualScripting;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    [Header("Trolley parameters")]
    [SerializeField] private float _force;
    [SerializeField] private float _maxSpeed = 10f;

    [Space]
    [SerializeField][ReadOnlyInspector] private float _currentSpeed;
    [SerializeField][ReadOnlyInspector] private bool _ableToThrottle = true;
    [SerializeField][ReadOnlyInspector] private bool _facingForward = true;

    private Vector3 _currentPosition;
    private Vector3 _previousPosition;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _currentPosition = GetComponent<Transform>().position;

        Vector3 positionDelta = _currentPosition - _previousPosition;
        float distanceDelta = Mathf.Sqrt(Mathf.Pow(positionDelta.x, 2) + Mathf.Pow(positionDelta.y, 2) + Mathf.Pow(positionDelta.z, 2));
        _currentSpeed = distanceDelta / Time.fixedDeltaTime;

        _ableToThrottle = (_currentSpeed < _maxSpeed);

        _previousPosition = _currentPosition;
    }

    public void Throttle()
    {
        if (_ableToThrottle)
        {
            int facing = (_facingForward ? 1 : -1);
            Vector3 dir = facing * _force * 100 * transform.forward;
            _rb.AddForce(dir);
        } //TODO create normal settings for accelerating and speed control
    }

    public void Brake()
    {
        //TODO braking when lever is touched (add braking lever)
    }

    public void ChangeDirection()
    {
        _facingForward = !_facingForward;
        //TODO check for _currentSpeed == 0 then able to change direction
        //     add an interactable object (lever or arrow that shows direction)
    }
}
