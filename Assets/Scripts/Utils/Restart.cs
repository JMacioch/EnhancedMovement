using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    public Transform spawnPos;
    public Vector3 lol;
    void Start()
    {
        lol = transform.position;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            transform.position = lol;
            Debug.Log(spawnPos);
        }
    }
}
