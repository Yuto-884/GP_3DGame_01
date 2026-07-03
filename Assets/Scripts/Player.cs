using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speedMax;
    [SerializeField] float accel;
    [SerializeField] float rotateSpeed;
    [SerializeField] float jumpSpeed;

    PlayerInput playerInput;
    Rigidbody rb;
    Vector3 rotateTarget;
    Animator animator;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }


    void Update()
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

        // プレイヤーの向きを変える
        if (accelVec3D != Vector3.zero)
        {
            rotateTarget = accelVec3D.normalized;
        }

        // 前方向をコピーしておく
        Vector3 forward = transform.forward;

        // 上方向を固定
        transform.up = Vector3.up;

        // 前方向をターゲットに向かって補間
        transform.forward =
            Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);

        // アニメーターのMoveSpeedパラメータに Rigidbody の移動速度の大きさを与える
        animator.SetFloat("MoveSpeed", rb.linearVelocity.magnitude); Vector3.Slerp(forward, rotateTarget, rotateSpeed * Time.deltaTime);


        // ジャンプ
        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpVec, ForceMode.VelocityChange);
        }
    }
}