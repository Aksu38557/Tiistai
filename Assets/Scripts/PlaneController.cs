using Unity.Mathematics;
using UnityEngine;

public class PlaneController : MonoBehaviour
{

    public float flySpeed = 5f;
    public float yawAmount = 120;

    private float yaw;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * flySpeed * Time.deltaTime;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        yaw += horizontalInput * yawAmount * Time.deltaTime;
        float pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * Mathf.Sign(verticalInput);
        float roll = Mathf.Lerp(0, 30, Mathf.Abs(horizontalInput)) * -Mathf.Sign(horizontalInput);

        transform.localRotation = quaternion.Euler(Vector3.up * yaw + Vector3.right * (pitch * Time.deltaTime) + Vector3.forward * (roll * Time.deltaTime));
    }
}
