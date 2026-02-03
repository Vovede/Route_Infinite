using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Headbob parameters")]
    public bool _canHeadbob = true;
    [SerializeField] private float _frequency = 2f;
    [SerializeField] private float _amplitude = 0.125f;
    private float _defaultYPosition;
    private float _timer;

    private PlayerMovementController _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<PlayerMovementController>();
        _defaultYPosition = transform.position.y - 1f;
    }

    private void Update()
    {
        if (_canHeadbob)
        {
            HandleHeadbob();
        }
    }

    private void HandleHeadbob()
    {
        if (!_controller._isGrounded) return;

        if (_controller._isWalking)
        {
            _timer += Time.deltaTime * _controller.currentSpeed * _frequency;
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                _defaultYPosition + Mathf.Sin(_timer) * _amplitude,
                transform.localPosition.z);
        }
    }
}
