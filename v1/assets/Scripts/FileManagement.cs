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
    String difficulty, timer, block, currentXPos, currentYPos, currentZPos;
    String points, currentlyPassingCar;
    String currentlyCrashing, crashedX, crashedY, crashedZ, totalCrashed;
    String leftArrowPressed, rightArrowPressed;

    // Use this for initialization
    void Start()
    {
        carbehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehaviour>();
        terrainManager = GameObject.FindGameObjectWithTag("terrainManager").GetComponent<TerrainManager>();

        currentXPos = ""; currentYPos = ""; currentZPos = "";
        difficulty = ""; timer = ""; block = "";
        points = ""; currentlyPassingCar = ""; 
        currentlyCrashing = ""; crashedX = ""; crashedY = ""; crashedZ = ""; totalCrashed = "";
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

            // Variables for when car is crashing
            totalCrashed = carbehaviour.getTimesCrashed().ToString();
            currentlyCrashing = carbehaviour.getCurrentlyCrashing().ToString();
            crashedX = carbehaviour.getCrashedX().ToString();
            crashedY = carbehaviour.getCrashedY().ToString();
            crashedZ = carbehaviour.getCrashedZ().ToString();

            // Checks if the left/right keys are being pressed at that time
            if (Input.GetKeyDown(KeyCode.LeftArrow)) leftArrowPressed = "1";
            if (Input.GetKeyUp(KeyCode.LeftArrow)) leftArrowPressed = "0";
            if (Input.GetKeyDown(KeyCode.RightArrow)) rightArrowPressed = "1";
            if (Input.GetKeyUp(KeyCode.RightArrow)) rightArrowPressed = "0";

            saveCarSpawnedDetails("0", spawnCarX: 0, spawnCarY: 0, spawnCarZ: 0, obstacleX: "0", obstacleY: "0", obstacleZ: "0");
            
        }
    }

    public void saveCarSpawnedDetails(String spawned, float spawnCarX, float spawnCarY, float spawnCarZ,
        String obstacleX, String obstacleY, String obstacleZ)
    {
        String separator = ",";
        StringBuilder output = new StringBuilder();

        setDefault();

       // Declares the string that is saved into the csv file (with all the variables above in their correct columns)
       String[] newLine = {block, timer, difficulty, "", 
                currentXPos, currentYPos, currentZPos, obstacleX, obstacleY, obstacleZ, "",
                points, currentlyPassingCar, "",
                currentlyCrashing, crashedX, crashedY, crashedZ, totalCrashed, "",
                spawned, spawnCarX.ToString(), spawnCarY.ToString(), spawnCarZ.ToString(), "",
                leftArrowPressed, rightArrowPressed};

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
        //String path = @"C:\Users\Sharl\Desktop\Files\UON - Research\2019 CompSci Honours\COMP4251\Imagine_street_racing\Data\1";
        Debug.Log(path);

        String separator = ",";
        StringBuilder output = new StringBuilder();

        String[] heading = { "Block", "In-Game Time", "DifficultyLevel", "",
            "CurrentX", "CurrentY", "CurrentZ", "obstacleX", "obstacleY", "obstacleZ", "",
            "Points", "PassingCar?", "",
            "Carcrash?", "CrashCarX", "CrashCarY", "CrashCarZ", "totalCarCrash", "",
            "CarSpawned?", "SpawnCarX", "SpawnCarY", "SpawnCarZ", "",
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
            block = "0"; timer = "0"; difficulty = "0"; currentXPos = "0"; currentYPos = "0"; currentZPos = "0"; points = "0";
            currentlyPassingCar = "0"; currentlyCrashing = "0"; crashedX = "0"; crashedY = "0"; crashedZ = "0"; totalCrashed = "0";
        }
    }
}

