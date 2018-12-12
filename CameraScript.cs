using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

   
    public Rigidbody2D rb;
    public Camera map;
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        map = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (Input.GetKey("w"))
        {
            rb.transform.position += Vector3.up;
        }
        if (Input.GetKey("s"))
        {
            rb.transform.position += Vector3.down;
        }
        if (Input.GetKey("a"))
        {
            rb.transform.position += Vector3.left;
        }
        if (Input.GetKey("d"))
        {
            rb.transform.position += Vector3.right;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            map.orthographicSize++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            map.orthographicSize--;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
        }

    }
}
