using UnityEngine;

public class TrafficMovement : MonoBehaviour
{
    private int speed;
    private Transform self;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<Transform>();
        speed = GameObject.FindWithTag("GameController").GetComponent<GlobalVars>().groundSpeed - 20;
    }

    // Update is called once per frame
    void Update()
    {
        self.position -= new Vector3(0,0,speed) * Time.deltaTime;
        if(self.position.z < -50)
        {
            Destroy(gameObject);
        }
    }
}
