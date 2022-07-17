using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    //private Vector3 baseScale;
    //public float minScale = 1.4f;
    //public float speed = 1f;
    //private float inverseMinScale;

    public float minScale = 0.9f;
    public float maxScale = 1f;
    public float speed = 3f;

	private void Start()
	{
        //baseScale = transform.localScale;
        //inverseMinScale = 1 - minScale; 
	}

	private void Update()
    {
        Vector3 vec = new Vector3(minScale + Mathf.Sin(Time.time * speed) * (maxScale - minScale), minScale + Mathf.Sin(Time.time * speed) * (maxScale - minScale), minScale + Mathf.Sin(Time.time * speed) * (maxScale - minScale));

        transform.localScale = vec;
    }
}
