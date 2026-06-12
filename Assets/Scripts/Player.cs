using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speedMax;

    PlayerInput playerInput;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }


    void Update()
    {
        var moveVec = playerInput.actions["Move"].ReadValue<Vector2>();

        var cameraDir = playerInput.camera.transform.forward;
        var cameraRight = playerInput.camera.transform.right;

        cameraDir.y = 0;
        cameraDir = cameraDir.normalized;

        cameraRight.y = 0;
        cameraRight = cameraRight.normalized;


        var moveVec3D =
            cameraDir * moveVec.y * speedMax
            + cameraRight * moveVec.x * speedMax;


        transform.position += moveVec3D * Time.deltaTime;
    }
}