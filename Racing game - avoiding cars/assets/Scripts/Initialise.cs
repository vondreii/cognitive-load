﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Initialise : MonoBehaviour {

    // Input field for participant's number
    public InputField participantNo;

    public Text instructions1;
    public Text instructions2;
    public Text instructions3;
    public Text instructions4;

    public Texture rawImageTexture1;
    public Texture rawImageTexture2;
    public RawImage rawImage;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        // After entering the number in the box, the user presses 'enter'
        if(Input.GetKeyDown(KeyCode.Return))
        {
            // Only accepts it if it is valid
            if (participantNo.text.Trim() != "")
            {
                PlayerPrefs.SetString("Participant", participantNo.text.Trim());
                PlayerPrefs.SetString("Date", DateTime.Now.ToString("yyyy-MM-dd-h-mm-tt"));

                PlayerPrefs.SetInt("briefing", 1);
                SceneManager.LoadScene(4);
            }
        }

        // Because the space from before also triggers the input space in this one too... So it is counting the space AFTER
        if(PlayerPrefs.GetInt("briefing") == 1 || PlayerPrefs.GetInt("briefing") == 2)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rawImage.texture = rawImageTexture1;
                instructions1.text = "Your primary goal is to avoid the cars. You receive points by passing cars.";
                instructions2.text = "You do not need to avoid the roadblocks (as shown below).";
                instructions3.text = "They will be transparent, and do not affect your score. ";
                
                PlayerPrefs.SetInt("briefing", PlayerPrefs.GetInt("briefing")+1);
            }
        }

        if (PlayerPrefs.GetInt("briefing") == 3 || PlayerPrefs.GetInt("briefing") == 4)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rawImage.texture = rawImageTexture2;
                instructions1.text = "You will play the same game 3 times.";
                instructions2.text = "After each game, you will have a break before continuing to the next one.";
                instructions3.text = "Good Luck!";
                instructions4.text = "Please press the [spacebar] key to start the first game";

                PlayerPrefs.SetInt("briefing", PlayerPrefs.GetInt("briefing") + 1);
            }
        }

        if (PlayerPrefs.GetInt("briefing") == 5)
        {
            PlayerPrefs.SetInt("briefing", 0);
            SceneManager.LoadScene(0);
        }

    }
}
