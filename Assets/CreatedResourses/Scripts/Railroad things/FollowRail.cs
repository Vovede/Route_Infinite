using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class FollowRail : MonoBehaviour
{
    [SerializeField] private SplineContainer rail;

    private Spline currentSpline;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentSpline = rail.Splines[0];
    }

    private void FixedUpdate()
    {
        var native = new NativeSpline(currentSpline, rail.transform.localToWorldMatrix);
        float distance = SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest, out float t);

        rb.MovePosition((Vector3)nearest);

        Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
        Vector3 up = native.EvaluateUpVector(t);

        var remappedForward = new Vector3(0, 0, 1);
        var remappedUp = new Vector3(0, 1, 0);
        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

        Quaternion targetRotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
        rb.MoveRotation(targetRotation);

        Vector3 engineForward = targetRotation * Vector3.forward;

        //if (Vector3.Dot(rb.linearVelocity, transform.forward) < 0)
        //{
        //    engineForward *= -1;
        //}
        //rb.linearVelocity = rb.linearVelocity.magnitude * engineForward;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        //Debug.Log(rb.linearVelocity);
    }
}