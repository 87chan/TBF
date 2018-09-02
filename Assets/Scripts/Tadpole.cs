using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tadpole : MonoBehaviour {

public bool bPlayer01 = true;

	// 成長段階の最大値
	static int MAX_LEVEL = 4;

	// 前進時の加速度
	static float ADVANCE_ACCEL = 10f;

	// 進行方向とは逆に働く減速度
	static float INVERSE_ACCEL = 0.25f;

	// 速度の最大値
	static float MAX_SPEED = 10f;

	// 弾かれた際の加速度
	static float POP_ACCEL = 10f;

	int Level = 1;
	bool bSelfMove;
	Vector3 StartPos;
	Vector3 EndPos;
	Vector3 Direction;
	float CurrentSpeed;

	private void UpdateFoodNumText()
	{
		Text text = this.gameObject.GetComponent<Text>();
		text.text = (Level - 1).ToString();
	}

	public void SetLevel(int level)
	{
		Level = level;
		this.UpdateFoodNumText();
	}

	public void LevelUp(int upNum = 1)
	{
		SetLevel(Mathf.Clamp((Level + upNum), 0, MAX_LEVEL));
	}

	public void LevelDown(int downNum = 1)
	{
		SetLevel(Mathf.Clamp((Level - downNum), 0, MAX_LEVEL));
	}

	public void AddAccel(float value)
	{
		CurrentSpeed += value;
		CurrentSpeed = Mathf.Clamp(CurrentSpeed,0,MAX_SPEED);
	}

	public void SetDirection(Vector3 direction)
	{
		Direction = direction;
	}

	// Use this for initialization
	void Start () 
	{
		
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

			AddAccel(ADVANCE_ACCEL);
			
			// 回転を変更
			float deg = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
			deg -= 90;
			transform.rotation = Quaternion.Euler(0,0,deg);
		}

        AddAccel(-INVERSE_ACCEL);
        velocity = CurrentSpeed * Direction;
        transform.position += velocity;
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
			otherTadPole.AddAccel(POP_ACCEL);

			this.SetDirection(-direction);
			this.AddAccel(POP_ACCEL);
		}
		}
	}
}
