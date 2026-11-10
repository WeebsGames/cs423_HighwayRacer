using System.Numerics;
using UnityEngine;

public class TrafficMovement : MonoBehaviour
{
    private int speed;
    private Transform self;
    private GameObject manager;
    private GameObject playerPos;
    private GlobalVars globalVars;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindWithTag("GameController");
        globalVars = manager.GetComponent<GlobalVars>();

        playerPos = GameObject.FindWithTag("Player");

        self = GetComponent<Transform>();
        speed = globalVars.groundSpeed - 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (globalVars.gameEnd)
        {
            speed = 0;
        }
        self.position -= new UnityEngine.Vector3(0,0,speed) * Time.deltaTime;
        if(self.position.z < -50)
        {
            Destroy(gameObject);
        }
        if (globalVars.sirenActive)
        {
            if(self.position.x > playerPos.GetComponent<Transform>().position.x)
            {
                self.position += new UnityEngine.Vector3(1,0,0) * Time.deltaTime;
            } else
            {
                self.position -= new UnityEngine.Vector3(1,0,0) * Time.deltaTime;
            }
        }
        
    }
}
