using JetBrains.Annotations;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    [Header("Trolley parameters")]
    [SerializeField] private float _mass;
    [SerializeField] private float _force;
    [SerializeField] private float _maxSpeed = 10f;
    public float tempSpeed = 0.5f;
    //private float _forcingTime = 1; // vars for future
    //private float _speed;

    private Rigidbody _rb;
    private FollowRail _fr;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _fr = GetComponent<FollowRail>();
    }

    public void Throttle()
    {
        if (_fr._currentSpeed + tempSpeed < _maxSpeed)
        {
            _fr._currentSpeed += tempSpeed;
        }
        else
        {
            _fr._currentSpeed = _maxSpeed;
        }
        //_speed = (_force * _forcingTime) / _rb.mass; // formula for future
    }
}
