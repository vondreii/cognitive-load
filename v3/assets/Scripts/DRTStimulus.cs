using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DRTStimulus : MonoBehaviour {

    // terrainManager object to control difficulty
    public TerrainManager terrainManager;
    public FileManagement fileManagement;

    // For the car horn sounds
    [SerializeField]
    AudioClip carSound = null;
    AudioSource carSource = null;

    String filePath;
    float interval, timer, threeSecondWait;
    float responseTimer, startWaitingTimer;
    int intervalSetToPlay;
    Boolean startTimer, trigger, justPlayed = false, waitForResponse, outputToFile;

    // Use this for initialization
    void Start () {
        fileManagement = GameObject.FindGameObjectWithTag("fileManager").GetComponent<FileManagement>();

        carSource = AudioHelper.CreateAudioSource(gameObject, carSound);
        outputToFile = false;
        interval = 0;
        threeSecondWait = 0;
        startTimer = false;
        responseTimer = 0;
        waitForResponse = false;
        startWaitingTimer = 0;
        trigger = false;
        intervalSetToPlay = 0;
        filePath = fileManagement.getFilePath();
    }

    public void setTimer(Boolean state)
    {
        startTimer = state;
        waitForResponse = false;
    }

    // Update is called once per frame
    void Update () {

        // Has it's own timer for how long the game is being played
        int currentTime = 0;

        

        // waitsForResponse is activated when the sound of the horn is played.
        if (waitForResponse)
        {
            if (outputToFile)
            {
                outputToFile = false;
            }
            // A second timer starts when the car horn sounds... this records the player's reaction time
            responseTimer += Time.deltaTime;

            if(responseTimer > 2) fileManagement.saveCarSpawnedDetails("DRTResponse", "0", "1", "0", "0");

            // If the player presses space, the timer stops and records their time
            if (Input.GetKeyDown(KeyCode.Space))
            {
				waitForResponse = false;
				//Debug.Log("Responded: " + responseTimer.ToString());

				// No longer waiting for response or beeping. The player pressed space.
                fileManagement.saveCarSpawnedDetails("DRTResponse", "0", "0", "1", responseTimer.ToString());

                carSource.Stop();

                // method from TerrainManager.cs to change the difficulty
                terrainManager.manageDifficulty(true, responseTimer);

                // No longer waits for a response.
                responseTimer = 0;
                fileManagement.saveCarSpawnedDetails("DRTResponse", "0", "0", "0", "0");
                
            }
        }
		
		if (startTimer)
		{
			// Timer that keeps track of how long the player is playing the game (seconds)
			timer += Time.deltaTime;
			currentTime = System.Convert.ToInt32(timer); // Only counts the seconds

			// Method that makes car sound (DRT)
			// Waits some seconds after the game starts to start playing the DRT (+ 3 seconds from the 3 second countdown)
			if(currentTime >= 3 && currentTime < 285)  DRT(currentTime);
		}
		if (Input.GetKeyDown(KeyCode.Space))
        {
            fileManagement.saveCarSpawnedDetails("NoDRTResponse", "1", "0", "0", "0");

                
            fileManagement.saveCarSpawnedDetails("NoDRTResponse", "0", "0", "0", "0");
		}
		
		
		
    }

    void DRT(int current)
    {
        // A random interval of seconds between 3-5
        int interval = UnityEngine.Random.Range(3, 6);
        
        // If the car sound is not currently playing
        if (!carSource.isPlaying)
        {
            // The car sound will play at random intervals (based on what is generated from above)
            carSource.PlayDelayed(interval);

            // The intervals are generated every frame. The interval chosen for this particular car horn sound is saved.
            intervalSetToPlay = interval;

            // Starts trigger to start counting when to wait for the response of the player.
            trigger = true;
        }
        if(trigger)
        { 
			
            // Starts timing when to wait for the response (because the PlayDelayed() stuffs up the timings).
            startWaitingTimer += Time.deltaTime;
            int startWaitingSeconds = System.Convert.ToInt32(startWaitingTimer);

            // When the play delayed ends, and the sound finally plays
            if (startWaitingSeconds >= (intervalSetToPlay))
            {          
				waitForResponse = true;
				
                if (responseTimer > 0)
                {
                    fileManagement.saveCarSpawnedDetails("NoDRTResponse", "1", responseTimer.ToString(), "0", "0");

                    // method from TerrainManager.cs to change the difficulty
                    terrainManager.manageDifficulty(false, responseTimer);
                    
                    responseTimer = 0;
                }

                fileManagement.saveCarSpawnedDetails("NoDRTResponse", "0", "0", "0", "0");
                fileManagement.saveCarSpawnedDetails("DRTResponse", "1", "1", "0", "0");

                // Then wait for the response
                
				//Debug.Log("Waiting for response (inside trigger): " + waitForResponse);
				startWaitingTimer = 0;
				trigger = false;
				outputToFile = true;
                
            }
			
        }

    }

}
