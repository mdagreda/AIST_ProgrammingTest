using UnityEngine;

/// <summary>
/// This class contains the settings for setting up the force sensor simulation
/// </summary>
[CreateAssetMenu(fileName = "SimulationSettings", menuName = "Custom/Simulation Settings")]
public class ForceSensorSimulationSettings : ScriptableObject
{
    /// <summary>
    /// The maximum force magnitude to be used for caluclating what color to diplay on the arrow
    /// </summary>
    public float maxForceMagnitude = 100f;

    /// <summary>
    /// The minimum transition time for simulating a change to the force or position of the force sensor
    /// </summary>
    public float minTransitionTime = 1f;

    /// <summary>
    /// The maximum transition time for simulating a change to the force or position of the force sensor
    /// </summary>
    public float maxTransitionTime = 5f;

    /// <summary>
    /// The minimum range for positions the force sensor can be simulated moving to.
    /// </summary>
    public Vector3 minPositions = new Vector3(-10, -10, -10);

    /// <summary>
    /// The maximum range for positions the force sensor can be simulated moving to.
    /// </summary>
    public Vector3 maxPositions = new Vector3(10, 10, 10);

    /// <summary>
    /// The minimum range for Orintations the force sensor can be simulated moving to.
    /// </summary>
    public Vector3 minOrintations = new Vector3(-180, -180, -180);

    /// <summary>
    /// The maximum range for Orintations the force sensor can be simulated moving to.
    /// </summary>
    public Vector3 maxOrintations = new Vector3(180, 180, 180);

    /// <summary>
    /// Color for low force readings
    /// </summary>
    public Color lowForceColor = Color.green;

    /// <summary>
    /// Color for high force readings
    /// </summary>
    public Color highForceColor = Color.red;
}
