using UnityEngine;
using System.Collections;

public class HarpoonShooter2D : MonoBehaviour
{
    [Header("Sterowanie")]
    public KeyCode shootKey = KeyCode.Space;

    [Header("Harpun")]
    public GameObject harpoonPrefab;
    public Transform firePoint;

    [Header("Parametry lotu")]
    public float distance = 10f;
    public float forwardSpeed = 10f;
    public float returnSpeed = 6f;
    public float returnDelay = 0.5f;

    [Header("Tekstury")]
    public Sprite fullHarpoon;
    public Sprite emptyHarpoon;

    private SpriteRenderer spriteRenderer;
    private bool isShooting = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(shootKey) && !isShooting)
        {
            StartCoroutine(HarpoonRoutine());
        }
    }

    IEnumerator HarpoonRoutine()
    {
        isShooting = true;
        spriteRenderer.sprite = emptyHarpoon;

        GameObject harpoon = Instantiate(
            harpoonPrefab,
            firePoint.position,
            firePoint.rotation
        );

        LineRenderer line = harpoon.GetComponent<LineRenderer>();

        Vector3 shootDirection = firePoint.right;
        Vector3 targetPos = firePoint.position + shootDirection * distance;

        // ===== LOT DO PRZODU =====
        while (Vector3.Distance(harpoon.transform.position, targetPos) > 0.05f)
        {
            harpoon.transform.position = Vector3.MoveTowards(
                harpoon.transform.position,
                targetPos,
                forwardSpeed * Time.deltaTime
            );

            RotateHarpoon(harpoon.transform, shootDirection);
            UpdateLine(line, harpoon.transform.position);

            yield return null;
        }

        // ===== DELAY (z żywą liną) =====
        float timer = 0f;
        while (timer < returnDelay)
        {
            timer += Time.deltaTime;
            UpdateLine(line, harpoon.transform.position);
            yield return null;
        }

        // ===== POWRÓT DO RUSZAJĄCEJ SIĘ ARMATY =====
        while (Vector3.Distance(harpoon.transform.position, firePoint.position) > 0.05f)
        {
            Vector3 returnDir = (firePoint.position - harpoon.transform.position).normalized;

            harpoon.transform.position = Vector3.MoveTowards(
                harpoon.transform.position,
                firePoint.position,
                returnSpeed * Time.deltaTime
            );

            RotateHarpoon(harpoon.transform, returnDir);
            UpdateLine(line, harpoon.transform.position);

            yield return null;
        }

        Destroy(harpoon);
        spriteRenderer.sprite = fullHarpoon;
        isShooting = false;
    }

    // ================== POMOCNICZE ==================

    void UpdateLine(LineRenderer line, Vector3 harpoonPos)
    {
        if (line == null) return;

        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, harpoonPos);
    }

    void RotateHarpoon(Transform harpoon, Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        harpoon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
