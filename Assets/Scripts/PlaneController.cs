using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlaneController : MonoBehaviour
{
    public Transform cameraTransform;
    public float shakeDuration = 0.15f;
    public float shakeAmount = 0.15f;

    private Vector3 cameraOriginalPosition;
    //
    private float currentCameraTilt;

    public float cameraTiltAmount = 15f;
    public float cameraTiltSpeed = 5f;
    //
    public float flySpeed = 5f;
    public float yawAmount = 120f;
    public float turnSmoothTime = 0.15f;

    public float hp = 3f;
    public float damageamount = 1f;

    private float yaw;
    private float yawVelocity;
    private float fixedY;

    public UnityEvent hit;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        fixedY = transform.position.y;

        if (cameraTransform != null)
        {
            cameraOriginalPosition = cameraTransform.localPosition;
        }
    }

    public void death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ResetBuilding"))
        {
            collision.gameObject.GetComponent<BoxCollider>().enabled = false;

            hp -= damageamount;
            hit.Invoke();

            StartCoroutine(CameraShake());

            if (hp <= 0)
            {
                death();
            }
        }

        if (collision.gameObject.CompareTag("DisappearBuilding"))
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator CameraShake()
    {
        if (cameraTransform == null)
        {
            yield break;
        }
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            cameraTransform.localPosition =
                cameraOriginalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = cameraOriginalPosition;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 horizontalForward = transform.forward;
        //
        horizontalForward.Normalize();
        //
        // Move forward
        transform.position += transform.forward * flySpeed * Time.deltaTime;

        // Turn left/right
        float targetYaw = yaw + horizontalInput * yawAmount;

        yaw = Mathf.SmoothDampAngle(
            yaw,
            targetYaw,
            ref yawVelocity,
            turnSmoothTime
        );
        //
        horizontalForward.Normalize();

        if (cameraTransform != null)
        {
            float targetTilt = -horizontalInput * cameraTiltAmount;

            currentCameraTilt = Mathf.Lerp(
                currentCameraTilt,
                targetTilt,
                cameraTiltSpeed * Time.deltaTime
            );

            cameraTransform.localRotation = Quaternion.Euler(
                0f,
                0f,
                currentCameraTilt
            );

            // Apply rotation
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            // Lock vertical position
            transform.position = new Vector3(
                transform.position.x,
                fixedY,
                transform.position.z
            );
        }
    }
}