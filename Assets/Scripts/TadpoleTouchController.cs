using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// おたまじゃくしの抽象化されたコントローラクラス
/// プレイヤーやAIはこのコントローラを用いてオタマジャクシを操作する。
/// </summary>
public class TadpoleTouchController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        Debug.Assert(this.Tadpole,"GUIにて操作対象のおたまじゃくしをセットする必要があります");
        this.boxCollider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        TouchInfo info = AppUtil.GetTouch();

        if (info == TouchInfo.Began)
        {
            StartPos = AppUtil.GetTouchPosition();

            // 自身が持つ矩形内がタップされていたときのみタップ開始とする.
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(StartPos);
            if(this.boxCollider.OverlapPoint(worldPos))
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
    }

    public Tadpole Tadpole; //操作対象のオタマジャクシ.

    Vector3 StartPos;       //タップ開始位置.
    bool bTapTrigger;       //タップされている
    Collider2D boxCollider; //自分が持っている当たり矩形    

}
