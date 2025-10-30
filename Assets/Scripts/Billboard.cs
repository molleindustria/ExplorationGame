using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    public bool onlyZ = true;

    void Start()
    {
       
    }



    void LateUpdate()
    {
        if (onlyZ)
        {
            Vector3 v = Camera.main.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(Camera.main.transform.position - v);
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
