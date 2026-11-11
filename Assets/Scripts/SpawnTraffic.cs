using System.Collections.Generic;
using UnityEngine;

public class SpawnTraffic : MonoBehaviour
{
  
    public GameObject trafficCar;

    private List<Transform> spawnPoints = new List<Transform>();
    private float timePassed = 0;
    private float spawnDelay;
    private GameObject manager;
    private GlobalVars globalVars;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController");
        globalVars = manager.GetComponent<GlobalVars>();

        spawnDelay = globalVars.spawnDelay;

        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
            Debug.Log("added " + child + " to spawnPoints");
        } 
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        spawnDelay = globalVars.spawnDelay;
        if (globalVars.boostActive)
        {
            timePassed += Time.deltaTime * (globalVars.boostSpeed/globalVars.groundSpeed);
        }
        if(timePassed >= spawnDelay)
        {
            for(int i = 0; i < 4; i++)
            {
                if(Random.Range(0,4) == i)
                {
                    Instantiate(trafficCar, spawnPoints[i].position, Quaternion.identity);
                    Debug.Log("spawned car");
                }
            }
            timePassed = 0;
        }
    }
}
