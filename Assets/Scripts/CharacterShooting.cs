using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float lateralSpeed = 10f;
    public Animator animator;
    public Transform shootPoint;
    public GameObject projectilePrefab;
    public Camera mainCamera;
    public WeaponData weaponData;

    private bool isMovingSide = false;
    private float lastShootTime = -Mathf.Infinity;

    [Header("Jump Settings")]
    public float jumpHeight = 4f;
    public float jumpDuration = 1f;
    public AnimationCurve jumpCurve;
    public float cameraShakeIntensity = 0.2f;
    public float cameraShakeDuration = 0.2f;

    [Header("Jump Forward Motion")]
    public float jumpForwardDistance = 5f; // ðàññòîÿíèå, íà êîòîðîå ïåðñîíàæ ñìåùàåòñÿ âïåð¸ä âî âðåìÿ ïðûæêà

    private bool isJumping = false;
    private float groundY;
    private bool canShoot = true;

    private Vector3 cameraOffset;
    private Vector3 cameraShakeOffset = Vector3.zero;

    [Header("Stomp Settings")]
    public float stompRadius = 1.5f;

    [HideInInspector]
    public bool isLandingNow = false; // âðåìåííàÿ íåóÿçâèìîñòü ïðè ïðèçåìëåíèè

    [Header("Водная зона")]
    public ListokZoneTracker listokZone; // Ссылка на зону
    public GameObject waterSplashPrefab; // Префаб эффекта

    void Start()
    {
        if (mainCamera != null)
        {
            //mainCamera.transform.SetParent(transform);
            cameraOffset = new Vector3(0, 12, -10);
           // mainCamera.transform.localPosition = new Vector3(0, 12, -10);
            //mainCamera.transform.localRotation = Quaternion.Euler(40, 0, 0);
        }

        groundY = transform.position.y;
    }

    void Update()
    {
        if (!isJumping)
        {
            HandleShooting();
        }

        // Äâèæåíèå âïåð¸ä âñåãäà
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
           // mainCamera.transform.position = transform.position + cameraOffset + cameraShakeOffset;
            //mainCamera.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        }
    }


    void HandleShooting()
    {
        if (!canShoot) return;

        if (Input.GetMouseButton(1) && Time.time >= lastShootTime + weaponData.cooldown)
        {
            lastShootTime = Time.time;
            animator.SetFloat("Speed", 2f);
            animator.SetTrigger("Attack");

            // Âèçóàëüíûé ýôôåêò âûñòðåëà
            if (weaponData.shootEffect != null)
            {
                Quaternion effectRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(90f, 0f, 0f);
                Instantiate(weaponData.shootEffect, shootPoint.position, effectRotation);
            }

            // Создание снаряда
            /*Quaternion fixedRotation = Quaternion.Euler(0f, 0f, 0f);
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, fixedRotation);
            ProjectileLogic logic = projectile.GetComponent<ProjectileLogic>();
            if (logic != null)
            {
                logic.Initialize(
                    weaponData.projectileSpeed,
                    transform.forward,
                    weaponData.damage,
                    weaponData.projectileRange
                );
            }*/

            // Создание снаряда
            Quaternion fixedRotation = Quaternion.Euler(0f, 0f, 0f);
            Vector3 spawnPos = shootPoint.position;

            // Если игрок находится в зоне ListokZone — опускаем снаряд
            if (listokZone != null && listokZone.isInListokZone)
            {
                spawnPos.y -= 1f;
            }

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, fixedRotation);

            // Подключаем логику снаряда
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

            // Если игрок в зоне — прикрепляем эффект WaterSplash
            if (listokZone != null && listokZone.isInListokZone && waterSplashPrefab != null)
            {
                GameObject splash = Instantiate(waterSplashPrefab, projectile.transform.position, Quaternion.identity, projectile.transform);
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
                    CheckStompKill(); // ñðàçó óáèâàåì âñåõ âðàãîâ â ðàäèóñå

                    StartCoroutine(LandingInvulnerabilityReset()); // ñáðîñèì ôëàã ÷åðåç êàäð
                }
            }

            float t = elapsed / jumpDuration;

            // Âåðòèêàëüíîå ïåðåìåùåíèå
            float height = jumpCurve.Evaluate(t);
            float verticalY = groundY + height * jumpHeight;

            // Ãîðèçîíòàëüíîå ïåðåìåùåíèå âïåð¸ä (ïî Z)
            float forwardZ = Mathf.Lerp(startPos.z, startPos.z + jumpForwardDistance, t);

            // Îáíîâëÿåì ïîçèöèþ
            transform.position = new Vector3(startPos.x, verticalY, forwardZ);

            // Ïîâîðîò íà 360° ïî X
            float rotationAngle = 360f * t;
            Quaternion rotationOffset = Quaternion.Euler(rotationAngle, 0f, 0f);
            transform.rotation = startRotation * rotationOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Çàâåðøàåì ïðûæîê
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
                    enemy.Die(); // ìãíîâåííàÿ ñìåðòü
                }
            }
        }
    }

    System.Collections.IEnumerator LandingInvulnerabilityReset()
    {
        yield return null; // îäèí êàäð
        isLandingNow = false;
    }

    System.Collections.IEnumerator LandingInvulnerability()
    {
        isLandingNow = true;
        Debug.Log("ÍÅÓßÇÂÈÌÛÉ!");
        yield return new WaitForSeconds(5f); // äîñòàòî÷íî îäíîãî êàäðà
        isLandingNow = false;
    }
}