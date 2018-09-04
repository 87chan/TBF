using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// おたまじゃくしのAI.
/// </summary>
public class TadpoleAI : MonoBehaviour
{
    // パラメータを変更するだけでAIの挙動が変わるようにする = バリエーション豊かにしやすくするため
    // この構造体のパラメータのデータドリブンでAIを記載する.
    // 人間の能力値をベースに考える.
    public struct DATA
    {
        public float 集中力;  //集中力、これが高いほど有利な判定を出しやすくなる.(反応速度や操作精度等)(0～1)

        public float 操作精度; //これが高いと狙った方向に進みやすくなる.(0～1).

        public float 最小反応速度; // 最小反応速度 (秒数指定).反応速度は人間同様に場合によってブレがある.
        public float 最大反応速度; // 最大反応速度 (秒数指定)

        public float hate;     // 他おたまじゃくしに対する憎しみの値.これが高いとタックルを重視する.
        public float hateRate; // hate の 上がりやすさ補正値 (0 ～ 1)
        public float hateDown; // hate の 下がりやすさ.

        public float 空間把握能力; //これが高いと、ちゃんと自分に近い餌を狙う(0～1).

    }

	// Use this for initialization
	void Start ()
    {
        Debug.Assert(tadpole, "GUIにて操作するTadpoleを設定してください");

        GameMain gameMain = GameObject.Find("GameMain").GetComponent<GameMain>();
        Debug.Assert(gameMain, "GameMain という名前で検索しましたがヒエラルキ上に見つかりませんでした");

        this.fieldFoods = gameMain.FieldFoods;
        this.fieldTadpoles = gameMain.FieldTadpoles;
    }

    // Update is called once per frame
    void Update ()
    {
        // まずは状況判断. 今の状況を確認して反応速度の秒数分だけずらして行動させる.
        float 反応速度 = Random.Range(Mathf.Lerp(data.最小反応速度, data.最大反応速度, data.集中力), data.最大反応速度);

        //とりあえず適当な餌のいちに向かっていくだけ.

        //Debug.Log(fieldFoods.Count);
	}

    public DATA data;       // 
    public Tadpole tadpole; // 操作対象.

    // フィールド上に存在する餌.
    private List<Food> fieldFoods;
    // フィールド上に存在するおたまじゃくし
    private List<Tadpole> fieldTadpoles;
}
