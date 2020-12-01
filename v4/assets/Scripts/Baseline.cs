using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;

public class Baseline : MonoBehaviour {

    public FileManagement fileManagement;

    List<float> allPlayerResponseTimes;
    List<float> baselineResponseTimes;
    List<float> movingAverageArray;

    float mean, sd, criticalPoint;
    int numberOfReactionsAfter1Min;

    Boolean createBaselineNow;
    String filePath;

    // Use this for initialization
    void Start () {
        fileManagement = GameObject.FindGameObjectWithTag("fileManager").GetComponent<FileManagement>();

        allPlayerResponseTimes = new List<float>();
        baselineResponseTimes = new List<float>();
        movingAverageArray = new List<float>();
        mean = 0;
        createBaselineNow = true;
        numberOfReactionsAfter1Min = 0;

        filePath = fileManagement.getFilePath();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float getCritPoint() { return criticalPoint;  }

    // ------------------------------------------------------------------------------------------------------ //
    // -------------------- A D D S  R E S P O N S E  T I M E S  T O  A R R A Y S --------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    public void addResponseTime(float responseTime, float timer)
    {
        // Add all the response times of the user into an array
        allPlayerResponseTimes.Add(responseTime);

        // The baseline array is a separate array that stores the first 8/9 etc responses (everything before 60 minutes)
        if (timer <= 60)
        {
            fileManagement.saveCarSpawnedDetails("baseline", responseTime.ToString(), "0", "0", "0");
            
			if(responseTime > 0) 
			{ 
				baselineResponseTimes.Add(responseTime); 
				// Debug.Log(responseTime);
			}
			
            fileManagement.saveCarSpawnedDetails("baseline", "0", "0", "0", "0");
        }

        // After playing for 60 minutes, their responses are added to an array where their moving average will be calculated
        else
        { movingAverageArray.Add(responseTime); }
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------------ C R E A T E  B A S E L I N E ------------------------------------------ //
    // ------------------------------------------------------------------------------------------------------ //
    public void createBaseline()
    {
        float calc = 0; // Used during the sd calculation.
        String responseStr = "";
        // Calculates the mean of all responses in the baseline
        for (int i = 0; i < baselineResponseTimes.Count; i++)
        {
            mean += baselineResponseTimes[i];
            responseStr += (i + 1) + ": " + baselineResponseTimes[i] + " | ";
			// Debug.Log(responseStr);
        }
        mean = mean / baselineResponseTimes.Count;

        
        //calculate standard deviation
        for (int i = 0; i < baselineResponseTimes.Count; i++)
        {
            // Part of formula: (the number - mean)^2 - for each number.
            float sum = Mathf.Pow((baselineResponseTimes[i] - mean), 2);
            
            // Adds them together
            calc += sum;
        }
        // Does the calculation for the square root of the final sum/(number of responses-1) (for the sample standard deviation).
        sd = Mathf.Sqrt(calc / (baselineResponseTimes.Count - 1));

        // Calculates the 2nd percentile ( 2 sd away from the mean - the "critical point")
        criticalPoint = mean + (sd * 2f);

        fileManagement.saveCarSpawnedDetails("baseline", "0", mean.ToString(), sd.ToString(), criticalPoint.ToString());

        //File.AppendAllText(filePath, " --> BASELINE CREATED -->" + responseStr + " ");
        //File.AppendAllText(filePath, " --> Final SD: " + sd + ", Critical Point: " + criticalPoint);
    }

    // ------------------------------------------------------------------------------------------------------ //
    // -------------------------- G E T  M O V I N G  A V E R A G E S --------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    public float getMovingAverage(float responseTime, float timer)
    {
        double WMA = 0;
        // Calculates the WMA of the first 3 responses
        if (timer > 60)
        {
            getAllResponseTimes("movingAverage");

            // First, calculate the first 3 moving averages (get the 3 inputs after the first minute).
            if (movingAverageArray.Count == 3)
            {
                 WMA = movingAverages(movingAverageArray[0], movingAverageArray[1], movingAverageArray[2]);
            }

            // Then, for every added response, only do the average for the last 3 spots in the array
            if (movingAverageArray.Count > 3)
            {
                WMA = movingAverages(movingAverageArray[movingAverageArray.Count - 3], movingAverageArray[movingAverageArray.Count - 2], 
                    movingAverageArray[movingAverageArray.Count - 1]);  
            }

            fileManagement.saveCarSpawnedDetails("movingAverage", "0", "0", "0", "0");
            fileManagement.saveCarSpawnedDetails("weightedMovingAverage", "0", "0", "0", "0");
        }
        return (float)WMA;
    }

    public float movingAverages(float m1, float m2, float m3)
    {
        double WMA = 0;
        float weightedM1 = m1*(float)0.5;
        float weightedM3 = m3*(float)2;

        // Will take the last 3 numbers in the array (so.. movingAverageArray.Count returns the length of the array so it 
        // uses that to get the last 3 numbers...)
        WMA = (weightedM1 + m2 + weightedM3);
        WMA = WMA / 3;

        // Save to file the reaction times that will be used before calculating the moving average.
        fileManagement.saveCarSpawnedDetails("movingAverage", m1.ToString(), m2.ToString(), m3.ToString(), "0");

        // Save to file the weighted reaction times and the final WMA.
        fileManagement.saveCarSpawnedDetails("weightedMovingAverage", weightedM1.ToString(), m2.ToString(), weightedM3.ToString(), WMA.ToString());

        return (float)WMA;
    }

    // ------------------------------------------------------------------------------------------------------ //
    // ------------------------------- P R I N T I N G  A R R A Y S ----------------------------------------- //
    // ------------------------------------------------------------------------------------------------------ //
    public void getAllResponseTimes(String type)
    {
        //Debug.Log(allPlayerResponseTimes.Count);
        //Debug.Log("first response: "+allPlayerResponseTimes[0]);

        List<float> array;
        if (type.Equals("movingAverage")) array = movingAverageArray;
        else if (type.Equals("basline")) array = baselineResponseTimes;
        else array = allPlayerResponseTimes;

        String responseStr = "";
        for (int i = 0; i < array.Count; i++)
        {
            responseStr += (i + 1) + ": " + array[i] + " | ";
        }
        //Debug.Log(responseStr);
    }
}
