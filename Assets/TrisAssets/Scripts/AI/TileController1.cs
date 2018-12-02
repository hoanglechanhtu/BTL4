using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TileController1 : MonoBehaviour {

    public Sprite player;
    public Sprite enemy;
    public int x;
    public int y;
    public Image image;
    // Use this for initialization

    public void setCoodinate(int _x, int _y) {
        x = _x;
        y = _y;
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        Debug.Log("mouse down");
        if (BoardController.instance.whoseTurn == BoardController.Type.Player) {
            if (BoardController.instance.GetType(x, y) == BoardController.Type.None) {
                
                if (image) {
                    image.sprite = player;
                }
                BoardController.instance.OnMove(x, y, BoardController.Type.Player);

            }
        }
    }

    public void OnAIMove() {
        if (BoardController.instance.whoseTurn == BoardController.Type.Enemy)
        {
            if (BoardController.instance.GetType(x, y) == BoardController.Type.None)
            {
                
                if (image)
                {
                    image.sprite = enemy;
                }
                BoardController.instance.OnMove(x, y, BoardController.Type.Enemy);

            }
        }
    }
}
