using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// おたまじゃくしのAI.
/// </summary>
public class TadpoleAI : MonoBehaviour
{
    // #todo とりあえず適当に日本語変数名になっているので後で変える.

    // パラメータを変更するだけでAIの挙動が変わるようにする = バリエーション豊かにしやすくするため
    // この構造体のパラメータ使ってデータドリブンなAIを目指す.
    // 人間の能力値をベースに考える.
    [System.Serializable]
    public struct DATA
    {
        public float 集中力;  //集中力、これが高いほど有利な判定を出しやすくなる.(反応速度や操作精度等)(0～1)

        public float 操作精度; //これが高いと狙った方向に進みやすくなる.(0～1). 
        public float 操作精度のブレ度数; //操作精度のブレ度数.

        public float 最小反応速度; // 最小反応速度 (秒数指定).反応速度は人間同様に場合によってブレがある.
        public float 最大反応速度; // 最大反応速度 (秒数指定)

        public float hate;     // 他おたまじゃくしに対する憎しみの値.これが高いとタックルを重視する.
        public float hateRate; // hate の 上がりやすさ補正値 (0 ～ 1)
        public float hateDown; // hate の 下がりやすさ.

        public float 空間把握能力; //これが高いと、ちゃんと自分に近い餌を狙う(0～1).

        public float タップ連打可能秒数; 　// 次のタップ操作までこの秒数指定分だけ待つ(秒数).
        public float タップクールタイム;  // 次のタップ操作までのクールタイム(秒数)
    }

	// Use this for initialization
	void Start ()
    {
        tadpole = this.GetComponentInChildren<Tadpole>();
        Debug.Assert(tadpole, "おたまじゃくしが見つかりません");

        GameMain gameMain = GameObject.Find("GameMain").GetComponent<GameMain>();
        Debug.Assert(gameMain, "GameMain という名前で検索しましたがヒエラルキ上に見つかりませんでした");

        this.fieldFoods = gameMain.FieldFoods;
        this.fieldTadpoles = gameMain.FieldTadpoles;

        // #todo データ仮適当設定.
        this.Data.集中力 = 0.8f;
        this.Data.タップ連打可能秒数 = 0.7f;
        this.Data.操作精度 = 0.75f;
        this.Data.操作精度のブレ度数 = 10.0f; 
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMain.bGameStart)
        {
            // まずは状況判断. 今の状況を確認して反応速度の秒数分だけずらして行動させる.
            float 反応速度 = Random.Range(Mathf.Lerp(Data.最小反応速度, Data.最大反応速度, Data.集中力), Data.最大反応速度);

            //とりあえず適当な餌の位置に向かっていくだけ.
            if (fieldFoods.Count > 0)
            {
                if (this.Data.タップクールタイム < 0)
                {
                    this.Data.タップクールタイム = this.Data.タップ連打可能秒数; //クールタイム回復.


                    //餌の方向へ向かう.
                    var direction = fieldFoods[0].transform.position - tadpole.transform.position;
                    direction.z = 0.0f;

                    // もしも餌と重なり合っていたら適当に上方向に動かす
                    if (direction.sqrMagnitude < Mathf.Epsilon)
                    {
                        direction.y = 1.0f;
                    }

                    // 操作精度判定.
                    // 進みたい方向に対して ある程度の範囲のブレが生じる.
                    direction = GetPrecisionDir(direction);

                    // 進める.
                    tadpole.MoveDirection(direction.normalized);
                }
            }

            // 
            foreach (var tad in fieldTadpoles)
            {
                if (this.tadpole == tad) //自分自身は除く.
                {
                    continue;
                }
                // 自分より強いやつにタックル.
            }

            // クールタイムは毎秒減る.
            this.Data.タップクールタイム -= Time.deltaTime;
        }
	}

    /// <summary>
    /// 操作精度を考慮した移動方向を返す.
    /// </summary>
    /// <returns>操作精度を考慮した移動方向</returns>
    private Vector3 GetPrecisionDir(Vector3 direction)
    {
        float shakeDirection = Random.Range(0, 1) == 0 ? -1.0f : 1.0f;  //ブレ方向.
        float precision = Random.Range(Mathf.Lerp(Data.操作精度, 1.0f, Data.集中力), 1.0f);    //最終的な精度.
        Quaternion q = Quaternion.Euler(0, 0, precision * this.Data.操作精度のブレ度数 * shakeDirection);
        return  q * direction;
    }

    public DATA Data;       // 
    Tadpole tadpole; // 操作対象.

    private float 意思決定した時間 = 0.0f;

    // フィールド上に存在する餌.
    private List<Food> fieldFoods;
    // フィールド上に存在するおたまじゃくし
    private List<Tadpole> fieldTadpoles;
}
