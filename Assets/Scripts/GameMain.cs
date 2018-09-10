using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour {

	public Food Food;
    public TadpoleAI TadpoleAI;
    public TadpoleTouchController TadpolePlayer;
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

    // 前フレームの傾き
    Vector3 PrevAccel;

    int SHAKE_NUM_TO_RESTART = 4;
    int ShakeCount;

    void InitVariable()
    {
        ShakeCount = 0;
    }

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

    public void CreatePlayer(int playerNum)
    {
        if(playerNum == 2)
        {
            this.CreatePlayerCore(new Vector2(0, 320), new Vector2(720, 640));
            this.CreatePlayerCore(new Vector2(0, -320), new Vector2(720, 640));
        }
        else if(playerNum == 3)
        {
            this.CreatePlayerCore(new Vector2(0, 320), new Vector2(720, 640));
            this.CreatePlayerCore(new Vector2(-180, -320), new Vector2(360, 640));
            this.CreatePlayerCore(new Vector2(180, -320), new Vector2(360, 640));
        }
        else
        {
            Debug.Assert(false, "指定したプレイヤー数は想定されていません");
        }
    }

    void CreatePlayerCore(Vector2 spawnPosition, Vector2 size)
    {
        TadpoleTouchController newTadpolePlayer = Instantiate(TadpolePlayer, new Vector3(), Quaternion.identity);
        newTadpolePlayer.transform.SetParent(Canvas.transform, false);
        newTadpolePlayer.ChangePlayerTouchInfo(spawnPosition, size);
    }

    void Awake()
    {
        // #todo ひとまずここでフレームレート設定している。今後アプリ自体の初期化場所が決まったら移動させる
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    void Start ()
    {
        this.GameMainInit();
    }

    // ゲームを開始する為の準備処理
    void GameMainInit()
    {
        this.InitVariable();

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

        Debug.Assert(TadpolePlayer, "GameMainにプレイヤーのBehaviorを指定してください");
        if(TadpolePlayer)
        {
            // プレイヤー生成
            this.CreatePlayer(PlayerNum);
        }

        // オタマジャクシをフィールド上から取得.
        GameObject[] tadpoles = GameObject.FindGameObjectsWithTag("Player");
        foreach(var tadpole in tadpoles)
        {
            fieldTadpoles.Add(tadpole.GetComponent<Tadpole>());
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		this.CheckRestart();
	}

    void CheckRestart()
    {
        Vector3 accel = Input.acceleration;

        // シェイクを検知
        if (Vector3.Dot(PrevAccel, accel) < 0.0f)
        {
            ++ShakeCount;
            // 一定数振ったと見なしてリスタート
            if(ShakeCount >= SHAKE_NUM_TO_RESTART)
            {
                this.Restart();
            }
        }
        PrevAccel = Input.acceleration;
    }

    void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    void OnFoodDead(Food food)
    {
        fieldFoods.Remove(food);
    }
}
