using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class FileManagement : MonoBehaviour
{
    TerrainManager terrainManager;
    CarBehaviour carbehaviour;
    String filePath;

    // Variables that are stored in the excel file.
    String currentXPos, currentYPos, currentZPos;
    String difficulty, timer;
    String block;
    String points, currentlyCrashing, currentlyPassingCar, currentlyPassingRB;
    String passedX, passedY, passedZ, crashedCarX, crashedCarY, crashedCarZ, totalCrashedCar;
    String currentlyCrashingRB, crashedRBX, crashedRBY, crashedRBZ, timesCrashedRB;
    String leftArrowPressed, rightArrowPressed;

    // Use this for initialization
    void Start()
    {
        carbehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehaviour>();
        terrainManager = GameObject.FindGameObjectWithTag("terrainManager").GetComponent<TerrainManager>();

        currentXPos = ""; currentYPos = ""; currentZPos = "";
        difficulty = ""; timer = "";
        block = "";
        points = ""; currentlyPassingCar = ""; currentlyPassingRB = "";  
        currentlyCrashing = ""; crashedCarX = ""; crashedCarY = ""; crashedCarZ = ""; totalCrashedCar = "";
        currentlyCrashingRB = ""; crashedRBX = ""; crashedRBY = ""; crashedRBZ = ""; timesCrashedRB = "";
        leftArrowPressed = "0"; rightArrowPressed = "0";
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------- F I L E  &  D A T A  M A N A G E M E N T ----------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    void Update()
    {
        if (terrainManager.startTimer)
        {
            // The Current Block
            block = PlayerPrefs.GetInt("numberOfBlocks").ToString();

            // Gets variables regarding difficulty
            difficulty = terrainManager.getCurrentDifficultyLevel().ToString();
            timer = terrainManager.getTimer().ToString();

            // Gets the current X,Y,Z coordinates where the car is driving 
            currentXPos = carbehaviour.transform.position.x.ToString();
            currentYPos = carbehaviour.transform.position.y.ToString();
            currentZPos = carbehaviour.transform.position.z.ToString();

            // Variables for points and passing cars correctly
            points = carbehaviour.getPoints().ToString();
            currentlyPassingCar = carbehaviour.getCurrentlyPassingCar().ToString();
            currentlyPassingRB = carbehaviour.getCurrentlyPassingRB().ToString();

            // Variables for when car is crashing into another car
            currentlyCrashing = carbehaviour.getCurrentlyCrashingCar().ToString();
            crashedCarX = carbehaviour.getCrashedX().ToString();
            crashedCarY = carbehaviour.getCrashedY().ToString();
            crashedCarZ = carbehaviour.getCrashedZ().ToString();
            totalCrashedCar = carbehaviour.getTimesCrashedCar().ToString();

            // Variables for when car is crashing into a RoadBlock
            currentlyCrashingRB = carbehaviour.getCurrentlyCrashingRB().ToString();
            crashedRBX = carbehaviour.getCrashedRBX().ToString();
            crashedRBY = carbehaviour.getCrashedRBY().ToString();
            crashedRBZ = carbehaviour.getCrashedRBZ().ToString();
            timesCrashedRB = carbehaviour.getTimesCrashedRB().ToString();

            // Checks if the left/right keys are being pressed at that time
            if (Input.GetKeyDown(KeyCode.LeftArrow)) leftArrowPressed = "1";
            if (Input.GetKeyUp(KeyCode.LeftArrow)) leftArrowPressed = "0";
            if (Input.GetKeyDown(KeyCode.RightArrow)) rightArrowPressed = "1";
            if (Input.GetKeyUp(KeyCode.RightArrow)) rightArrowPressed = "0";

            //Debug.Log("getCurrentlyPassingRB: " + currentlyPassingRB);

            saveCarSpawnedDetails("0", "0", "0", "0", "0");
        }
    }

    // Declares the string that is saved into the csv file (with all the variables above in their correct columns)
    // Spawning is added separately because they happen in separately (in addition to every frame) so an extra line is added for them in the datafile
    public void saveCarSpawnedDetails(String insertType, String spawning, String x, String y, String z)
    {
        String spawningCar = "0", spawnCarX = "0", spawnCarY = "0", spawnCarZ = "0",
            spawningRB = "0", spawnRBX = "0", spawnRBY = "0", spawnRBZ = "0",
            obstacleCarX = "0", obstacleCarY = "0", obstacleCarZ = "0", obstacleRBX = "0", obstacleRBY = "0", obstacleRBZ = "0";
        String separator = ",";
        StringBuilder output = new StringBuilder();

        setDefault(); // Sets the default values if timer < 0.

        // Depending on where the method is being called, it will take in different arguments. This is for spawning:
        if (insertType.Equals("spawnCar")) { spawningCar = spawning; spawnCarX = x; spawnCarY = y; spawnCarZ = z; }
        if (insertType.Equals("spawnRB")) { spawningRB = spawning; spawnRBX = x; spawnRBY = y; spawnRBZ = z;  }

        // This is for when obstacles are passing the car:
        if (insertType.Equals("obstacleCar")) { obstacleCarX = x; obstacleCarY = y; obstacleCarZ = z; }
        if (insertType.Equals("obstacleRB")) { obstacleRBX = x; obstacleRBY = y; obstacleRBZ = z; }

        //Debug.Log("Obstacle " + insertType + ": "+ obstacleRBX + ", " + obstacleRBY + ", " + obstacleRBZ);

        // Basic Details about the state of the game
        String[] newLine = {block, timer, difficulty, "",

                // Current position and obstacles that are nearby
                currentXPos, currentYPos, currentZPos, obstacleCarX, obstacleCarY, obstacleCarZ, obstacleRBX, obstacleRBY, obstacleRBZ, "",
                points, currentlyPassingCar, currentlyPassingRB, "", // Current Points and succesful gameplay

                // Crashing into cars and roadblocks
                currentlyCrashing, crashedCarX, crashedCarY, crashedCarZ, totalCrashedCar, "", 
                currentlyCrashingRB, crashedRBX, crashedRBY, crashedRBZ, timesCrashedRB, "", 

                // Spawning cars and roadblocks
                spawningCar, spawnCarX, spawnCarY, spawnCarZ, 
                spawningRB, spawnRBX, spawnRBY, spawnRBZ, "", 

                leftArrowPressed, rightArrowPressed}; // Location of left/right arrows

        output.AppendLine(string.Join(separator, newLine));

        // Appends the new data to the file
        File.AppendAllText(filePath, output.ToString());
    }
    // returns the filePath so other classes can save data in the file
    public String getFilePath() { return filePath; }


    // ------------------------------------------------------------------------------------------------------ //
    // ----------------- C R E A T I N G   T H E   F I L E   A N D   H E A D I N G S ------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    public void createDataFile()
    {
        String appPath = Application.dataPath;
        String path = appPath.Substring(0, appPath.IndexOf("Asset")) + "Data";
        //String path = @"C:\Users\Sharl\Desktop\Files\UON - Research\2019 CompSci Honours\COMP4251\Imagine_street_racing\Data\2";
        Debug.Log(path);

        String separator = ",";
        StringBuilder output = new StringBuilder();

        String[] heading = { "Block", "In-Game Time", "DifficultyLevel", "",
            "CurrentX", "CurrentY", "CurrentZ", "obstacleCarX", "obstacleCarY", "obstacleCarZ", "obstacleRB-X", "obstacleRB-Y", "obstacleRB-Z", "",
            "Points", "PassingCar?", "PassingRB?", "",
            "Carcrash?", "CrashCarX", "CrashCarY", "CrashCarZ", "totalCarCrash", "",
            "RBCrash?", "CrashRB-X", "CrashRB-Y", "CrashRB-Z", "totalRBCrash", "",
            "CarSpawned?", "SpawnCarX", "SpawnCarY", "SpawnCarZ", "RBSpawned?", "SpawnRB-X", "SpawnRB-Y", "SpawnRB-Z", "",
            "LeftButtonDown", "RightButtonDown" };
        output.AppendLine(string.Join(separator, heading));

        filePath = path + @"\Participant-" + PlayerPrefs.GetString("Participant") + "-" + PlayerPrefs.GetString("Date") + "-Block" + (PlayerPrefs.GetInt("numberOfBlocks") + 1) + ".csv";

        File.AppendAllText(filePath, output.ToString());
    }

    public void setDefault()
    {
        // If the car is spawned before the timer starts, just set all the values to 0
        if (terrainManager.getTimer() < 0)
        {
            block = "0"; timer = "0"; difficulty = "0"; currentXPos = "0"; currentYPos = "0"; currentZPos = "0";
            points = "0"; currentlyPassingCar = "0"; currentlyPassingRB = "0";
            currentlyCrashing = "0"; crashedCarX = "0"; crashedCarY = "0"; crashedCarZ = "0"; totalCrashedCar = "0"; totalCrashedCar = "0";
            currentlyCrashingRB = "0"; crashedRBX = "0"; crashedRBY = "0"; crashedRBZ = "0"; currentlyCrashingRB = "0"; timesCrashedRB = "0";
        }

    }
}

