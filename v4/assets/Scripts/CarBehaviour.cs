using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CarBehaviour : MonoBehaviour
{
    // Variables to check if the car is on the road
    public bool grounded;
    public LayerMask whatIsGround;

    // ----------------------------- S E R I A L I S E D   F I E L D S ---------------------------------------- //
    [Range(1f, 10f)]
    [SerializeField]
    float horizontalSpeed = 5f;

    float currentVerticalSpeed;

    [SerializeField]
    float accelerationTime = 2f;

    [SerializeField]
    AudioClip carSound = null;
    AudioSource carSource = null;
    Renderer renderer;

    [SerializeField]
    [Range(-3f, 3f)]
    float minSpeedPitch;
    [SerializeField]
    [Range(-3f, 3f)]
    float maxSpeedPitch;

    [SerializeField]
    AudioClip crashSound = null;
    AudioSource crashSource = null;

    // ----------------------------- D E C L A R E D   V A R I A B L E S ---------------------------------------- //
    public TerrainManager terrainManager;
    FileManagement fileManagement;
    Rigidbody rb;

    // Variables about points
    public Text pointsUI;
    int points;
    int numTimesCrashedCar, currentlyCrashingCar, currentlyPassingCar;
    int numTimesCrashedRB, currentlyCrashingRB, currentlyPassingRB;

    // Coordinates of where the car crashed
    float crashedX, crashedY, crashedZ;

    // Coordinates of where the roadblock crashed
    float crashedRBX, crashedRBY, crashedRBZ;

    // Coordinates of where the car passed without hitting
    float passedX, passedY, passedZ;

    string filePath;

    // ------------------------------------------------------------------------------------------------------ //
    // --------------------------------------------- S T A R T ---------------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    void Start()
    {
        // pointsUI.text = "Points: 0";
		pointsUI.text = ""; 
        points = 0;
        numTimesCrashedCar = 0; currentlyCrashingCar = 0;
        currentVerticalSpeed = 0;
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        rb.centerOfMass = rb.position - new Vector3(0, 0, 1);
        carSource = AudioHelper.CreateAudioSource(gameObject, carSound);
        crashSource = AudioHelper.CreateAudioSource(gameObject, crashSound);

        crashedX = 0; crashedY = 0; crashedZ = 0;
        crashedRBX = 0; crashedRBY = 0; crashedRBZ = 0;
        passedX = 0; passedY = 0; passedZ = 0;

        fileManagement = GameObject.FindGameObjectWithTag("fileManager").GetComponent<FileManagement>();
        filePath = fileManagement.getFilePath();
    }


    // ------------------------------------------------------------------------------------------------------ //
    // ----------------------------- G E T T E R S  &  S E T T E R S ---------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //

    // Getters for the points
    public int getPoints() { return points; }

    // Gets the X,Y,Z coordinates of where the car crashed
    public int getTimesCrashedCar() { return numTimesCrashedCar; }
    public int getCurrentlyCrashingCar() { return currentlyCrashingCar; }
    public float getCrashedX() { return crashedX; }
    public float getCrashedY() { return crashedY; }
    public float getCrashedZ() { return crashedZ; }

    // Gets the X,Y,Z coordinates of where the RB crashed
    public int getTimesCrashedRB() { return numTimesCrashedRB; }
    public int getCurrentlyCrashingRB() { return currentlyCrashingRB; }
    public float getCrashedRBX() { return crashedRBX; }
    public float getCrashedRBY() { return crashedRBY; }
    public float getCrashedRBZ() { return crashedRBZ; }

    // Gets the X,Y,Z coordinates of where the car passed without hitting
    public int getCurrentlyPassingCar() { return currentlyPassingCar; }
    public int getCurrentlyPassingRB() { return currentlyPassingRB; }

    void OnValidate()
    {
        minSpeedPitch = Mathf.Clamp(minSpeedPitch, -3f, maxSpeedPitch);
        maxSpeedPitch = Mathf.Clamp(maxSpeedPitch, minSpeedPitch, 3f);
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------------------------- U P D A T E S -------------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    void FixedUpdate()
    {

        if (GameplayManager.Instance.CanUpdateGame())
        {
            // Handle acceleration at beginning of game
            var speedIncrease = GameplayManager.Instance.GetCurrentTopSpeed() * Time.fixedDeltaTime / accelerationTime;
            // Clamp speed to current allowed top speed
            currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + speedIncrease, 0f, GameplayManager.Instance.GetCurrentTopSpeed());
            // Move car
            rb.position += new Vector3(0, 0, currentVerticalSpeed);
            // Update GameplayManager with speed
            GameplayManager.Instance.SetCurrentSpeed(currentVerticalSpeed);

            if (!carSource.isPlaying)
            {
                carSource.Play();
            }

            carSource.pitch = Mathf.Lerp(minSpeedPitch, maxSpeedPitch, currentVerticalSpeed / GameplayManager.Instance.MaxVerticalTopSpeed);
        }
        else
        {
            carSource.Stop();
        }
    }

    void Update()
    {
        if (GameplayManager.Instance.CanUpdateGame())
        {
            // Process left and right car movement
            Vector3 pos = rb.position;

            pos.x += (Input.GetAxis("Horizontal") * horizontalSpeed);

            pos.x = Mathf.Clamp(pos.x, -GameplayManager.Instance.HorizontalBounds, GameplayManager.Instance.HorizontalBounds);
            rb.position = pos;
        }
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ---------------------------------------- C O L L I S I O N S ----------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    void OnTriggerEnter(Collider other)
    {
        Vector3 pos = rb.position;

        // ---------------------------------------- FOR CARS -----------------------------------------------//
        // We only care if the car hits obstacles on the road -- colliding with anything else is fine
        if (other.gameObject.tag == "ObstacleCar")
        {
            // Count the number of times crashed.
            currentlyCrashingCar++;
            crashedX = pos.x; crashedY = pos.y; crashedZ = pos.z;

            // The player will not get any points if they crash their car.
            numTimesCrashedCar++;
            if (points > 0) points--;

            crashSource.Play();
        }
        // Resets all the variables back to 0 when it is no longer crashing
        else { currentlyCrashingCar = 0; crashedX = 0; crashedY = 0; crashedZ = 0; }

        // If they passed a car and did not hit it
        if (other.gameObject.tag == "PassedCar")
        {
            // +1 point for each car passed + counts how many cars passed at that point
            currentlyPassingCar++;
            
            points++;

            // Points update on the UI
            // pointsUI.text = "Points: " + points;
			pointsUI.text = "";
        }
        else { currentlyPassingCar = 0; }

        // ---------------------------------------- FOR ROADBLOCKS -----------------------------------------------//
        // We only care if the car hits obstacles on the road -- colliding with anything else is fine
        if (other.gameObject.tag == "roadBlock")
        {
            // Same as previous - just with the roadblock
            //Debug.Log(" --> Crashed into a roadblock at (" + pos.x + ", " + pos.y + ", " + pos.z + ")");

            // Records the data
            numTimesCrashedRB++;
            currentlyCrashingRB++;

           // Debug.Log("Currently Crashing RB: " + currentlyCrashingRB + ", Total times crashed: " + numTimesCrashedRB);

            crashedRBX = pos.x; crashedRBY = pos.y; crashedRBZ = pos.z;

            if (points > 0) points--;

            crashSource.Play();
        }
        // Resets all the variables back to 0 when it is no longer crashing
        else { currentlyCrashingRB = 0; crashedRBX = 0; crashedRBY = 0; crashedRBZ = 0; }

        if (other.gameObject.tag == "passedRoadblock")
        {
            // +1 point for each car passed + counts how many cars passed at that point
            points++;
            currentlyPassingRB++;

            // pointsUI.text = "Points: " + points;
			pointsUI.text = "";

            //Debug.Log("Currently Passing RB: " + currentlyPassingRB);
        }
        else { currentlyPassingRB = 0; }

        getAllPassingObstacles("cars", pos, terrainManager.objCars);
        getAllPassingObstacles("RBs", pos, terrainManager.objRBs);

       
    }

    public void getAllPassingObstacles(String listType, Vector3 pos, ArrayList objList)
    {
        // Gets the objects that the car is passing at that point.
        double obsX = 0, obsY = 0, obsZ = 0, rangeMin = 0, rangeMax = 0;

        // Gets the list of all coordinates of the cars that were spawned 
        for (int i = 0; i < objList.Count; i++)
        {
            // Depending on their spot in the array, they are either x, y or z coordinates
            if (i % 3 == 0) obsX = Convert.ToDouble(objList[i]);
            if (i % 3 == 1) obsY = Convert.ToDouble(objList[i]);
            if (i % 3 == 2)
            {
                obsZ = Convert.ToDouble(objList[i]);
                //Debug.Log(i + ": All Obstacle Cars: " + obsX + ", " + obsY + ", " + obsZ);

                rangeMin = obsZ - 50; rangeMax = obsZ + 50;
                if (pos.z > rangeMin && pos.z < rangeMax)
                {
                    //Debug.Log("passed obs: " + obsX.ToString() + ", " + obsY.ToString() + ", " + obsZ.ToString());

                    //Debug.Log(listType + ": " + obsX.ToString() + ", " + obsY.ToString() + ", " + obsZ.ToString());
                    if (listType.Equals("cars"))
                        fileManagement.saveCarSpawnedDetails("obstacleCar", "0", obsX.ToString(), obsY.ToString(), obsZ.ToString());
                    if (listType.Equals("RBs"))
                        fileManagement.saveCarSpawnedDetails("obstacleRB", "0", obsX.ToString(), obsY.ToString(), obsZ.ToString());
  
                }
            }
        }
    }
}
