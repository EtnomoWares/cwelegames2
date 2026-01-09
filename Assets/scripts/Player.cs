using UnityEngine;

public class Player : MonoBehaviour
{
    // Input
    private float moveHorizontal;
    private float moveVertical;

    [Header("Movement")]
    public float speed = 220f;
    public float maxSpeed = 14f;
    public float linearDamping = 1.2f;

    [Header("Rotation")]
    public float baseTorque = 2f;
    public float maxTorque = 8f;
    public float torqueRampSpeed = 2.5f;
    public float angularResistance = 3f;
    public float maxAngularVelocity = 90f;

    private float currentTorque;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = linearDamping;
    }

    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Nap�d
        if (Mathf.Abs(moveVertical) > 0.01f)
        {
            rb.AddForce(transform.up * moveVertical * speed * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        // Limit pr�dko�ci
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Narastanie momentu obrotowego
        if (Mathf.Abs(moveHorizontal) > 0.01f)
        {
            currentTorque += torqueRampSpeed * Time.fixedDeltaTime;
            currentTorque = Mathf.Clamp(currentTorque, baseTorque, maxTorque);
        }
        else
        {
            currentTorque = 0f;
        }

        // Obr�t
        rb.AddTorque(-moveHorizontal * currentTorque);

        // Op�r obrotu
        rb.AddTorque(-rb.angularVelocity * angularResistance);

        // Limit pr�dko�ci k�towej
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }
}
