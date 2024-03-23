using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementScript : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 15.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 1.0f;
    public float lookXLimit = 45.0f;

    private PlayerInput playerInput;
    
    private CharacterController characterController;
    private Vector2 movementInput;
    private bool jumpInput;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private void Start()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {

        Cursor.visible = playerInput.Player.Showmouse.ReadValue<float>() > 0;
        
        CalculateMovement(out var forward, out var right, out var curSpeedX, out var curSpeedY);
        
        var movementDirectionY = moveDirection.y;
        moveDirection = forward * curSpeedX + right * curSpeedY;
        jumpInput = playerInput.Player.UpDown.ReadValue<float>() > 0;

        if (jumpInput && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (!canMove) return;

        rotationX += -playerInput.Player.MouseDelta.ReadValue<Vector2>().y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, playerInput.Player.MouseDelta.ReadValue<Vector2>().x * lookSpeed, 0);
    }

    private void CalculateMovement(out Vector3 forward, out Vector3 right, out float curSpeedX, out float curSpeedY)
    {
        movementInput = playerInput.Player.MoveKeys.ReadValue<Vector2>();
        var isRunning = playerInput.Player.Run.ReadValue<float>() > 0;
        forward = transform.TransformDirection(Vector3.forward);
        right = transform.TransformDirection(Vector3.right);
        curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * movementInput.y : 0;
        curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * movementInput.x : 0;
    }
}
