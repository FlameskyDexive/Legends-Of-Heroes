using UnityEngine;
using System.Collections;

public class AutoRotary : MonoBehaviour {

	public Vector3 vector;

	void Start () {
		
	}
	
	void Update () {
		transform.Rotate(vector*Time.deltaTime, Space.Self);
	}
}
