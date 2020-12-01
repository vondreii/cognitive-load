using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public GameplayManager gm;
    public GameObject[] myPlanes; // An array of all the planes/terrains
    public Baseline baseline;
    CarBehaviour carbehaviour;
    public FileManagement fileManagement;
    DifficultySettings difficultySettings;
    BlockManager block;

    // public variables
    public String filePath;
    public float offscreenSpawnOffset = 1000f;

    // private global variables
    int seconds, times, critPointCounter, noCritPointCounter, previousLanes;
    float timer, planeEdgeZ;
    Boolean first, createBaselineNow;
    public Boolean startTimer;

    public ArrayList objRBs = new ArrayList();
    public ArrayList objCars = new ArrayList();

    // Variables that will control how difficult the game is 
    int startDistance; // how far from the player the cars will spawn 
    int distance; // distance between the cars
    int numberOfCars; // How many cars can spawn at once
    int difficultyLvl; // Just to label 

    // Variables for the cars being spawned
    float spawnCarX, spawnCarY, spawnCarZ;

    // ------------------------------------------------------------------------------------------------------ //
    // --------------------------------- S T A R T  &  U P D A T E ------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    void Start()
    {
        carbehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehaviour>();
        baseline = GameObject.FindGameObjectWithTag("baseline").GetComponent<Baseline>();
        fileManagement = GameObject.FindGameObjectWithTag("fileManager").GetComponent<FileManagement>();
        difficultySettings = GameObject.FindGameObjectWithTag("Difficulty").GetComponent<DifficultySettings>();
        block = GameObject.FindGameObjectWithTag("block").GetComponent<BlockManager>();
        

        // Start spawning further down so we get an edge at the bottom on start
        planeEdgeZ = -1000f; times = 0; previousLanes = 0;
        SpawnNewPlane(0);

        seconds = 0; timer = -3f;
        startTimer = false; first = true; createBaselineNow = true;
        spawnCarX = 0; spawnCarY = 0; spawnCarZ = 0;

        // When the start the game, the game is easier spawing less cars with longer distances between them
        startDistance = 500; numberOfCars = 1; distance = 300; difficultyLvl = 1;

        fileManagement.createDataFile();
        filePath = fileManagement.getFilePath();

        PlayerPrefs.SetInt("startingNewBlock", 0);
        PlayerPrefs.SetInt("numberOfBlocks", PlayerPrefs.GetInt("numberOfBlocks") + 1);
    }

    void Update()
    {
        var maxZ = CameraController.GetMaxZ();
        int currentTime = 0;

        if (startTimer)
        {
            // A timer for how long the player has been playing
            timer += Time.deltaTime;
            currentTime = System.Convert.ToInt32(timer); // Only counts the seconds     

            // Update the file of the state
            //File.AppendAllText(filePath, "\r\nCurrent Time: " + timer );
        }
        if (currentTime >= 300) endGame(currentTime);

        // Calculates the difficulty
        difficultySettings.setDifficultySettings(difficultyLvl);

        // As soon as 60 seconds pass, it will create the baseline of the current data collected. 
        if (timer >= 60)
        {
            // Ensures it is only created once, when it passes the 60 second mark.
            if (createBaselineNow)
            {
                difficultyLvl = 2;
                //File.AppendAllText(filePath, " --> Increase difficulty to " + difficultyLvl);

                // Creates the baseline, and then never creates the baseline again.
                baseline.createBaseline(); createBaselineNow = false;
            }
        }

        // Spawn a new plane if we need to
        if (planeEdgeZ - maxZ <= offscreenSpawnOffset)
        {
            // Calls the methods to spawn the terrain
            SpawnNewPlane(offscreenSpawnOffset);

            // Depending on the difficulty, it will spawn x number of cars
            for (int i = 0; i <= numberOfCars; i++)
            {
                // The cars will spawn at x distance from the previously spawned car
                spawnCars(startDistance, startDistance);  startDistance += distance;
            }
        }

        
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ----------------------------- G E T T E R S  &  S E T T E R S ---------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //

    // Gets the current difficulty level the player is playing in
    public int getCurrentDifficultyLevel() { return difficultyLvl; }

    // Gets the current difficulty level the player is playing in
    public void setNumCars(int numCars) { numberOfCars = numCars; }

    // Gets the current difficulty level the player is playing in
    public void setStartDistance(int start) { startDistance = start; }

    // Gets the current difficulty level the player is playing in
    public void setDistance(int dis) { distance = dis; }

    // Sets the state of the timer and determines when it should start
    public void setTimer(Boolean state) { startTimer = state; timer = -3f; }

    // Sets the state of the timer and determines when it should start
    public float getTimer() { return timer; }

    // Gets the current difficulty level the player is playing in
    public void setDifficultyLevel(int newDifficulty) { difficultyLvl = newDifficulty; }

    // Spawn Car X, Y, Z  coordinates
    public float getSpawnCarX() { return spawnCarX; }
    public float getSpawnCarY() { return spawnCarY; }
    public float getSpawnCarZ() { return spawnCarZ; }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------- D I F F I C U L T Y  C O N T R O L L E R ----------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    public void manageDifficulty(Boolean responded, float responseTime)
    {
        // Logs in the file that they have responded
        // Saves all their response times in arrays within the Baseline class (keeps track of the data).
        // When <= 60 seconds, adds to baselineArray, when > 60 seconds, adds it to the moving averages array
        baseline.addResponseTime(responseTime, timer);

        // Gets the moving average of the data
        float WMA = baseline.getMovingAverage(responseTime, timer);

        if (timer < 60) baseline.getAllResponseTimes("baseline");
        else if (timer < 60) baseline.getAllResponseTimes("movingAverage");

        if (WMA > 0)
        {
            // Compares if their moving average is greater than the critical point.
            // Counts if they hit the critical point 3 times. 
            if (WMA > baseline.getCritPoint()) critPointCounter++; 
            else noCritPointCounter++;
            
            // If they have made mistakes for multiple times, the difficulty decreases.
            if (WMA > baseline.getCritPoint() && critPointCounter >= 2)
            {
                critPointCounter = 0;
                if (difficultyLvl > 1) difficultyLvl--;

                // Debug.Log("crit: " + critPointCounter + ", noCrit: " + noCritPointCounter + "decrease by -1 to " + difficultyLvl);
                fileManagement.saveCarSpawnedDetails("difficultyAdjust", critPointCounter.ToString(), noCritPointCounter.ToString(), "-1", "0");
            }

            // Otherwise, if they are consistently doing well, the difficulty will increase.
            else if (WMA < baseline.getCritPoint() && noCritPointCounter >= 2)
            {
                noCritPointCounter = 0; 
                if (difficultyLvl < 10) difficultyLvl++;

                // Debug.Log("crit: " + critPointCounter + ", noCrit: " + noCritPointCounter + "increase by +1 to " + difficultyLvl);
                fileManagement.saveCarSpawnedDetails("difficultyAdjust", critPointCounter.ToString(), noCritPointCounter.ToString(), "+1", "0");
            }

            fileManagement.saveCarSpawnedDetails("difficultyAdjust", critPointCounter.ToString(), noCritPointCounter.ToString(), "0", "0");
        }
    }

    // ------------------------------------------------------------------------------------------------------ //
    // -------------------------------------- E N D  G A M E ------------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    void endGame(int currentTime)
    {
        // Should stop the game after 5 minutes and save the data
        setTimer(false);
        timer = -3f;
        currentTime = 0;
        block.startNewBlock();
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------------------ S P A W N   C A R S --------------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    void spawnCars(float offset, int distance)
    {
        // Selects a random car from the list, gets the name and makes an instance of the car 
        var randomCarName = myPlanes[UnityEngine.Random.Range(4, myPlanes.Length)].name;
        var car = ObjectPooler.Instance.GetPooledObject(randomCarName);
        
        // The car will spawn either in the right, left or middle lanes
        int lane = UnityEngine.Random.Range(0, 3);

        // Depending on which lane they spawn in, the x variable will change.
        int[] lanes = { -52, 0, 52 };

        while (previousLanes == lane) lane = UnityEngine.Random.Range(0, 3);
        previousLanes = lane;

        if (car != null)
        {
            spawnCarX = lanes[lane];
            spawnCarY = 3.5f;
            spawnCarZ = GameplayManager.Instance.CurrentZPos + offset + distance;

            // Place terrain on screen
            car.transform.position = new Vector3(spawnCarX, spawnCarY, spawnCarZ);
            car.gameObject.SetActive(true);

            // If the spawned obstacle is a roadblock, store it in the 'spawned roadblock' columns in the datafile
            if (!randomCarName.Equals("RoadBlock01"))
            {
                // Saves the coordinates where a car has already spawned
                objCars.Add(spawnCarX); objCars.Add(spawnCarY); objCars.Add(spawnCarZ);
                fileManagement.saveCarSpawnedDetails("spawnCar", "1", spawnCarX.ToString(), spawnCarY.ToString(), spawnCarZ.ToString());

            }
            else
            {
                // Saves the coordinates where an RB has already spawned
                objRBs.Add(spawnCarX); objRBs.Add(spawnCarY); objRBs.Add(spawnCarZ);
                fileManagement.saveCarSpawnedDetails("spawnRB", "1", spawnCarX.ToString(), spawnCarY.ToString(), spawnCarZ.ToString());

                //Debug.Log("spawned RB: " + spawnCarX.ToString() + ", " + spawnCarY.ToString() + ", " + spawnCarZ.ToString());
            }
        }

    }

    // ------------------------------------------------------------------------------------------------------ //
    // --------------------------------- S P A W N   T E R R A I N ------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    void SpawnNewPlane(float offset)
    {
        // Selects a random plane from the list of planes, gets the name and makes an instance of the plane
        var randomPlaneName = myPlanes[UnityEngine.Random.Range(0, 4)].name;
        var plane = ObjectPooler.Instance.GetPooledObject(randomPlaneName);

        // Because the car is moving, the off screen spawning point is moved further
        // When the game starts, the road has already spawned (so we will not spawn anything)
        if (first)
        {
            planeEdgeZ += offscreenSpawnOffset;

            // Counts how many roads have been spawned before spawning a new road
            times++;
            if(times == 1) first = false;
        }
        else
        {
            planeEdgeZ += offscreenSpawnOffset;

            // For the rest of the game, spawn the roads.
            if (plane != null)
            {
                // Places the terrain onto the screen before the car reaches it
                plane.transform.position = new Vector3(transform.position.x, transform.position.y, GameplayManager.Instance.CurrentZPos + offset);
                plane.gameObject.SetActive(true);
            }
        }
    }
}
