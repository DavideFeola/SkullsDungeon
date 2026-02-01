using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Aim & Shoot")]
    public Weapon weapon;
    public Camera worldCamera;

    [Header("Animation")]
    public Animator animator;

    [Header("Anim Controllers")]
    public RuntimeAnimatorController unarmedController;
    public RuntimeAnimatorController armedController;
    public bool startUnarmed = true;

    [Header("Tuning")]
    public float moveDeadzone = 0.2f;     // evita jitter su input analogico
    public float aimDeadzone = 0.05f;     // evita flip quando miri quasi verticale
    public float runThreshold = 0.05f;    // soglia per considerare "sto correndo" (usala anche nell'Animator)

    Vector2 moveInput;
    Rigidbody2D rb;

    float lastDirX = 1f;   // 1 = destra, -1 = sinistra
    float aimY = 0f;       // ✅ nuovo: -1 down, +1 top
    bool isShootingAnim = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (worldCamera == null) worldCamera = Camera.main;
        if (animator == null) animator = GetComponent<Animator>();

        // ===== START STATE (unarmed) =====
        if (startUnarmed)
        {
            if (weapon != null) weapon.gameObject.SetActive(false);
            if (unarmedController != null && animator != null)
                animator.runtimeAnimatorController = unarmedController;

            if (animator != null) animator.SetBool("IsShooting", false);
        }
        else
        {
            if (weapon != null) weapon.gameObject.SetActive(true);
            if (armedController != null && animator != null)
                animator.runtimeAnimatorController = armedController;

            if (animator != null) animator.SetBool("IsShooting", false);
        }
    }

    public void EquipWeaponFromShop()
    {
        if (weapon != null) weapon.gameObject.SetActive(true);

        if (animator == null) animator = GetComponent<Animator>();
        if (armedController != null && animator != null)
            animator.runtimeAnimatorController = armedController;

        isShootingAnim = false;
        if (animator != null) animator.SetBool("IsShooting", false);
    }

    public void UnequipWeapon()
    {
        if (weapon != null) weapon.gameObject.SetActive(false);

        if (animator == null) animator = GetComponent<Animator>();
        if (unarmedController != null && animator != null)
            animator.runtimeAnimatorController = unarmedController;

        isShootingAnim = false;
        if (animator != null) animator.SetBool("IsShooting", false);
    }

    void Update()
    {

        if (worldCamera == null || !worldCamera.enabled)
            worldCamera = Camera.main;

        // ============ MOVIMENTO ============
        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsUsingMobileControls())
        {
            moveInput = MobileInputManager.Instance.GetMovementInput();
        }
        else
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        // Applica deadzone movimento (soprattutto per mobile)
        Vector2 filteredMove = moveInput;
        if (filteredMove.magnitude < moveDeadzone)
            filteredMove = Vector2.zero;

        // ===== ANIM: direzione (MOVIMENTO) =====
        if (Mathf.Abs(filteredMove.x) > 0.01f)
        {
            lastDirX = Mathf.Sign(filteredMove.x);
        }
        else if (Mathf.Abs(filteredMove.y) > 0.01f)
        {
            // Verticale puro: su = Run_Right, giù = Run_Left
            lastDirX = (filteredMove.y > 0f) ? 1f : -1f;
        }

        // ============ MIRA E SPARO ============
        isShootingAnim = false;
        bool weaponActive = (weapon != null && weapon.gameObject.activeSelf);

        if (weaponActive)
        {
            if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsUsingMobileControls())
            {
                Vector2 shootDir = MobileInputManager.Instance.GetShootingInput();

                if (shootDir.magnitude > 0.1f)
                {
                    weapon.SetAimDirection(shootDir);

                    // ✅ AimY per top/down shot (mobile)
                    Vector2 n = shootDir.normalized;
                    aimY = n.y;

                    // direzione left/right basata sulla mira (mobile)
                    if (Mathf.Abs(shootDir.x) > aimDeadzone)
                        lastDirX = Mathf.Sign(shootDir.x);

                    weapon.TryStartFire();
                    isShootingAnim = true;
                }
                else
                {
                    weapon.StopFire();
                    isShootingAnim = false;
                }
            }
            else
            {
                bool holdFire = Input.GetMouseButton(0);

                if (worldCamera != null)
                {
                    Vector3 mouseWorld = worldCamera.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 aimDir = (mouseWorld - transform.position);

                    if (aimDir.sqrMagnitude > 0.0001f)
                    {
                        weapon.SetAimDirection(aimDir);

                        Vector2 n = aimDir.normalized;

                        // ✅ AimY per top/down shot (PC) - sempre aggiornato
                        aimY = n.y;

                        // left/right basato su mira SOLO mentre spari
                        if (holdFire && Mathf.Abs(n.x) > aimDeadzone)
                            lastDirX = Mathf.Sign(n.x);
                    }
                }

                if (holdFire) weapon.TryStartFire();
                else weapon.StopFire();

                isShootingAnim = holdFire;
            }
        }
        else
        {
            // se non hai arma, puoi anche azzerare AimY (opzionale)
            aimY = 0f;
        }

        // ===== ANIM: speed + parametri =====
        if (animator != null)
        {
            float rbSpeed = (rb != null) ? rb.linearVelocity.magnitude : 0f;
            float inputSpeed = filteredMove.magnitude;
            float animSpeed = Mathf.Max(inputSpeed, rbSpeed);

            animator.SetFloat("Speed", animSpeed);
            animator.SetFloat("DirectionX", lastDirX);
            animator.SetFloat("AimY", aimY);          // ✅ nuovo parametro
            animator.SetBool("IsShooting", isShootingAnim);
        }
    }

    void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Dynamic && rb.simulated)
            rb.linearVelocity = moveInput * moveSpeed;
        else
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void OnFireDown()
    {
        if (weapon != null && weapon.gameObject.activeSelf)
        {
            weapon.TryStartFire();
            isShootingAnim = true;
            if (animator != null) animator.SetBool("IsShooting", true);
        }
    }

    public void OnFireUp()
    {
        if (weapon != null && weapon.gameObject.activeSelf)
            weapon.StopFire();

        isShootingAnim = false;
        if (animator != null) animator.SetBool("IsShooting", false);
    }

    public void RefreshCamera()
    {
        worldCamera = Camera.main;
        Debug.Log("[PlayerController] Camera aggiornata a: " + (worldCamera != null ? worldCamera.name : "null"));
    }
}
