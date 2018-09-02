using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tadpole : MonoBehaviour {

public bool bPlayer01 = true;

	static int MaxLevel = 3;
	static float AdvanceAccel = 10f;
	static float InverseAccel = 0.25f;

	int Level = 0;
	bool bSelfMove = false;
	Vector3 StartPos;
	Vector3 EndPos;
	bool bAdvance;
	Vector3 Direction;
	float CurrentSpeed;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        TouchInfo info = AppUtil.GetTouch();

		Vector3 velocity = new Vector3();

        if (info == TouchInfo.Began)
		{
			StartPos = AppUtil.GetTouchPosition();

			float height = Screen.height * 0.5f;
			if((StartPos.y >= height && bPlayer01)
			|| (StartPos.y < height && !bPlayer01))
			{
				bSelfMove = true;
			}
        }
		else if (info == TouchInfo.Ended && bSelfMove)
		{
			bSelfMove = false;

			EndPos = AppUtil.GetTouchPosition();
		
			Vector3 direction = (StartPos - EndPos);
			Direction = direction.normalized;

			float deg = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
			deg -= 90;

			transform.rotation = Quaternion.Euler(0,0,deg);
			
			bAdvance = true;
			CurrentSpeed = AdvanceAccel;
		}

		if(bAdvance)
		{
			CurrentSpeed -= InverseAccel;

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

	public void LevelUp(int upNum = 1)
	{
		Level += upNum;
		Level = Mathf.Clamp(Level,0,MaxLevel);
	}

	public void LevelDown(int downNum = 1)
	{
		Level -= downNum;
		Level = Mathf.Clamp(Level,0,MaxLevel);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(gameObject && other)
		{
			Tadpole otherTadPole = other.gameObject.GetComponent<Tadpole>();
		if(otherTadPole)
		{
			// よりレベルの高い方を下げる
			// 同レベルの場合は下がらない
			if(otherTadPole.Level == this.Level)
			{
			}
			else if(otherTadPole.Level < Level)
			{
				this.LevelDown();
			}
			else
			{
				otherTadPole.LevelDown();
			}
		}
		}
	}
}
