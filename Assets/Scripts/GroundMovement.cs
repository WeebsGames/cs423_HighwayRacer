using UnityEngine;
using UnityEngine.UIElements;

public class GroundMovement : MonoBehaviour
{
    public GameObject resetPoint;
    public float speed;

    private Transform self;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(self.position.z < -1000)
        {
            self.position += new Vector3(0,0,2000);
        }
        self.position -= new Vector3(0,0,speed) * Time.deltaTime;
    }
}
