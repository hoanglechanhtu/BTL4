using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    // Use this for initialization
    public float time=1;
    public static AIController instance;
    void Start () {
        instance = this;
	}
    public void move()
    {
        StartCoroutine(onMove());
    }
    IEnumerator onMove() {
        yield return new WaitForSeconds(time);
        int [] AI_move = BoardController.instance.getAiMove();
        if (AI_move != null)
        {
            int x = AI_move[0];
            int y = AI_move[1];
            BoardController.instance.boardItem[x, y].transform.GetChild(0).GetComponent<TileController1>().OnAIMove();
            Debug.Log("On move " + x + "  " + y);
        }
        yield return 0;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
