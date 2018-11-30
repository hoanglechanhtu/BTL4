using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float delta=5;
    public Transform togo;
    private Vector3 direction;
	// Use this for initialization
	void Start () {
        direction = togo.position - transform.position;
        direction.Normalize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void goUp() {
        Vector3 position = transform.position;
        position -= direction*delta;
        transform.position = position;
    }

    public void goDown() {
        Vector3 position = transform.position;
        position += direction * delta;
        transform.position = position;
    }


}
