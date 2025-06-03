using UnityEngine;

public class BlackHoleGaze : MonoBehaviour
{
    [Header("Black Hole Settings")]
    public Transform blackHole;
    public float gazeRadius = 20f;
    public float attractionSpeed = 0.5f; // Hoe snel sensitivity daalt
    public float minSensitivity = 0.1f;
    public float recoverySpeed = 1f; // Hoe snel je weer herstelt als je wegkijkt

    private FirstPersonController playerController;
    private float defaultSensitivity;
    private float gazeTimer = 0f;
    private bool isLookingAtHole = false;

    void Start()
    {
        playerController = GetComponent<FirstPersonController>();
        if (playerController != null)
        {
            defaultSensitivity = playerController.mouseSensitivity;
        }
        else
        {
            Debug.LogError("BlackHoleGaze requires FirstPersonController on the same object.");
        }
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, blackHole.position) <= gazeRadius)
        {
            Vector3 toHole = (blackHole.position - playerController.playerCamera.transform.position).normalized;
            Vector3 lookDir = playerController.playerCamera.transform.forward;

            float dot = Vector3.Dot(toHole, lookDir);
            isLookingAtHole = dot > 0.95f;

            if (isLookingAtHole)
            {
                gazeTimer += Time.deltaTime * attractionSpeed;
            }
            else
            {
                gazeTimer -= Time.deltaTime * recoverySpeed;
            }

            // Aantrekking naar black hole
            Quaternion targetRotation = Quaternion.LookRotation(toHole);
            Quaternion currentRotation = playerController.playerCamera.transform.rotation;
            float attractionStrength = Mathf.Pow(gazeTimer, 2); // kwadratisch sterker

            playerController.playerCamera.transform.rotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                Time.deltaTime * attractionStrength * 2f
            );
        }
        else
        {
            gazeTimer -= Time.deltaTime * recoverySpeed;
        }

        gazeTimer = Mathf.Clamp01(gazeTimer);

        if (playerController != null)
        {
            playerController.mouseSensitivity = Mathf.Lerp(defaultSensitivity, minSensitivity, gazeTimer);
        }
    }
}
