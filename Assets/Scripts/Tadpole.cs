﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tadpole : MonoBehaviour
{
    public bool bPlayer01 = true;

    // 成長段階の最大値
    static public int MAX_LEVEL = 4;

    // 前進時の加速度
    const float ADVANCE_ACCEL = 400f;

    // 進行方向とは逆に働く減速度
    const float INVERSE_ACCEL = 10f;

    // 速度の最大値
    const float MAX_SPEED = 400f;

    // 弾かれた際の加速度
    const float POP_ACCEL_ACTIVE = 200f;
    const float POP_ACCEL_PASSIVE = 400f;

    const float KOCKBACK_TIME = 0.1f;

	public string PlayerName;
	public int Level = 1;
	bool bKockBack;
	Vector3 Direction;
	float CurrentSpeed;
	float CurrentKockBackTime;

	public bool CheckKockBack() { return bKockBack; }

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

	public void KockBack()
	{
		bKockBack = true;
	}

	public void LevelUp(int upNum = 1)
	{
		SetLevel(Mathf.Clamp((Level + upNum), 0, MAX_LEVEL));
	}

	public void LevelDown(int downNum = 1)
	{
		SetLevel(Mathf.Clamp((Level - downNum), 0, MAX_LEVEL));
	}

	public void AddAccel(float value, bool bReset = false)
	{
		if(bReset)
		{
			CurrentSpeed = 0.0f;
		}
		CurrentSpeed += value;
		CurrentSpeed = Mathf.Clamp(CurrentSpeed,0,MAX_SPEED);
	}

	public void SetDirection(Vector3 direction)
	{
		Direction = direction;
	}

    /// <summary>
    /// 指定した方向におたまじゃくしが移動する.
    /// </summary>
    /// <param name="direction">正規化された移動方向,画面中央を0とした2d座標系.zは使わない</param>
    /// <param name="accel">加速量</param>
    public void MoveDirection(Vector3 direction,float accel = ADVANCE_ACCEL)
    {
        if (Time.timeScale > 0)
        {
            //方向、加速度、回転の設定.
            SetDirection(direction);
            AddAccel(accel);

            float deg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            deg -= 90;
            transform.rotation = Quaternion.Euler(0, 0, deg);
        }
    }

	// Use this for initialization
	void Start () 
	{
		
	}

    // Update is called once per frame
    void Update()
    {
		this.UpdateTransform();

		this.UpdateKockBack();
    }

	void UpdateTransform()
	{
        AddAccel(-INVERSE_ACCEL);
        Vector3 velocity = CurrentSpeed * Direction * Time.deltaTime;
        transform.position += velocity;

		AdjustPosition();
	}

	// 移動後の位置調整
	void AdjustPosition()
	{
		Vector3 newPosition = transform.position;
        Rect rect = new Rect(-AppUtil.GetOrthographicSize() * 0.5f, AppUtil.GetOrthographicSize());
		float radius = this.GetComponent<CircleCollider2D>().radius * 2.0f;
		
		// 横軸の画面端押し出し
		if((newPosition.x - radius) < rect.xMin)
		{
			newPosition.x = rect.xMin + radius;
		}
		else if(rect.xMax < newPosition.x + radius)
		{
			newPosition.x = rect.xMax - radius;
		}

		// 縦軸の画面端押し出し
		if(newPosition.y - radius < rect.yMin)
		{
			newPosition.y = rect.yMin + radius;
		}
		else if(rect.yMax < newPosition.y + radius)
		{
			newPosition.y = rect.yMax - radius;
		}

		transform.position = newPosition;
	}

	void UpdateKockBack()
	{
		if(this.CheckKockBack())
		{
			CurrentKockBackTime += Time.deltaTime;
			if(CurrentKockBackTime >= KOCKBACK_TIME)
			{
				bKockBack = false;
				CurrentKockBackTime = 0.0f;
			}
		}
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject && other)
        {
            Tadpole otherTadPole = other.gameObject.GetComponent<Tadpole>();
            if (otherTadPole)
            {
				// ノックバック中は処理しない
                if (!otherTadPole.CheckKockBack() && !this.CheckKockBack())
                {
					bool bSpawnFood = false;
					Vector3 spawnPosition = new Vector3();
					Tadpole activeTadpole = null;

                    // よりレベルの高い方を下げる
                    // 同レベルの場合は下がらない
                    if (otherTadPole.Level == this.Level)
                    {
                    }
                    else if (otherTadPole.Level < this.Level)
                    {
                        this.LevelDown();
						bSpawnFood = true;
						spawnPosition = this.transform.position;
						activeTadpole = otherTadPole;
                    }
                    else
                    {
                        otherTadPole.LevelDown();
                        bSpawnFood = true;
                        spawnPosition = otherTadPole.transform.position;
						activeTadpole = this;
                    }

					float popAccel = 0.0f;
                    Vector3 direction = otherTadPole.transform.position - this.transform.position;
                    direction.Normalize();

                    // ヒットしてきた対象
                    otherTadPole.SetDirection(direction);
                    otherTadPole.KockBack();

					popAccel = (activeTadpole == null)? POP_ACCEL_ACTIVE :
					((activeTadpole == otherTadPole)? POP_ACCEL_ACTIVE: POP_ACCEL_PASSIVE);
					otherTadPole.AddAccel(popAccel, true);

                    // 自分自身
                    this.SetDirection(-direction);
                    this.KockBack();

					popAccel = (activeTadpole == null)? POP_ACCEL_ACTIVE :
					((activeTadpole == this)? POP_ACCEL_ACTIVE: POP_ACCEL_PASSIVE);
					this.AddAccel(popAccel, true);

					// エサの配置
					if(bSpawnFood)
                    {
                        GameObject.Find("GameMain").GetComponent<GameMain>().CreateFood(spawnPosition,false);
                    }
                }
            }
        }
    }
}
