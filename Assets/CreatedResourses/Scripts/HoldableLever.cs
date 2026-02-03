using UnityEngine;
using Unity.Cinemachine;

public class HoldableLever : MonoBehaviour
{
    [Header("Interaction Points")]
    [SerializeField] private Transform[] _standpoints;
    [SerializeField] private Transform _lookPoint;

    [Header("Camera Clamp")]
    [SerializeField] private float _panClamp = 30f;   // left/right
    [SerializeField] private float _tiltUpClamp = 20f;
    [SerializeField] private float _tiltDownClamp = 10f;
    [SerializeField][ReadOnlyInspector] private bool _isClamping = false;

    private PlayerMovementController _playerController;
    private GameObject _player;

    private CinemachineCamera _cmCamera;
    private CinemachinePanTilt _panTilt;

    private float _centerPan;
    private float _centerTilt;

    private bool _isHolding;
    private bool _canHeadbob;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerMovementController>();
        _player = _playerController.gameObject;

        _cmCamera = FindFirstObjectByType<CinemachineCamera>();
        _panTilt = _cmCamera.GetComponent<CinemachinePanTilt>();

        _canHeadbob = _cmCamera.GetComponent<Headbob>()._canHeadbob;
    }

    private void Update()
    {
        if (_isClamping) ClampCamera();
        if (_isHolding) CaptureLeverPush();
    }

    public void StartInteraction()
    {
        if (_isHolding)
            return;

        SnapToClosestStandpoint();
        RotatePlayerOnce();

        _playerController.enabled = false;
        _isHolding = true;
        _canHeadbob = false;
    }

    public void DiscardInteraction()
    {
        if (_isHolding)
        {
            _playerController.enabled = true;
            _isHolding = false;
            _isClamping = false;
            _canHeadbob = true;
        }
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
        if (_lookPoint == null || _panTilt == null)
            return;

        Vector3 dir = _lookPoint.position - _cmCamera.transform.position;

        if (dir.sqrMagnitude < 0.001f)
            return;

        // --- PLAYER ROTATION (flat) ---
        Vector3 flatDir = dir;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude > 0.001f)
            _player.transform.rotation = Quaternion.LookRotation(flatDir);

        // --- CAMERA ROTATION (Pan/Tilt) ---

        // Convert direction into local space of the player
        Vector3 localDir = _player.transform.InverseTransformDirection(dir);

        // Yaw: left/right
        float yaw = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        // Pitch: up/down
        float pitch = -Mathf.Asin(localDir.y) * Mathf.Rad2Deg + 10;

        _panTilt.PanAxis.Value = yaw;
        _panTilt.TiltAxis.Value = pitch;

        _centerPan = _panTilt.PanAxis.Value;
        _centerTilt = _panTilt.TiltAxis.Value;
        _isClamping = true;
    }


    private void ClampCamera()
    {
        if (_panTilt == null)
            return;

        // --- PAN (left / right) ---
        float panMin = _centerPan - _panClamp;
        float panMax = _centerPan + _panClamp;

        _panTilt.PanAxis.Value = Mathf.Clamp(
            _panTilt.PanAxis.Value,
            panMin,
            panMax
        );

        // --- TILT (up / down) ---
        float tiltMin = _centerTilt - _tiltDownClamp - 10;
        float tiltMax = _centerTilt + _tiltUpClamp + 30;

        _panTilt.TiltAxis.Value = Mathf.Clamp(
            _panTilt.TiltAxis.Value,
            tiltMin,
            tiltMax
        );
    }

    private void CaptureLeverPush()
    {
        Debug.Log("Pan: " + _panTilt.PanAxis.Value);
        Debug.Log("Tilt: " + _panTilt.TiltAxis.Value);
    }

    private void ResetLeverPush()
    {

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
