using UnityEngine;

public class cameraPosLink : MonoBehaviour
{
    public Transform car;
    private Transform self;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        self.position = new Vector3(car.position.x, self.position.y, self.position.z);
    }
}
