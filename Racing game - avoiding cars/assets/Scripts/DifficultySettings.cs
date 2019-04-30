using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings : MonoBehaviour
{
    TerrainManager terrainManager;
    // Use this for initialization
    void Start()
    {
        terrainManager = GameObject.FindGameObjectWithTag("terrainManager").GetComponent<TerrainManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void setDifficultySettings(float currentTime)
    {
        // Uses the set(startDistance, numCars, distance) method. Uses the 3 numbers to determine how many cars will appear,
        // and the distance between the cars (to fit more cars).

        // ----- LEVEL 10 ----- //
        if (currentTime >= 180) set(200, 28, 30, 10);

        // ----- LEVEL 9 ----- //
        else if (currentTime >= 160) set(200, 25, 50, 9);

        // ----- LEVEL 8 ----- //
        else if (currentTime >= 140) set(200, 22, 100, 8);

        // ----- LEVEL 7 ----- //
        else if (currentTime >= 120) set(200, 19, 200, 7);

        // ----- LEVEL 6 ----- //
        else if (currentTime >= 100) set(250, 16, 250, 6);

        // ----- LEVEL 5 ----- //
        else if (currentTime >= 80) set(300, 13, 300, 5);

        // ----- LEVEL 4 ----- //
        else if (currentTime >= 60) set(350, 10, 350, 4);

        // ----- LEVEL 3 ----- //
        else if (currentTime >= 40) set(400, 7, 400, 3);

        // ----- LEVEL 2 ----- //
        else if (currentTime >= 20) set(450, 4, 450, 2);

        // ----- LEVEL 1 ----- //
        else if (currentTime >= 0) set(500, 1, 500, 1);

    }

    void set(int start, int numCars, int distance, int newDifficulty)
    {
        terrainManager.setStartDistance(start);
        terrainManager.setNumCars(numCars);
        terrainManager.setDistance(distance);
        terrainManager.setDifficultyLevel(newDifficulty);

        Debug.Log("New Difficulty is: " + newDifficulty);
    }
}
