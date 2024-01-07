using UnityEngine;
public class Character : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private float scrollInput;
    [SerializeField] private CharacterController characterController;
    private Camera camera;
    [SerializeField] private float speed;
    private float currentSpeed;
    [SerializeField] private float sensivity;
    [SerializeField] private float scrollSensivity;
    [SerializeField] private float gravity = -9.81f;
    private float xRot;
    private SceneController sceneController;
    private void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        Cursor.SetCursor(sceneController.cursorNormalTexture, Vector2.zero, CursorMode.Auto);
        currentSpeed = speed;
        camera = Camera.main;
    }
    private void Update()
    {
        Inputs();
        Move();
        Zoom();
        if (Input.GetMouseButton(1))
        {
            Rotate();
            Cursor.SetCursor(sceneController.cursorRotateTexture, Vector2.zero, CursorMode.Auto);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            Cursor.SetCursor(sceneController.cursorNormalTexture, Vector2.zero, CursorMode.Auto);
        }
    }
   
    private void Zoom()
    {
        float fov = camera.fieldOfView;
        fov -= scrollInput * scrollSensivity * Time.deltaTime;
        fov = Mathf.Clamp(fov, 50, 75);
        camera.fieldOfView = fov;
    }
    private void Inputs()
    {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        scrollInput = Input.GetAxis("Mouse ScrollWheel");
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
        camera.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
}
