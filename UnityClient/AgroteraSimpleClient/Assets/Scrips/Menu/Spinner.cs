using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
	public Vector3 Axis = new Vector3(0, 1, 0);
	public float Spin = 0;


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(Axis, Spin * Time.deltaTime,Space.Self);
	}
}
