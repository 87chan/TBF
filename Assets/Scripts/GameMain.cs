using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

	public Food Food;
    public TadpoleAI TadpoleAI;
	public int InitialFoodNum;
    public int PlayerNum;
    public int AINum;

    // フィールド上に存在する餌.
    public List<Food> FieldFoods { get{ return fieldFoods; } }
    List<Food> fieldFoods = new List<Food>();

    // フィールド上に存在するおたまじゃくし
    public List<Tadpole> FieldTadpoles { get { return fieldTadpoles; } }
    List<Tadpole> fieldTadpoles = new List<Tadpole>();

    Canvas Canvas;


    /// <summary>
    /// 餌生成.
    /// </summary>
    /// <param name="spawnPosition">生成位置</param>
    public void CreateFood(Vector3 spawnPosition)
    {
        Food newFood = Instantiate(Food, spawnPosition, Quaternion.identity);
        newFood.OnDeadListeners += OnFoodDead;    //死亡時のコールバック.
        fieldFoods.Add(newFood);                  //管理.
    }

    public void CreateTadpoleAI(Vector3 spawnPosition)
    {
        TadpoleAI newTadpoleAI = Instantiate(TadpoleAI, spawnPosition, Quaternion.identity);
        newTadpoleAI.transform.SetParent(Canvas.transform, false);
    }

    void Awake()
    {
        // #todo ひとまずここでフレームレート設定している。今後アプリ自体の初期化場所が決まったら移動させる
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    void Start ()
    {
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Debug.Assert(Canvas, "Canvas という名前で検索しましたがヒエラルキ上に見つかりませんでした");

        Debug.Assert(Food, "GameMainにエサのBehaviorを指定してください");
		if(Food)
		{
            for(int i = 0; i < InitialFoodNum;++i)
			{
                // 餌生成.
                this.CreateFood(AppUtil.GetRandomFieldPos());
			}
		}

        Debug.Assert(Food, "GameMainにAIのBehaviorを指定してください");
        if(TadpoleAI)
        {
            for(int i = 0; i < AINum;++i)
            {
                // AI生成
                this.CreateTadpoleAI(AppUtil.GetRandomFieldPos());
            }
        }

        // オタマジャクシをフィールド上から取得.
        GameObject[] tadpoles = GameObject.FindGameObjectsWithTag("Player");
        foreach(var tadpole in tadpoles)
        {
            fieldTadpoles.Add(tadpole.GetComponent<Tadpole>());
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnFoodDead(Food food)
    {
        fieldFoods.Remove(food);
    }
}
