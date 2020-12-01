using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class BlockManager : MonoBehaviour
{

    Boolean exit = false;
    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (exit)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Application.Quit();
            }
        }

        // For every update frame, it will check if we are suppose to start a new block (This is at the point where the black screen shows)
        if (PlayerPrefs.GetInt("startingNewBlock") == 1)
        {
            // The user can press space to start the next block
            if (Input.GetKeyDown(KeyCode.Space))
            {

                // Loads the scene for the next game 
                SceneManager.LoadScene(0);
            }
        }
    }

    public void startNewBlock()
    {
        if (PlayerPrefs.GetInt("numberOfBlocks") >= 3)
        {
            // Deletes the number of blocks (so, when restarting the game completely, it is 0).
            exit = true;
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(2);
        }
        else
        {
            // The code in TerrainManager will call this method 
            // If we are starting a new block, the scene (with the black screen) will load
            SceneManager.LoadScene(1);

            // This startingNewBlock value is checked on the next update frame
            PlayerPrefs.SetInt("startingNewBlock", 1);
        }
    }
}
