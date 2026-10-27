using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        // rb.MovePosition(rb.position + move);

        Vector3 turn = transform.right * turnInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + turn);
    }
}