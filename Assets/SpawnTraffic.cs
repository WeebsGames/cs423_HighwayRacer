using System.Collections.Generic;
using UnityEngine;

public class SpawnTraffic : MonoBehaviour
{
    public int spawnDelay;
    public GameObject trafficCar;
    private List<Transform> spawnPoints = new List<Transform>();
    private float timePassed = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if(timePassed >= spawnDelay)
        {
            Debug.Log("spawned car");
            Instantiate(trafficCar, spawnPoints[Random.Range(0, 4)].position, Quaternion.identity);
            timePassed = 0;
        }
    }
}
