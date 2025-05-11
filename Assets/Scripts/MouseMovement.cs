using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float xRotation = 0f; 
    float yRotation = 0f; 
    /// <summary>
    /// TEST
    /// </summary>
    public float topClamp = -90f; 
    public float bottomClamp = 90f; 
    void Start()
    {
        //lock the cursor cuz its an fps you dont need it
        Cursor.lockState = CursorLockMode.Locked;   
    }

    void Update()
    {
        //get the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //rotation around x Axis looking w saf
        xRotation -= mouseY;

        //dont look too much like dont see legs
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        //rotation around y Axis looking w saf
        yRotation += mouseX;

        //apply rotation
        transform.localRotation = Quaternion.Euler(xRotation,yRotation,0f);
    }
}
