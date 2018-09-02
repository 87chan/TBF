using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

	public Food Food;
	public int InitialFoodNum;

	// Use this for initialization
	void Start () {
		if(Food)
		{
			Camera myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
			float height = myCamera.orthographicSize * 2.0f;
			float width = (height / 16) * 9;
			for(int i = 0; i < InitialFoodNum;++i)
			{
				Vector3 Pos = new Vector3(Random.Range(0, width),Random.Range(0,height),0);
				Pos -= new Vector3(width * 0.5f,height * 0.5f,0);
				Instantiate(Food,Pos,Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
