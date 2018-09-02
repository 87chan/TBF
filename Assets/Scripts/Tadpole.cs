using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tadpole : MonoBehaviour {
	Vector3 StartPos;
	Vector3 EndPos;
	bool bAdvance;
	Vector3 Direction;
	float CurrentSpeed;
	static float advanceAccel = 10f;
	static float inverseAccel = 0.25f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
		// https://qiita.com/tempura/items/4a5482ff6247ec8873df
        TouchInfo info = AppUtil.GetTouch();

		Vector3 velocity = new Vector3();

        if (info == TouchInfo.Began)
        {
			StartPos = AppUtil.GetTouchPosition();
        }
		else if (info == TouchInfo.Ended)
		{
			EndPos = AppUtil.GetTouchPosition();
		
			Vector3 direction = (StartPos - EndPos);
			Direction = direction.normalized;

			float deg = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
			deg -= 90;

			transform.rotation = Quaternion.Euler(0,0,deg);
			
			bAdvance = true;
			CurrentSpeed = advanceAccel;
		}

		if(bAdvance)
		{
			CurrentSpeed -= inverseAccel;
			Debug.Log(CurrentSpeed);
			velocity = CurrentSpeed * Direction;

			if(CurrentSpeed >= 0)
			{
			transform.position += velocity;
			}
			else
			{
bAdvance = false;
			}
		}
    }
}
