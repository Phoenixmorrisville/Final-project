
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarHUD : MonoBehaviour
{
    public CarController car;
    public Rigidbody carRigidbody;

    [Header("Speedometer")]
    public TextMeshProUGUI speedText;
    public Image speedBar;

    [Header("Other UI")]
    public TextMeshProUGUI obstacleText;
    public GameObject brakeIndicator;

    void Update()
    {
        if (car == null || carRigidbody == null)
            return;

        // Convert the car's speed from m/s to MPH.
        float speedMPH =
            carRigidbody.linearVelocity.magnitude * 2.23694f;

        // Display current speed in MPH.
        if (speedText != null)
        {
            speedText.text =
                Mathf.RoundToInt(speedMPH) + " MPH";
        }

        // Fill the speed bar based on the MPH top speed.
        if (speedBar != null)
        {
            speedBar.fillAmount = Mathf.Clamp01(
                speedMPH / Mathf.Max(car.topSpeed, 0.01f)
            );
        }

        // Update obstacle counter.
        if (obstacleText != null)
        {
            obstacleText.text =
                "Obstacles Hit: " + car.obstacleHits;
        }

        // Show brake indicator when braking.
        if (brakeIndicator != null)
        {
            brakeIndicator.SetActive(car.isBraking);
        }
    }
}
