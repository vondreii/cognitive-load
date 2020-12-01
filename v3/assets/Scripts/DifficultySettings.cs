using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings : MonoBehaviour {

    TerrainManager terrainManager;
	// Use this for initialization
	void Start () {
        terrainManager = GameObject.FindGameObjectWithTag("terrainManager").GetComponent<TerrainManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setDifficultySettings(int difficultyLvl)
    {
		//Debug.Log("Difficulty: "+difficultyLvl);
        // Uses the set(startDistance, numCars, distance) method. Uses the 3 numbers to determine how many cars will appear,
        // and the distance between the cars (to fit more cars).

        // ----- LEVEL 10 ----- //
        if (difficultyLvl == 10) set(200, 28, 30);

        // ----- LEVEL 9 ----- //
        else if (difficultyLvl == 9) set(200, 25, 50);

        // ----- LEVEL 8 ----- //
        else if (difficultyLvl == 8) set(200, 22, 100);

        // ----- LEVEL 7 ----- //
        else if (difficultyLvl == 7) set(200, 19, 200);

        // ----- LEVEL 6 ----- //
        else if (difficultyLvl == 6) set(250, 16, 250);

        // ----- LEVEL 5 ----- //
        else if (difficultyLvl == 5) set(300, 13, 300);

        // ----- LEVEL 4 ----- //
        else if (difficultyLvl == 4) set(350, 10, 350);

        // ----- LEVEL 3 ----- //
        else if (difficultyLvl == 3) set(400, 7, 400);

        // ----- LEVEL 2 ----- //
        else if (difficultyLvl == 2) set(450, 4, 450);

        // ----- LEVEL 1 ----- //
        else if (difficultyLvl == 1) set(500, 1, 500);

    }

    void set(int start, int numCars, int distance)
    {
        terrainManager.setStartDistance(start);
        terrainManager.setNumCars(numCars);
        terrainManager.setDistance(distance);
    }
}
