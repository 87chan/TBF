﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(gameObject && other)
		{
		Tadpole tadPole = other.gameObject.GetComponent<Tadpole>();
		if(tadPole)
		{
			tadPole.LevelUp();
			Destroy(gameObject);
		}
		}
	}
}