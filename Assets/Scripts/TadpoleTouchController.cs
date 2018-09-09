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

    Tadpole Tadpole; //操作対象のオタマジャクシ.

    Vector3 StartPos;       //タップ開始位置.
    bool bTapTrigger;       //タップされている
    Collider2D boxCollider; //自分が持っている当たり矩形    

}
