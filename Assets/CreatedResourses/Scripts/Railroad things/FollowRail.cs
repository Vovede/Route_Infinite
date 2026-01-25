using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class FollowRail : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

    [SerializeField] private VehicleType _vehicleType;

    [ReadOnlyInspector] public float _currentSpeed;

    private float3 previousNearest = float3.zero;

    private void Update()
    {
        var localPoint = _splineContainer.transform.InverseTransformPoint(transform.position);

        SplineUtility.GetNearestPoint(_splineContainer.Spline, localPoint, out var nearest, out var ratio);

        var tangent = SplineUtility.EvaluateTangent(_splineContainer.Spline, ratio);

        var rotation = Quaternion.LookRotation(tangent);
        transform.rotation = rotation;
        
        if (Vector3.SqrMagnitude(previousNearest - nearest) >= 0.0001)
        {
            var globalNearest = _splineContainer.transform.TransformPoint(nearest);
            var perpendicular = Vector3.Cross(tangent, Vector3.up);
            var position = globalNearest + perpendicular.normalized;
            transform.position = position;

            previousNearest = nearest;
        }
        
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime, Space.Self);
    }

    private enum VehicleType 
    { 
        trolley, train
    }
}