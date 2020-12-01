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
    DRTStimulus drt;

    // Variables that are stored in the excel file.
    String difficulty, timer, block, currentXPos, currentYPos, currentZPos;
    String points, currentlyPassingCar, currentlyPassingRB;
    String currentlyCrashing, crashedX, crashedY, crashedZ, totalCrashed;
    String currentlyCrashingRB, crashedRBX, crashedRBY, crashedRBZ, timesCrashedRB;
    String leftArrowPressed, rightArrowPressed;
    String beepPlayed, waitingForResponse, pressedSpace, responseTime, noResponse, NoResponseTime;
    String baseline, mean, sd, criticalPoint;
    String movingAvg1, movingAvg2, movingAvg3, weightedMovingAvg1, weightedMovingAvg2, weightedMovingAvg3, WMATotal, difficultyAdjust;
    String critPointCounter, noCritPointCounter, DifficultyAdjust;

    // Use this for initialization
    void Start()
    {
        carbehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehaviour>();
        terrainManager = GameObject.FindGameObjectWithTag("terrainManager").GetComponent<TerrainManager>();
        drt = GameObject.FindGameObjectWithTag("Player").GetComponent<DRTStimulus>();
        setDefault();
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

            // Variables for when car is crashing
            totalCrashed = carbehaviour.getTimesCrashedCar().ToString();
            currentlyCrashing = carbehaviour.getCurrentlyCrashingCar().ToString();
            crashedX = carbehaviour.getCrashedX().ToString();
            crashedY = carbehaviour.getCrashedY().ToString();
            crashedZ = carbehaviour.getCrashedZ().ToString();

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

            saveCarSpawnedDetails("0", "0", "0", "0", "0");
        }
    }

    public void saveCarSpawnedDetails(String insertType, String input1, String input2, String input3, String input4)
    {
        String spawningCar = "0", spawnCarX = "0", spawnCarY = "0", spawnCarZ = "0",
            spawningRB = "0", spawnRBX = "0", spawnRBY = "0", spawnRBZ = "0",
            obstacleCarX = "0", obstacleCarY = "0", obstacleCarZ = "0", obstacleRBX = "0", obstacleRBY = "0", obstacleRBZ = "0";

        // Depending on where the method is being called, it will take in different arguments. This is for spawning:
        if (insertType.Equals("spawnCar"))
            { spawningCar = input1; spawnCarX = input2; spawnCarY = input3; spawnCarZ = input4; }

        if (insertType.Equals("spawnRB"))
            { spawningRB = input1; spawnRBX = input2; spawnRBY = input3; spawnRBZ = input4; }

        // This is for when obstacles are passing the car:
        if (insertType.Equals("obstacleCar"))
            { obstacleCarX = input2; obstacleCarY = input3; obstacleCarZ = input4; }

        if (insertType.Equals("obstacleRB"))
            { obstacleRBX = input2; obstacleRBY = input3; obstacleRBZ = input4; }

        // When the beep sound is played (catches their response/ no response)
        if (insertType.Equals("DRTResponse"))
            { beepPlayed = input1; waitingForResponse = input2; pressedSpace = input3; responseTime = input4; }

        if (insertType.Equals("NoDRTResponse"))
            { noResponse = input1; NoResponseTime = input2; }

        // Variables saved to file when calculating the baseline:
        if (insertType.Equals("baseline"))
            { baseline = input1; mean = input2; sd = input3; criticalPoint = input4; }

        // The moving averages (before weighting them)
        if (insertType.Equals("movingAverage"))
            { movingAvg1 = input1; movingAvg2 = input2; movingAvg3 = input3; }

        // The moving averages (after weighting them) + the final WMA and resulted difficulty changes.
        if (insertType.Equals("weightedMovingAverage"))
            { weightedMovingAvg1 = input1; weightedMovingAvg2 = input2; weightedMovingAvg3 = input3; WMATotal = input4; }

        // When hitting the critical point a certain number of times, difficulty either goes up or down:
        if (insertType.Equals("difficultyAdjust"))
            { critPointCounter = input1; noCritPointCounter = input2; difficultyAdjust = input3;  }

        // ---------------------------------------------------------------------------------------------------- //
        //                                     SAVES THE DATA TO THE CSV FILE
        // ---------------------------------------------------------------------------------------------------- //
        String separator = ",";
        StringBuilder output = new StringBuilder();

        if (terrainManager.getTimer() < 0) setDefault();

        // Declares the string that is saved into the csv file (with all the variables above in their correct columns)
        String[] newLine = {block, timer, difficulty, "",
                currentXPos, currentYPos, currentZPos, obstacleCarX, obstacleCarY, obstacleCarZ, obstacleRBX, obstacleRBY, obstacleRBZ, "",
                points, currentlyPassingCar, currentlyPassingRB, "",

                // Crashing into RB or Car
                currentlyCrashing, crashedX, crashedY, crashedZ, totalCrashed, "",
                currentlyCrashingRB, crashedRBX, crashedRBY, crashedRBZ, timesCrashedRB, "",

                // DRT Data:
                beepPlayed, waitingForResponse, pressedSpace, responseTime, "",
                noResponse, NoResponseTime, "",
                
                // Baseline and statistics:
                baseline, mean, sd, criticalPoint, "",

                // both moving averages and weighted MAs for when determining the difficulty:
                movingAvg1, movingAvg2, movingAvg3, "",
                weightedMovingAvg1, weightedMovingAvg2, weightedMovingAvg3, WMATotal, "",

                // When hitting the critical point a certain number of times, difficulty either goes up or down:
                critPointCounter, noCritPointCounter, difficultyAdjust, "",

                spawningCar, spawnCarX.ToString(), spawnCarY.ToString(), spawnCarZ.ToString(), spawningRB, spawnRBX, spawnRBY, spawnRBZ, "",
                leftArrowPressed, rightArrowPressed};

        output.AppendLine(string.Join(separator, newLine));

        // Appends the new data to the file
        File.AppendAllText(filePath, output.ToString());
    }
    

    // ------------------------------------------------------------------------------------------------------ //
    // ----------------- C R E A T I N G   T H E   F I L E   A N D   H E A D I N G S ------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    public void createDataFile()
    {
        String appPath = Application.dataPath;
        String path = appPath.Substring(0, appPath.IndexOf("Asset")) + "Data";
        //String path = @"C:\Users\Sharl\Desktop\Files\UON - Research\2019 CompSci Honours\COMP4251\Imagine_street_racing\Data\4";
        Debug.Log(path);

        String separator = ",";
        StringBuilder output = new StringBuilder();

        // The heading for the file.
        String[] heading = { "Block", "In-Game Time", "DifficultyLevel", "",
            "CurrentX", "CurrentY", "CurrentZ", "obstacleCarX", "obstacleCarY", "obstacleCarZ", "obstacleRBX", "obstacleRBY", "obstacleRBZ", "",
            "Points", "PassingCar?", "PassingRB?", "",

            // Car or Roadblock crashes:
            "Carcrash?", "CrashCarX", "CrashCarY", "crashCarZ", "totalCarCrash", "",
            "RBCrash?", "CrashRB-X", "CrashRB-Y", "CrashRB-Z", "totalRBCrash", "",

            // DRT Data:
            "BeepPlayed", "WaitingForResponse", "SpacePressed", "responseTime", "",
            "noResponse", "NoResponseTime", "",

            // Baseline:
            "Baseline", "Mean", "SD", "CriticalPoint", "",

            // WMAs for when determining the difficulty
            "movingAvg1", "movingAvg2", "movingAvg3", "",
            "movingAvg1*0.5", "movingAvg2", "movingAvg3*2", "WMATotal", "",

            // When hitting the critical point a certain number of times, difficulty either goes up or down:
            "critPointCounter", "noCritPointCounter", "DifficultyAdjust", "",

            "CarSpawned?", "SpawnCarX", "SpawnCarY", "SpawnCarZ", "RBSpawned?", "SpawnRB-X", "SpawnRB-Y", "SpawnRB-Z", "",
            "LeftButtonDown", "RightButtonDown" };

        output.AppendLine(string.Join(separator, heading));

        filePath = path + @"\Participant-" + PlayerPrefs.GetString("Participant") + "-" + PlayerPrefs.GetString("Date") + "-Block" + (PlayerPrefs.GetInt("numberOfBlocks") + 1) + ".csv";

        File.AppendAllText(filePath, output.ToString());
    }

    public void setDefault()
    {
        // If the car is spawned before the timer starts, just set all the values to 0
        block = "0"; timer = "0"; difficulty = "0"; currentXPos = "0"; currentYPos = "0"; currentZPos = "0";
        points = "0"; currentlyPassingCar = "0"; currentlyPassingRB = "0";
        currentlyCrashing = "0"; crashedX = "0"; crashedY = "0"; crashedZ = "0"; totalCrashed = "0";
        currentlyCrashingRB = "0"; crashedRBX = "0"; crashedRBY = "0"; crashedRBZ = "0"; currentlyCrashingRB = "0"; timesCrashedRB = "0";
        beepPlayed = "0";  waitingForResponse = "0"; pressedSpace = "0"; responseTime = "0";
        noResponse = "0"; NoResponseTime = "0";
        baseline = "0"; mean = "0"; sd = "0";  criticalPoint = "0";
        movingAvg1 = "0"; movingAvg2 = "0"; movingAvg3 = "0";
        weightedMovingAvg1 = "0"; weightedMovingAvg2 = "0"; weightedMovingAvg3 = "0"; WMATotal = "0";
        critPointCounter = "0"; noCritPointCounter = "0";  difficultyAdjust = "0";
        leftArrowPressed = "0"; rightArrowPressed = "0";     
    }

    // returns the filePath so other classes can save data in the file
    public String getFilePath() { return filePath; }
}

