using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // 死亡時のコールバック.
    public delegate void OnDead(Food food);
    public OnDead OnDeadListeners;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(gameObject && other)
		{
		    Tadpole tadPole = other.gameObject.GetComponent<Tadpole>();

		    // ノックバック中は処理しない
		    if(tadPole && !tadPole.CheckKockBack())
		    {
			    tadPole.LevelUp();
                if(OnDeadListeners != null)
                {
                    this.OnDeadListeners(this);
                }
                Destroy(gameObject);
		    }
		}
	}

    // 
}
