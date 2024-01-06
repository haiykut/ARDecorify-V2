using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] float speed;
    [SerializeField] float sensivity;
    float inputX;
    float inputZ;
    float mouseX;
    float mouseY;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Rotate();
    }
    void Rotate()
    {
        transform.eulerAngles = new Vector3(mouseX * sensivity, mouseY * sensivity, 0);
    }
    void GetInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        mouseY += Input.GetAxis("Mouse X");
        mouseX += Input.GetAxis("Mouse Y");
    }
    void Move()
    {
        if(Camera.current != null)
        Camera.current.transform.Translate(new Vector3(inputX * speed, 0.0f, inputZ * speed));
    }
}
