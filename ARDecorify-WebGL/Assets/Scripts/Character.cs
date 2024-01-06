
using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform camera;
    [SerializeField] private float speed;
    private float currentSpeed;
    [SerializeField] private float sensivity;
    [SerializeField] private float gravity = -9.81f;
    float xRot;
    [SerializeField] Texture2D cursorRotate;
    [SerializeField] Texture2D cursorNormal;
    private void Start()
    {
        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
        currentSpeed = speed;
    }
    private void Update()
    {

        Inputs();
        Move();
        if (Input.GetMouseButton(1))
        {
            Rotate();
            Cursor.SetCursor(cursorRotate, Vector2.zero, CursorMode.Auto);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
        }

    }

    private void Inputs()
    {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    private void Move()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
            currentSpeed = speed * 5f;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            currentSpeed = speed;
        Vector3 movementVector = transform.TransformDirection(playerMovementInput);

        if (characterController.isGrounded)
        {
            velocity.y = -1;
        }
        else
        {
            velocity.y -= gravity * -2f * Time.deltaTime;
        }
        characterController.Move(movementVector * currentSpeed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    
    }
    private void Rotate()
    {
        xRot -= playerMouseInput.y * sensivity;
        transform.Rotate(0, playerMouseInput.x * sensivity, 0);
        camera.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
}
