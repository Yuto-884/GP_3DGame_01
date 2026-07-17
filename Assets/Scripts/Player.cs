using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    void Update()
    {
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
        animator.SetFloat("MoveSpeed", velocityXZ.magnitude);

        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpVec, ForceMode.VelocityChange);
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
    }
}