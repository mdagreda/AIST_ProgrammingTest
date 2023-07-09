using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class visualizes simulated force sensor data
/// </summary>
public class ForceSensorVisualizor : MonoBehaviour
{
    /// <summary>
    /// The Arrow image to change the color of depending on the force magnitude
    /// </summary>
    [SerializeField]
    private Image arrowImage;

    /// <summary>
    /// Color for low force readings
    /// </summary>
    private Color lowForceColor = Color.green;

    /// <summary>
    /// Color for high force readings
    /// </summary>
    private Color highForceColor = Color.red;

    /// <summary>
    /// The settings for the force sensor simulation
    /// </summary>
    [SerializeField]
    private ForceSensorSimulationSettings simulationSettings;

    /// <summary>
    /// The sensor feedback simulation object
    /// </summary>
    private ForceSensorLibrary.ForceSensorFeedback sensorFeedback;

    private void Start()
    {
        // creating the force sensor simulation with the desired settings
        sensorFeedback = new ForceSensorLibrary.ForceSensorFeedback(
            simulationSettings.maxForceMagnitude,
            simulationSettings.minTransitionTime,
            simulationSettings.maxTransitionTime,
            ToForceSensorLibVector3(simulationSettings.minPositions),
            ToForceSensorLibVector3(simulationSettings.maxPositions),
            ToForceSensorLibVector3(simulationSettings.minOrintations),
            ToForceSensorLibVector3(simulationSettings.maxOrintations)) ;

        highForceColor = simulationSettings.highForceColor;

        lowForceColor = simulationSettings.lowForceColor;
    }

    private void Update()
    {
        UpdateForceSensor();
    }

    /// <summary>
    /// Updates the force sensor arrow visualization
    /// </summary>
    private void UpdateForceSensor()
    {
        // Get the force sensor data
        ForceSensorLibrary.ForceSensorData sensorData = sensorFeedback.GetForceSensorData();

        // Calculate the magnitude of the force reading
        float magnitude = Mathf.Sqrt(
            sensorData.ForceReading.X * sensorData.ForceReading.X +
            sensorData.ForceReading.Y * sensorData.ForceReading.Y +
            sensorData.ForceReading.Z * sensorData.ForceReading.Z
        );

        // Set the position and rotation of the arrow
        this.transform.position = ToUnityVector3(sensorData.Position);
        this.transform.rotation = Quaternion.LookRotation(ToUnityVector3(sensorData.ForceReading));

        // Uses the sensor rotation data
        //this.transform.rotation =  Quaternion.Euler(ToUnityVector3(sensorData.Orientation));

        if(magnitude < 0)
        {
            Debug.LogError("Should not get a negative magnitude");
        }

        // Change the color of the arrow based on the magnitude
        arrowImage.color = Color.Lerp(lowForceColor, highForceColor, magnitude / simulationSettings.maxForceMagnitude);
    }

    /// <summary>
    /// Converts a force sensor library Vector3 to a Unity Vector3
    /// </summary>
    /// <param name="vectorToConvert">tThe unity Vector3 to convert</param>
    /// <returns>The converted unity Vector3</returns>
    private Vector3 ToUnityVector3(ForceSensorLibrary.Vector3 vectorToConvert)
    {
        return new Vector3(vectorToConvert.X, vectorToConvert.Y, vectorToConvert.Z);
    }

    /// <summary>
    /// Converts a Unity Vector3 to a force sensor library Vector3
    /// </summary>
    /// <param name="vectorToConvert">tThe unity Vector3 to convert</param>
    /// <returns>The converted force sensor library Vector3</returns>
    private ForceSensorLibrary.Vector3 ToForceSensorLibVector3(Vector3 vectorToConvert)
    {
        return new ForceSensorLibrary.Vector3(vectorToConvert.x, vectorToConvert.y, vectorToConvert.z);
    }
}

