using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// おたまじゃくしの抽象化されたコントローラクラス
/// プレイヤーやAIはこのコントローラを用いてオタマジャクシを操作する。
/// </summary>
public class TadpoleTouchController : MonoBehaviour
{
    public void ChangePlayerTouchInfo(Vector2 position, Vector2 size)
    {
        this.transform.position = new Vector3(position.x, position.y, transform.position.z);
        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = size;
    }

	// Use this for initialization
	void Start ()
    {
        Tadpole = this.GetComponentInChildren<Tadpole>();
        Debug.Assert(this.Tadpole,"おたまじゃくしが見つかりません");
        this.boxCollider = GetComponent<Collider2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (!GameMain.bGameStart)
        {
            return;
        }
        //#todo 正気を疑うレベルで汚いが緊急時なので、無理やりマルチタップとエディタ上での処理をifdefで分けている
#if UNITY_EDITOR
        TouchInfo info = AppUtil.GetTouch();

        if (info == TouchInfo.Began)
        {
            StartPos = AppUtil.GetTouchPosition();

            // 自身が持つ矩形内がタップされていたときのみタップ開始とする.
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(StartPos);
            if (this.boxCollider.OverlapPoint(worldPos))
            {
                bTapTrigger = true;
            }
        }
        else if (info == TouchInfo.Ended && bTapTrigger)
        {
            bTapTrigger = false;

            Vector3 endPos = AppUtil.GetTouchPosition();

            // 指を動かした場合にのみ、オタマジャクシを移動させる。
            if (StartPos != endPos)
            {
                Vector3 direction = (StartPos - endPos).normalized;
                // 移動方向の確定.おたまじゃくしに伝える.
                this.Tadpole.MoveDirection(direction);
            }
        }
#else
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);

            Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, 0.0f);
        
            if ((int)touch.phase == (int)TouchInfo.Began)
            {
                // 自身が持つ矩形内がタップされていたときのみタップ開始とする.
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchPos);
                if (this.boxCollider.OverlapPoint(worldPos))
                {
                    bTapTrigger   = true;           //タップトリガ.
                    this.fingerId = touch.fingerId; //指IDを記録.
                    StartPos      = touchPos;       //タップスタート位置を記憶.
                }
            }
            else if ((int)touch.phase == (int)TouchInfo.Ended && bTapTrigger)
            {
                //指IDが一致した場合のみ移動処理.
                if(this.fingerId == touch.fingerId)
                {
                    bTapTrigger = false;
                    // 指を動かした場合にのみ、オタマジャクシを移動させる。
                    if (StartPos != touchPos)
                    {
                        Vector3 direction = (StartPos - touchPos).normalized;
                        // 移動方向の確定.おたまじゃくしに伝える.
                        this.Tadpole.MoveDirection(direction);
                    }
                }
            }
        }

#endif
    }

    Tadpole Tadpole; //操作対象のオタマジャクシ.

    Vector3 StartPos;       //タップ開始位置.
    bool bTapTrigger;       //タップされている
    Collider2D boxCollider; //自分が持っている当たり矩形    

    int fingerId;   // タップした指ID.

}
