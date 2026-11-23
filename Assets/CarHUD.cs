
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
        // Convert meters per second to MPH.
        float speedMPH = carRigidbody.linearVelocity.magnitude * 2.23694f;

        // Display rounded speed.
        speedText.text = Mathf.RoundToInt(speedMPH) + " MPH";

        // Fill the speed bar based on topSpeed.
        if (speedBar != null)
        {
            speedBar.fillAmount = Mathf.Clamp01(
                carRigidbody.linearVelocity.magnitude / car.topSpeed
            );
        }

        // Update obstacle counter.
        if (obstacleText != null)
        {
            obstacleText.text = "Obstacles Hit: " + car.obstacleHits;
        }

        // Show brake indicator when braking.
        if (brakeIndicator != null)
        {
            brakeIndicator.SetActive(car.isBraking);
        }
    }
}
