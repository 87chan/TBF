using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tadpole : MonoBehaviour {

public bool bPlayer01 = true;

	static int MaxLevel = 3;
	static float AdvanceAccel = 10f;
	static float InverseAccel = 0.25f;
	static float SpeedMax = 10f;
	static float PopAccel = 10f;

	int Level = 0;
	bool bSelfMove = false;
	Vector3 StartPos;
	Vector3 EndPos;
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

			AddAccel(AdvanceAccel);
			
			// 回転を変更
			float deg = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
			deg -= 90;
			transform.rotation = Quaternion.Euler(0,0,deg);
		}

        AddAccel(-InverseAccel);
        velocity = CurrentSpeed * Direction;
        transform.position += velocity;
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

	public void AddAccel(float value)
	{
		CurrentSpeed += value;
		CurrentSpeed = Mathf.Clamp(CurrentSpeed,0,SpeedMax);
	}

	public void SetDirection(Vector3 direction)
	{
		Direction = direction;
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

			Vector3 direction = otherTadPole.transform.position - this.transform.position;
			direction.Normalize();

			otherTadPole.SetDirection(direction);
			otherTadPole.AddAccel(PopAccel);

			this.SetDirection(-direction);
			this.AddAccel(PopAccel);
		}
		}
	}
}
