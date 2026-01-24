using UnityEngine;
using System.Collections;

public class EnemyAI2D : MonoBehaviour
{
    public enum State { Idle, Attack }
    public State currentState = State.Idle;

    [Header("Referencje")]
    public Transform player;
    public Rigidbody2D rb;
    public LineRenderer attackLine;

    [Header("Idle")]
    public Vector2 idleCenter;
    public float idleSpeed = 1f;
    public float idleRadius = 2f;
    public float detectPlayerDistance = 20f;

    [Header("Attack - podejście")]
    public float approachSpeed = 2.5f;
    public float startAttackDistance = 5f;

    [Header("Attack - dash")]
    public float chargeTime = 3f;
    public float dashTime = 0.2f;
    public float dashDistance = 5f;

    [Header("Skala")]
    public float scaleYCharged = 0.4f;
    public float scaleYNormal = 0.5f;

    private Vector2 dashDirection;
    private bool isAttacking;

    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;

        idleCenter = transform.position;

        attackLine.positionCount = 2;
        attackLine.enabled = false;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (currentState == State.Idle && dist <= detectPlayerDistance)
            currentState = State.Attack;

        if (currentState == State.Attack && !isAttacking && dist > detectPlayerDistance * 1.2f)
            currentState = State.Idle;
    }

    void FixedUpdate()
    {
        if (currentState == State.Idle)
        {
            IdleMovement();
            RotateTowards(rb.linearVelocity);
        }
        else if (currentState == State.Attack && !isAttacking)
        {
            ApproachPlayer();
            RotateTowards(rb.linearVelocity);
        }
    }

    // ---------- IDLE ----------
    void IdleMovement()
    {
        Vector2 offset = (Vector2)transform.position - idleCenter;

        if (offset.magnitude > idleRadius)
            rb.linearVelocity = -offset.normalized * idleSpeed;
        else
            rb.linearVelocity = new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time)) * idleSpeed;
    }

    // ---------- APPROACH ----------
    void ApproachPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * approachSpeed;

        if (Vector2.Distance(transform.position, player.position) <= startAttackDistance)
        {
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(AttackSequence());
        }
    }

    // ---------- ATTACK ----------
    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // 1. Namierzenie (JEDNORAZOWO)
        dashDirection = (player.position - transform.position).normalized;
        RotateTowards(dashDirection);

        // 2. Ładowanie
        attackLine.enabled = true;
        float t = 0f;

        while (t < chargeTime)
        {
            t += Time.deltaTime;
            float lerp = t / chargeTime;

            attackLine.SetPosition(0, transform.position);
            attackLine.SetPosition(1, transform.position + (Vector3)(dashDirection * dashDistance));

            Color c = new Color(1f, 0f, 0f, Mathf.Lerp(0.2f, 1f, lerp));
            attackLine.startColor = c;
            attackLine.endColor = c;

            Vector3 scale = transform.localScale;
            scale.y = Mathf.Lerp(scaleYNormal, scaleYCharged, lerp);
            transform.localScale = scale;

            yield return null;
        }

        // 3. Dash
        attackLine.enabled = false;
        transform.localScale = new Vector3(transform.localScale.x, scaleYNormal, transform.localScale.z);

        float speed = dashDistance / dashTime;
        float timer = 0f;

        while (timer < dashTime)
        {
            rb.linearVelocity = dashDirection * speed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }

    // ---------- ROTATION (JEDYNA METODA) ----------
    void RotateTowards(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // -90 bo przód sprite'a (oczy) jest NA DOLE
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
