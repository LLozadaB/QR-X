using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPlatformSpin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, 25f * Time.deltaTime, Space.Self);
    }
}
