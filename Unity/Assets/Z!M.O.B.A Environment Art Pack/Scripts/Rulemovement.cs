using UnityEngine;
using System.Collections;

public class Rulemovement : MonoBehaviour {

	public float x_length = 1.0f;	//左右往复移动的范围
	public float x_speed = 1.0f;	//左右移动的速度
	public float y_length = 1.0f;	//上下往复移动的范围
	public float y_speed = 1.0f;	//上下移动的速度

	private Vector3 oldPostion;

	void Start () {
		oldPostion = transform.position; //为的是在任意位置来回移动
	}

	void Update () {
		float x_positon = Mathf.PingPong (Time.time*x_speed, x_length);
		float y_positon = Mathf.PingPong (Time.time*y_speed, y_length);
		transform.position = oldPostion + new Vector3(x_positon, y_positon, 0);
	}

}
