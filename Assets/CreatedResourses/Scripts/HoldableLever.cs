using UnityEngine;
using Unity.Cinemachine;

public class HoldableLever : MonoBehaviour
{
    [Header("Interaction Points")]
    [SerializeField] private Transform[] _standpoints;
    [SerializeField] private Transform _lookPoint;

    [Header("Camera Clamp")]
    [SerializeField] private float _yawClamp = 30f;
    [SerializeField] private float _pitchClamp = 15f;

    private PlayerMovementController _playerController;
    private GameObject _player;

    private CinemachineCamera _cmCamera;
    private CinemachinePanTilt _panTilt;

    private float _originalYawMin;
    private float _originalYawMax;
    private float _originalPitchMin;
    private float _originalPitchMax;

    private bool _isHolding;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerMovementController>();
        _player = _playerController.gameObject;

        _cmCamera = FindFirstObjectByType<CinemachineCamera>();
        _panTilt = _cmCamera.GetComponent<CinemachinePanTilt>();
    }

    public void StartInteraction()
    {
        if (_isHolding)
            return;

        SnapToClosestStandpoint();
        RotatePlayerOnce();
        //ClampCamera();

        _playerController.enabled = false;
        _isHolding = true;
    }

    public void DiscardInteraction()
    {
        _playerController.enabled = true;
        _isHolding = false;
    }

    private void SnapToClosestStandpoint()
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Transform t in _standpoints)
        {
            float d = Vector3.Distance(_player.transform.position, t.position);
            if (d < minDist)
            {
                minDist = d;
                closest = t;
            }
        }

        if (closest != null)
            _player.transform.position = closest.position;
    }

    private void RotatePlayerOnce()
    {
        if (_lookPoint == null)
            return;

        Vector3 dir = _lookPoint.position - _player.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
            _cmCamera.transform.rotation = Quaternion.LookRotation(dir);
    }

    private void ClampCamera()
    {
        if (_panTilt == null)
            return;

        // Save original limits
        _originalYawMin = _panTilt.PanAxis.Range.x;
        _originalYawMax = _panTilt.PanAxis.Range.y;
        _originalPitchMin = _panTilt.TiltAxis.Range.x;
        _originalPitchMax = _panTilt.TiltAxis.Range.y;

        float currentYaw = _panTilt.PanAxis.Value;
        float currentPitch = _panTilt.TiltAxis.Value;

        _panTilt.PanAxis.Range = new Vector2(
            currentYaw - _yawClamp,
            currentYaw + _yawClamp
        );

        _panTilt.TiltAxis.Range = new Vector2(
            currentPitch - _pitchClamp,
            currentPitch + _pitchClamp
        );
    }

    public void Release()
    {
        if (!_isHolding)
            return;

        _isHolding = false;
        _playerController.enabled = true;

        if (_panTilt == null)
            return;

        _panTilt.PanAxis.Range = new Vector2(_originalYawMin, _originalYawMax);
        _panTilt.TiltAxis.Range = new Vector2(_originalPitchMin, _originalPitchMax);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (_standpoints != null)
            foreach (var t in _standpoints)
                if (t != null)
                    Gizmos.DrawSphere(t.position, 0.1f);

        if (_lookPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_lookPoint.position, 0.1f);
        }
    }
#endif
}
