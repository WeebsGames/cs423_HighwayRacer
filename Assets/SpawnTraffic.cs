using System.Collections.Generic;
using UnityEngine;

public class SpawnTraffic : MonoBehaviour
{
    public int spawnDelay;
    public GameObject trafficCar;
    private List<GameObject> spawnPoints = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time % spawnDelay == 0)
        {
            
        }
    }
}
