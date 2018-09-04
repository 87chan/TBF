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


    // Use this for initialization
    void Start () {
		if(Food)
		{
			Camera myCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
			float height = myCamera.orthographicSize * 2.0f;
			float width = (height / 16) * 9;
			for(int i = 0; i < InitialFoodNum;++i)
			{
				Vector3 Pos = new Vector3(Random.Range(0, width),Random.Range(0,height),0);
				Pos -= new Vector3(width * 0.5f,height * 0.5f,0);

                // 餌生成.
                Food newFood = Instantiate(Food, Pos, Quaternion.identity);
                newFood.OnDeadListeners += OnFoodDead;    //死亡時のコールバック.
                fieldFoods.Add(newFood);                  //管理.
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
