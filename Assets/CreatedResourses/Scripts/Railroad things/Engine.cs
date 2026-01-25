using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Throttle(float power)
    {
        Debug.Log($"Throttled {power} power");
        //Vector3 dir = power * transform.forward;
        //rb.AddForce(dir);
    }
}
