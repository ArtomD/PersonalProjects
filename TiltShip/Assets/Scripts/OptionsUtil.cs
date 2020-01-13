using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUtil : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Calibrate()
    {
        Vector3 calib = Input.acceleration;
        PlayerPrefs.SetFloat("calibX", calib.x);
        PlayerPrefs.SetFloat("calibY", calib.y);
        PlayerPrefs.SetFloat("calibZ", calib.z);
        PlayerPrefs.Save();
    }
}
