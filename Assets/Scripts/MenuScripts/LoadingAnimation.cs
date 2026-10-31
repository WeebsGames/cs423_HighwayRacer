using TMPro;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{

    public int speed = 1;
    public int dots = 3;
    
    private TMP_Text self;
    private string oText;
    private float timePassed;
    private int dotCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        self = GetComponent<TMP_Text>();
        oText = self.text;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= speed)
        {
            timePassed = 0;
            dotCount++;
            if(dotCount > dots)
            {
                self.text = oText;
                dotCount = 0;
            } else
            {
                self.text += ".";
            }
        }
    }
}
