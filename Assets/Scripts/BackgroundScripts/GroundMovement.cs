using UnityEngine;
using UnityEngine.UIElements;

public class GroundMovement : MonoBehaviour
{
    public GameObject resetPoint;
    
    private float speed;
    private Transform self;
    private GameObject manager;
    private GlobalVars globalVars;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindWithTag("GameController");
        globalVars = manager.GetComponent<GlobalVars>();

        self = GetComponent<Transform>();
        speed = globalVars.groundSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (globalVars.gameEnd)
        {
            speed = 0;
        }
        if(self.position.z < -1000)
        {
            self.position += new Vector3(0,0,2000);
        }
        self.position -= new Vector3(0,0,speed) * Time.deltaTime;
    }
}
