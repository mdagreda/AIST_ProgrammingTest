using System;
using System.Diagnostics;

/// <summary>
/// This class simulates the feedback from a force sensor
/// </summary>
public class ForceSensorLibrary
{
    /// <summary>
    /// This vector 3 class can store a 3d vector and do a few vector math functions
    /// </summary>
    [Serializable]
    public struct Vector3
    {
        /// <summary>
        /// The following three variables are the three component variables of the vector
        /// </summary>
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Calculates the magnitude of the vector
        /// </summary>
        /// <returns>The magnitude of the vector</returns>
        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Lerps a vector from a to b
        /// </summary>
        /// <param name="start">The starting point of the lerp</param>
        /// <param name="end">The ending point of the lerp</param>
        /// <param name="percentComplete">the percentage of the way through the lerp represented as a value from 0 to 1</param>
        /// <returns>The lerped vector at the position in the lerp defined by the percent complete</returns>
        public static Vector3 Lerp(Vector3 start, Vector3 end, float percentComplete)
        {
            float x = start.X + (end.X - start.X) * percentComplete;
            float y = start.Y + (end.Y - start.Y) * percentComplete;
            float z = start.Z + (end.Z - start.Z) * percentComplete;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Normalizes the vector
        /// </summary>
        public void Normalize()
        {
            float magnitude = Magnitude();

            // Avoid dividing by zero
            if (magnitude != 0)
            {
                X /= magnitude;
                Y /= magnitude;
                Z /= magnitude;
            }
        }
    }

    /// <summary>
    /// A struct to hold the different reading to be simulated from the force sensor
    /// </summary>
    public struct ForceSensorData
    {
        public Vector3 ForceReading;

        public Vector3 Position;

        public Vector3 Orientation;
    }

    /// <summary>
    /// The class that simulates the feedback from the force sensor
    /// </summary>
    public class ForceSensorFeedback
    {
        /// <summary>
        /// This is used for the generating of random numbers
        /// </summary>
        private readonly System.Random random = new System.Random();

        /// <summary>
        /// The minimum transition time for changes to the sensor feedback
        /// </summary>
        private readonly float minTransitionTime = 1f;

        /// <summary>
        /// The maximum transition time for changes to the sensor feedback
        /// </summary>
        private readonly float maxTransitionTime = 5f;

        /// <summary>
        /// The maximum force magnitude
        /// </summary>
        private float maxForceMagnitude = 100f;

        /// <summary>
        /// The current transition time for the feedback changes
        /// </summary>
        private float currentTransitionTime = 0f;

        /// <summary>
        /// The time of the previous update
        /// </summary>
        private float previousTime = 0f;

        /// <summary>
        /// The current simulated position
        /// </summary>
        private Vector3 currentPosition;

        /// <summary>
        /// The current target position to move the simulated position to
        /// </summary>
        private Vector3 targetPosition;

        /// <summary>
        /// The current orientation of the simulated sensor
        /// </summary>
        private Vector3 currentOrientation;

        /// <summary>
        /// The target orientation to transition the simulated orintation to
        /// </summary>
        private Vector3 targetOrientation;

        /// <summary>
        /// The minimum positions that simulated sensor can move to
        /// </summary>
        public Vector3 minPositions;

        /// <summary>
        /// The maximum positions that simulated sensor can move to
        /// </summary>
        public Vector3 maxPositions;

        /// <summary>
        /// The minimum orintation that simulated sensor can move to
        /// </summary>
        public Vector3 minOrintations;

        /// <summary>
        /// The maximum orintation that simulated sensor can move to
        /// </summary>
        public Vector3 maxOrintations;

        /// <summary>
        /// The current force of the simulated sensor
        /// </summary>
        private Vector3 currentForce;

        /// <summary>
        /// The target amount of time to transition over
        /// </summary>
        private float targetTransitionTime;

        /// <summary>
        /// The target force to transition the simulated force to
        /// </summary>
        private Vector3 targetForce;

        /// <summary>
        /// The stopwatch to keep track of the time for the transitions in simulated feedback
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        /// Contructor that takes in all the settings needed to set the parameters of the simulated force sensor
        /// </summary>
        public ForceSensorFeedback(
            float maxForceMag, 
            float minTransTime, 
            float maxTransTime,
            Vector3 minPos,
            Vector3 maxPos,
            Vector3 minOri,
            Vector3 maxOri)
        {
            maxForceMagnitude = maxForceMag;
            minTransitionTime = minTransTime;
            maxTransitionTime = maxTransTime;
            minPositions = minPos;
            maxPositions = maxPos;
            minOrintations = minOri;
            maxOrintations = maxOri;

            // Set the first target force and position
            GenerateTargetForceAndTransitionTime();
            GenerateTargetPositionAndOrientation();

            stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Generates the simulated force sensor data
        /// </summary>
        /// <returns>Returns the simulated for sensor data</returns>
        public ForceSensorData GetForceSensorData()
        {
            // Get the current time and compare it to the last time read to see the time delta then add that 
            // to the current transition time.
            float currentTime = (float)stopwatch.Elapsed.TotalSeconds;

            currentTransitionTime += currentTime - previousTime;
            previousTime = currentTime;

            // If the current transition time is greater than or equal to the target transition then complete
            // the transition, generate new targets and reset the current transition time.
            if (currentTransitionTime >= targetTransitionTime)
            {
                currentPosition = targetPosition;
                currentOrientation = targetOrientation;
                GenerateTargetForceAndTransitionTime();
                GenerateTargetPositionAndOrientation();
                currentTransitionTime = 0f;
            }

            // Handle lerping the simulated force sensor data toward the target values.
            float t = currentTransitionTime / targetTransitionTime;
            Vector3 lerpedForce = Vector3.Lerp(currentForce, targetForce, t);
            Vector3 lerpedPosition = Vector3.Lerp(currentPosition, targetPosition, t);
            Vector3 lerpedOrientation = Vector3.Lerp(currentOrientation, targetOrientation, t);

            ForceSensorData sensorData = new ForceSensorData
            {
                Position = lerpedPosition,
                ForceReading = lerpedForce,
                Orientation = lerpedOrientation
            };

            return sensorData;
        }

        /// <summary>
        /// Generate the target force and transition time for the simulated force sensor data
        /// </summary>
        private void GenerateTargetForceAndTransitionTime()
        {
            targetTransitionTime = (float)(random.NextDouble() * (maxTransitionTime - minTransitionTime) + minTransitionTime);
            currentForce = targetForce;
            targetForce = GenerateRandomForceVector(maxForceMagnitude);
        }

        /// <summary>
        /// Generates a random force vector reading
        /// </summary>
        /// <param name="maxMagnitude">The maximum magnitude the force vector can be</param>
        /// <returns></returns>
        private Vector3 GenerateRandomForceVector(float maxMagnitude)
        {
            float random1 = (float)(random.NextDouble() * 2 - 1); // Random value between -1 and 1
            float random2 = (float)(random.NextDouble() * 2 - 1);
            float random3 = (float)(random.NextDouble() * 2 - 1);

            Vector3 randomVector = new Vector3(random1, random2, random3);
            randomVector.Normalize(); // Normalize the vector

            float scaleFactor = (float)random.NextDouble() * maxMagnitude;
            randomVector = new Vector3(randomVector.X * scaleFactor, randomVector.Y * scaleFactor, randomVector.Z * scaleFactor);

            return randomVector;
        }

        /// <summary>
        /// Generates the target positions and orientation for the force sensor.
        /// </summary>
        private void GenerateTargetPositionAndOrientation()
        {
            // Generate random positions
            float randomPositionX = GetRandomFloatInRange(minPositions.X, maxPositions.X);
            float randomPositionY = GetRandomFloatInRange(minPositions.Y, maxPositions.Y);
            float randomPositionZ = GetRandomFloatInRange(minPositions.Z, maxPositions.Z);

            // Generate random rotation angles
            float randomAngleX = GetRandomFloatInRange(minOrintations.X, maxOrintations.X);
            float randomAngleY = GetRandomFloatInRange(minOrintations.Y, maxOrintations.Y);
            float randomAngleZ = GetRandomFloatInRange(minOrintations.Z, maxOrintations.Z);

            targetPosition = new Vector3(randomPositionX, randomPositionY, randomPositionZ);
            targetOrientation = new Vector3(randomAngleX, randomAngleY, randomAngleZ);
        }

        /// <summary>
        /// Get a random float within a range.
        /// </summary>
        /// <param name="min">The minimum of the range</param>
        /// <param name="max">The maximum of the range</param>
        /// <returns></returns>
        private float GetRandomFloatInRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }

}

