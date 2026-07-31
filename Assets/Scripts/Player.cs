using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] float speedMax;
    [SerializeField] float accel;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float groundNormalYMin = 0.7f;
    [SerializeField] float groundDamping = 8f;
    [SerializeField] float airDamping = 0.5f;

    [SerializeField] GameObject firePrefab;
    [SerializeField] float fireSpeed = 20f;
    [SerializeField] Vector3 fireOffset;

    [SerializeField] int hp = 10;
    [SerializeField] float invincibleTimeMax = 0.5f;
    [SerializeField] float knockbackSpeed = 10;
    [SerializeField] TextMeshProUGUI hpText;

    [SerializeField] int maxAmmo = 10;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] TextMeshProUGUI ammoText;

    int ammo;
    float reloadTimer;

    float invincibleTime = 0;

    PlayerInput playerInput;
    Rigidbody rb;
    Vector3 rotateTarget;
    Animator animator;

    bool isGrounded = true;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.sleepThreshold = -1;

        hpText.text = "HP : " + hp;

        ammo = maxAmmo;
        UpdateAmmoUI();
    }

    void FixedUpdate()
    {
        // 地上と空中で減衰を変更
        if (isGrounded)
        {
            rb.linearDamping = groundDamping;
        }
        else
        {
            rb.linearDamping = airDamping;
        }

        // 毎フレーム一旦 false にする
        isGrounded = false;

        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (playerInput == null)
        {
            return;
        }

        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
        }

        if (isGrounded)
        {
            var accelVec = playerInput.actions["Move"].ReadValue<Vector2>();

            var cameraDir = playerInput.camera.transform.forward;
            cameraDir.y = 0;
            cameraDir = cameraDir.normalized;

            var cameraRight = playerInput.camera.transform.right;

            var accelVec3D =
                cameraDir * accelVec.y * accel
                + cameraRight * accelVec.x * accel;

            rb.AddForce(accelVec3D, ForceMode.Acceleration);

            if (accelVec3D != Vector3.zero)
            {
                rotateTarget = accelVec3D.normalized;
            }

        }

        // 前方向をコピー
        Vector3 forward = transform.forward;

        // 上方向固定
        transform.up = Vector3.up;

        // 向きを補間
        Vector3 targetForward =
            Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);

        if (targetForward != Vector3.zero)
        {
            transform.forward = targetForward;
        }

        // アニメーション
        Vector3 velocityXZ = rb.linearVelocity;
        velocityXZ.y = 0;
        animator.SetFloat("MoveSpeed", velocityXZ.magnitude );

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpVec, ForceMode.VelocityChange);
        }

        // 攻撃
        if (playerInput.actions["Attack"].WasPressedThisFrame() && ammo > 0)
        {
            ammo--;
            UpdateAmmoUI();

            var position = transform.position + transform.TransformVector(fireOffset);
            var fireObj = Instantiate(firePrefab, position, transform.rotation);

            fireObj.GetComponent<AttackObject>().owner = gameObject;

            var fireRB = fireObj.GetComponent<Rigidbody>();
            fireRB.linearVelocity = transform.forward * fireSpeed;
        }

        if (ammo < maxAmmo)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                ammo++;
                reloadTimer = 0;

                UpdateAmmoUI();
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y >= groundNormalYMin)
            {
                isGrounded = true;
            }
        }

        var attackObj = collision.gameObject.GetComponent<AttackObject>();
        if (attackObj != null &&
        attackObj.owner != gameObject &&
        invincibleTime <= 0)
        {
            hp -= attackObj.power;
            hpText.text = "HP : " + hp;
            invincibleTime = invincibleTimeMax;
            if (hp <= 0)
            {
                GameManager.Instance.GameOver();

                Destroy(gameObject);
            }

            // ノックバック
            var dir = transform.position - collision.transform.position;
            dir.y = 0;
            var knockbackVec = dir.normalized * knockbackSpeed;
            rb.AddForce(knockbackVec, ForceMode.VelocityChange);
        }
    }

    void UpdateAmmoUI()
    {
        string text = "Ammo : ";

        for (int i = 0; i < maxAmmo; i++)
        {
            if (i < ammo)
                text += "●";
            else
                text += "○";
        }

        ammoText.text = text;
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;

        if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }

        UpdateAmmoUI();
    }
}