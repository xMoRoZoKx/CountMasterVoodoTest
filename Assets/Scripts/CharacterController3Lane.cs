using UnityEngine;

public class CharacterController3Lane : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float lateralSpeed = 10f;
    public Transform[] lanePositions; // 0: левая, 1: центральная, 2: правая
    public Animator animator;
    public Transform shootPoint;
    public GameObject projectilePrefab;
    public Camera mainCamera;
    public WeaponData weaponData;

    private int currentLane = 1; // стартовая позиция – центральная
    private bool isMovingSide = false;
    private float lastShootTime = -Mathf.Infinity;

    [Header("Jump Settings")]
    public float jumpHeight = 4f;
    public float jumpDuration = 1f;
    public AnimationCurve jumpCurve;
    public float cameraShakeIntensity = 0.2f;
    public float cameraShakeDuration = 0.2f;

    [Header("Jump Forward Motion")]
    public float jumpForwardDistance = 5f; // расстояние, на которое персонаж смещается вперёд во время прыжка

    private bool isJumping = false;
    private float groundY;
    private bool canShoot = true;

    private Vector3 cameraOffset;
    private Vector3 cameraShakeOffset = Vector3.zero;

    [Header("Stomp Settings")]
    public float stompRadius = 1.5f;

    [HideInInspector]
    public bool isLandingNow = false; // временная неуязвимость при приземлении

    void Start()
    {
        if (mainCamera != null)
        {
            //mainCamera.transform.SetParent(transform);
            cameraOffset = new Vector3(0, 12, -10);
            mainCamera.transform.localPosition = new Vector3(0, 12, -10);
            mainCamera.transform.localRotation = Quaternion.Euler(40, 0, 0);
        }

        groundY = transform.position.y;
    }

    void Update()
    {
        if (!isJumping)
        {
            HandleInput();
            HandleShooting();
        }

        // Движение вперёд всегда
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + cameraOffset + cameraShakeOffset;
            mainCamera.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        }
    }

    void HandleInput()
    {
        if (isMovingSide) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > 0)
            {
                currentLane--;
                StartCoroutine(MoveToLane(currentLane));
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < 2)
            {
                currentLane++;
                StartCoroutine(MoveToLane(currentLane));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            StartCoroutine(DoJump());
        }
    }

    System.Collections.IEnumerator MoveToLane(int targetLane)
    {
        isMovingSide = true;
        Vector3 start = transform.position;
        Vector3 end = new Vector3(lanePositions[targetLane].position.x, start.y, start.z);
        float elapsed = 0f;
        float duration = Mathf.Abs(end.x - start.x) / lateralSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = new Vector3(Mathf.Lerp(start.x, end.x, t), start.y, start.z);
            yield return null;
        }

        transform.position = new Vector3(end.x, start.y, start.z);
        isMovingSide = false;
    }

    void HandleShooting()
    {
        if (!canShoot) return;

        if (Input.GetMouseButton(0) && Time.time >= lastShootTime + weaponData.cooldown)
        {
            lastShootTime = Time.time;
            animator.SetTrigger("Attack");

            // Визуальный эффект выстрела
            if (weaponData.shootEffect != null)
            {
                Quaternion effectRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(90f, 0f, 0f);
                Instantiate(weaponData.shootEffect, shootPoint.position, effectRotation);
            }

            // Создание снаряда
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            ProjectileLogic logic = projectile.GetComponent<ProjectileLogic>();
            if (logic != null)
            {
                logic.Initialize(
                    weaponData.projectileSpeed,
                    transform.forward,
                    weaponData.damage,
                    weaponData.projectileRange
                );
            }
        }
    }

    private bool stompExecuted = false;
    System.Collections.IEnumerator DoJump()
    {
        isJumping = true;
        canShoot = false;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsed < jumpDuration)
        {
            RaycastHit hit;
            if (!stompExecuted &&
                Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    stompExecuted = true;
                    isLandingNow = true;
                    CheckStompKill(); // сразу убиваем всех врагов в радиусе

                    StartCoroutine(LandingInvulnerabilityReset()); // сбросим флаг через кадр
                }
            }

            float t = elapsed / jumpDuration;

            // Вертикальное перемещение
            float height = jumpCurve.Evaluate(t);
            float verticalY = groundY + height * jumpHeight;

            // Горизонтальное перемещение вперёд (по Z)
            float forwardZ = Mathf.Lerp(startPos.z, startPos.z + jumpForwardDistance, t);

            // Обновляем позицию
            transform.position = new Vector3(startPos.x, verticalY, forwardZ);

            // Поворот на 360° по X
            float rotationAngle = 360f * t;
            Quaternion rotationOffset = Quaternion.Euler(rotationAngle, 0f, 0f);
            transform.rotation = startRotation * rotationOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Завершаем прыжок
        transform.position = new Vector3(startPos.x, groundY, startPos.z + jumpForwardDistance);
        transform.rotation = startRotation;
        stompExecuted = false;

        CheckStompKill();
        StartCoroutine(CameraShake());

        isJumping = false;
        canShoot = true;
    }


    System.Collections.IEnumerator CameraShake()
    {
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeIntensity;
            float y = Random.Range(-1f, 1f) * cameraShakeIntensity;
            cameraShakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraShakeOffset = Vector3.zero;
    }


    void CheckStompKill()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stompRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                WoodPartView enemy = hit.GetComponent<WoodPartView>();
                if (enemy != null)
                {
                    enemy.Die(); // мгновенная смерть
                }
            }
        }
    }

    System.Collections.IEnumerator LandingInvulnerabilityReset()
    {
        yield return null; // один кадр
        isLandingNow = false;
    }

    System.Collections.IEnumerator LandingInvulnerability()
    {
        isLandingNow = true;
        Debug.Log("НЕУЯЗВИМЫЙ!");
        yield return new WaitForSeconds(5f); // достаточно одного кадра
        isLandingNow = false;
    }
}