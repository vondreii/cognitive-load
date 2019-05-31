using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class TerrainManager : MonoBehaviour
{
    public GameplayManager gm;
    FileManagement fileManagement;
    CarBehaviour carbehaviour;
    DifficultySettings difficultySettings;
    BlockManager block;

    public GameObject[] myPlanes;

    public String filePath;
    public float offscreenSpawnOffset = 1000f;
    public int counter = 0;

    int seconds, times, difficultyLvl, previousLanes;
    float timer, planeEdgeZ;
    Boolean first;
    public Boolean startTimer;

    public ArrayList objRBs = new ArrayList();
    public ArrayList objCars = new ArrayList();

    // Variables for the cars being spawned
    float spawnCarX, spawnCarY, spawnCarZ;

    // Variables that will control how difficult the game is 
    public int startDistance = 0; // how far from the player the cars will spawn 
    public int distance = 0; // distance between the cars
    public int numberOfCars = 0; // How many cars can spawn at once

    // Use this for initialization
    // ------------------------------------------------------------------------------------------------------ //
    // --------------------------------- S T A R T  &  U P D A T E ------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    void Start()
    {
        carbehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehaviour>();
        fileManagement = GameObject.FindGameObjectWithTag("fileManager").GetComponent<FileManagement>();
        difficultySettings = GameObject.FindGameObjectWithTag("Difficulty").GetComponent<DifficultySettings>();
        block = GameObject.FindGameObjectWithTag("block").GetComponent<BlockManager>();

        // Start spawning further down so we get an edge at the bottom on start
        planeEdgeZ = -1000f; SpawnNewPlane(0);

        difficultyLvl = 0;
        counter = 1; seconds = 0;
        startTimer = false; timer = -3f;
        first = true; times = 0;
        previousLanes = 0;

        // Data File created
        fileManagement.createDataFile();
        filePath = fileManagement.getFilePath();

        spawnCarX = 0; spawnCarY = 0; spawnCarZ = 0;

        //PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("startingNewBlock", 0);
        PlayerPrefs.SetInt("numberOfBlocks", PlayerPrefs.GetInt("numberOfBlocks")+1);
    }

    void Update()
    {

        var maxZ = CameraController.GetMaxZ();

        int currentTime = 0;

        if (startTimer)
        {
            //Debug.Log("Time.deltaTime: " + Time.deltaTime);
            // A timer for how long the player has been playing
            timer += Time.deltaTime;
            currentTime = System.Convert.ToInt32(timer); // Only counts the seconds

            // Update the file of the state

            //File.AppendAllText(filePath, "" + timer + ", Difficulty: " + difficultyLvl);
            
        }
        if (currentTime >= 300) endGame(timer); // Ends game and starts new block

        // When the start the game, the game is easier spawing less cars with longer distances between them
        // The longer the player has been playing the game, the more cars spawn with shorter distances between them
        difficultySettings.setDifficultySettings(timer);

        // Spawn a new plane if we need to
        if (planeEdgeZ - maxZ <= offscreenSpawnOffset)
        {
            // Calls the methods to spawn the terrain
            SpawnNewPlane(offscreenSpawnOffset);

            // Depending on the difficulty, it will spawn x number of cars
            for (int i = 0; i <= numberOfCars; i++)
            {
                // The cars will spawn at x distance from the previously spawned car
                spawnCars(startDistance, startDistance); startDistance += distance;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ----------------------------- G E T T E R S  &  S E T T E R S ---------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //

    public float getTimer() { return timer; }

    // Gets the current difficulty level the player is playing in
    public int getCurrentDifficultyLevel() { return difficultyLvl; }

    // Gets the current difficulty level the player is playing in
    public void setDifficultyLevel(int newDifficulty) { difficultyLvl = newDifficulty; }

    // Gets the current difficulty level the player is playing in
    public void setNumCars(int numCars) { numberOfCars = numCars; }

    // Gets the current difficulty level the player is playing in
    public void setStartDistance(int start) { startDistance = start; }

    // Gets the current difficulty level the player is playing in
    public void setDistance(int dis) { distance = dis; }

    // Sets the state of the timer and determines when it should start
    public void setTimer(Boolean state) { startTimer = state; timer = -3f; }

    // Spawn Car X, Y, Z  coordinates
    public float getSpawnCarX() { return spawnCarX; }
    public float getSpawnCarY() { return spawnCarY; }
    public float getSpawnCarZ() { return spawnCarZ; }

    // ------------------------------------------------------------------------------------------------------ //
    // -------------------------------------- E N D  G A M E ------------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    void endGame(float currentTime)
    {
        // 5 minutes (test) 
        // Should stop the game after some minutes and save the data

        // gm.OnGameOver();
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

        // Depending on which lane they spawn in, the x variable will change.
        int[] lanes = { -60, 0, 60 };

        // The car will spawn either in the right, left or middle lanes
        int lane = UnityEngine.Random.Range(0, 3);

        while(previousLanes == lane) lane = UnityEngine.Random.Range(0, 3);

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
            }

            //Debug.Log("A car was spawned. (x,y,z): (" + lanes[lane] + ", " + transform.position.y + ", " + (GameplayManager.Instance.CurrentZPos + offset + distance) + ")");
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

        // Trying to fix glitch but nothing works
        if (first)
        {
            planeEdgeZ += offscreenSpawnOffset;

            // Counts how many roads have been spawned before spawning a new road
            times++;
            if (times == 1) first = false;
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