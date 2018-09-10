using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // 死亡時のコールバック.
    public delegate void OnDead(Food food);
    public OnDead OnDeadListeners;

    private readonly float DROP_SECOND = 3.5f;  // 何秒で池に落ちるか.
    private float dropSecond;   // 池に落ちるまでの残り秒数.
    private bool isDroped;      // 餌が池の中に落ちたかどうか.(生成されてしばらくは影状態になっている)
    private bool isDead;        // 完全に餌がもう死んだかどうか.

    private Color preDropColor = new Color(0.0f,0.0f,0.0f,0.5f);    //影の状態の色
    private Color dropedColor = Color.white;                        //落ちた状態の色

    private SpriteRenderer foodSprite;      //コンポーネント
    private bool isInitPreDrop; // 初期値.

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="isPreDrop">trueなら池に落ちる前の状態から始まる(影の演出アリ)</param>
    public void Initialize(bool isPreDrop)
    {
        isInitPreDrop = isPreDrop;
    }
    
    // Use this for initialization
    void Start ()
    {
        foodSprite = GetComponent<SpriteRenderer>();
        isDead = false;

        Debug.Assert(foodSprite, "コンポーネント未設定");

        if (this.isInitPreDrop)
        {
            //まだ池に落ちてない状態から始まる.
            isDroped = false;
            foodSprite.color = preDropColor;
            dropSecond = DROP_SECOND;
        }
        else
        {
            // すでに池に落ちている状態から始まる.
            isDroped = true;
            foodSprite.color = dropedColor;
            gameObject.layer = LayerName.Default;   //レイヤーを変えて当たるようにする.
        }
    }
	
	// Update is called once per frame
	void Update () {
        // 毎秒影時間を減らしていく
        if (!isDroped)
        {
            dropSecond -= Time.deltaTime;
            // 池に落ちた瞬間のトリガ.
            if (dropSecond <= 0.0f)
            {
                isDroped = true;
                foodSprite.color = dropedColor;
                gameObject.layer = LayerName.Default; //レイヤーを変えて当たるようにする.
            }
        }
    }

    /// <summary>
    /// コライダーと当たった瞬間.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
	{
        if (gameObject && other && isDroped && !isDead)
        {
            Tadpole tadPole = other.gameObject.GetComponent<Tadpole>();

            // ノックバック中は処理しない
            if (tadPole && !tadPole.CheckKockBack())
            {
                tadPole.LevelUp();

                // 餌死亡処理、callbackを呼び出す.
                isDead = true;          
                if (OnDeadListeners != null)
                {
                    this.OnDeadListeners(this);
                }
                Destroy(gameObject);
            }
        }
    }

}
