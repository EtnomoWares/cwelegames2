using UnityEngine;

public class Player : MonoBehaviour
{
    private float moveHorizontal;
    private float moveVertical;
    private float rotationPower =50f;
    private float angularResistance = 3f;
    private float maxAngularVelocity = 90f;


    private float baseTorque = 2f;        // minimalna si³a steru
    private float maxTorque = 8f;         // maksymalna si³a steru
    private float torqueRampSpeed = 2.5f; // jak szybko narasta obrót


    private float currentTorque;
    void Update()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

    }

    void FixedUpdate()
    {
        float speed = 150f;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Transform transform = GetComponent<Transform>();
        rb.AddForce(new Vector2(0f,moveVertical * speed * Time.deltaTime), ForceMode2D.Impulse);
        rb.linearDamping = 10;

        if (rb.angularVelocity < -maxAngularVelocity) { rb.angularVelocity = -maxAngularVelocity; }
        if (rb.angularVelocity > maxAngularVelocity) { rb.angularVelocity = maxAngularVelocity; }

        if (Mathf.Abs(moveHorizontal) > 0.01f)
        {
            currentTorque += torqueRampSpeed * Time.fixedDeltaTime;
            currentTorque = Mathf.Clamp(currentTorque, baseTorque, maxTorque);
        }
        else
        {
            currentTorque = 0f;
        }

        rb.AddTorque(-moveHorizontal * rotationPower);

        float resistanceTorque = (-rb.angularVelocity * angularResistance);
        rb.AddTorque(resistanceTorque);
    }
}
