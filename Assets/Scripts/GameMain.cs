using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {

	public Food Food;
	public int InitialFoodNum;

    // フィールド上に存在する餌.
    public List<Food> FieldFoods { get{ return fieldFoods; } }
    List<Food> fieldFoods = new List<Food>();

    // フィールド上に存在するおたまじゃくし
    public List<Tadpole> FieldTadpoles { get { return fieldTadpoles; } }
    List<Tadpole> fieldTadpoles = new List<Tadpole>();


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


    // Use this for initialization
    void Start () {
		if(Food)
		{
			Vector2 Size = AppUtil.GetOrthographicSize();
			for(int i = 0; i < InitialFoodNum;++i)
			{
				Vector3 Pos = new Vector3(Random.Range(0, Size.x),Random.Range(0,Size.y),0);
				Pos -= new Vector3(Size.x * 0.5f, Size.y * 0.5f,0);

                // 餌生成.
                this.CreateFood(Pos);
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
