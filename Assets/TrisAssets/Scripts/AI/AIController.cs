using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    // Use this for initialization
    public float time=3;
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

        //TODO AI code 
        //Board data
        // BoardController.instance.board[x,y]
        //Value 
        //BoardController.Type.None
        //BoardController.Type.Enemy
        //BoardController.Type.Player

        //Start
        //best move x,y
        int x =0;
        int y=0;
        do
        {

            x = Random.Range(0, BoardController.instance.m - 1);
            y = Random.Range(0, BoardController.instance.n - 1);

        }
        while (BoardController.instance.GetType(x, y) != BoardController.Type.None);


        //End

        BoardController.instance.boardItem[x, y].transform.GetChild(0).GetComponent<TileController1>().OnAIMove();
         
        Debug.Log("On move " + x + "  " + y);
        yield return 0;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
