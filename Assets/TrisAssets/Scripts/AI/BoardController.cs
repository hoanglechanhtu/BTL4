using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoardController : MonoBehaviour {


    private enum GameState
    {
        Running=0,
        Win,
        Tie
    }
    public int AI_difficult = 8;
    GameState state = GameState.Running;
    // Use this for initialization
    public static BoardController instance;
    public Text stateMessage;
    private int moveCount = 0;
    public Type whoseTurn;
    public int n;
    public int m;
    Type winner = Type.None;
    public enum Type {
        None=0,
        Player,
        Enemy
    } 
    public Text col;
    public Text row;
    public Text dif;
    private TicTacToe_Minimax_AI.TicTacToeGame minimaxBOT;
    public int[] getAiMove()
    {
        return minimaxBOT.ComputerMakeMove(AI_difficult);
    }
    float width = 800;
    float height = 600;
    public GameObject tilePrefab;
    Type[,] board;
    public GameObject[,] boardItem;
    private void ChangeTurn() {
        if (whoseTurn == Type.Player)
        {
            whoseTurn = Type.Enemy;
            stateMessage.text = "It's oponent's turn!";
            AIController.instance.move();
        }
        else
        {
            whoseTurn = Type.Player;
            stateMessage.text = "It's your turn!";
        }
    }

    public Type GetType(int x, int y) {
        return board[x, y];
    }
    public void OnMove(int x, int y, Type p) {
        board[x, y] = p;
        if(p == Type.Player)
        {
            minimaxBOT.GetNextMoveFromUser(x, y);
        }
        moveCount++;
        //TODO check game state
        state = checkGameState();
        if (state == GameState.Win)
        {
            if (winner == Type.Player)
            {
                stateMessage.text = "You won!";
            }
            else
            {
                stateMessage.text = "You lose!";
            }
        }
        else if (state == GameState.Tie)
        {
            stateMessage.text = "Tie";
        }
        else {
            ChangeTurn();
        }
        
        
       
    }


  
    private GameState checkGameState()
    {
        for (int j = 0; j < m; j++)
        {
            for (int i = 0; i < n; i++)
            {
                GameState g1 = allFieldTheSame(i, j, 0, 1);
                GameState g2 = allFieldTheSame(i, j, 1, 0);
                GameState g3 = allFieldTheSame(i, j, 1, 1);
                GameState g4 = allFieldTheSame(i, j, 0, -1);
                GameState g5 = allFieldTheSame(i, j, -1, 0);
                GameState g6 = allFieldTheSame(i, j, -1, -1);
                GameState g7 = allFieldTheSame(i, j, 1, -1);
                GameState g8 = allFieldTheSame(i, j, -1, 1);
                if (g1 == GameState.Win
                    || g2 == GameState.Win
                    || g3 == GameState.Win
                    || g4 == GameState.Win
                    || g5 == GameState.Win
                    || g6 == GameState.Win
                    || g7 == GameState.Win
                    || g8 == GameState.Win)
                {
                    return GameState.Win;
                }
            }
        }

        if (winner == Type.None && moveCount == n * m) {
            return GameState.Tie;
        }
        return GameState.Running;
    }

    private GameState allFieldTheSame(int x, int y, int dx, int dy) {
        if (!checkCoor(x, y)) {
            return GameState.Running;
        }
        Boolean isSame = true;
        Type tile = board[y,x];
        if (tile == Type.None) {
            return GameState.Running;
        }
        for (int i = 0; i < 5; i++)
        {
            if (checkCoor(x + i * dx, y + i * dy))
            {
                Debug.Log("Check " + y + i * dy + "    " + x + i * dx);
                if (tile != board[y + i * dy, x + i * dx])
                {
                    isSame = false;
                    break;
                }
            }
            else {
                isSame = false;
            }
        }

        if (isSame)
        {
            if (tile == Type.Player)
            {
                winner = Type.Player;
                return GameState.Win;
            }
            else
            {
                winner = Type.Enemy;
                return GameState.Win;
            }
        }
        else {
            return GameState.Running;
        }

    }

    bool checkCoor(int x, int y) {
        if (x >= 0 && x < n)
        {
            if (y >= 0 && y < m) {
                return true;
            }
        }
        return false;
    }
    public void startGame()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        n = int.Parse(col.text);
        m = int.Parse(row.text);
        AI_difficult = int.Parse(dif.text);
        
        boardItem = new GameObject[m, n];
        InitWith(n, m);
        GridLayoutGroup grid = transform.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(width / n, height / m);
        board = new Type[m, n];
        for (int i = 0; i < m; i++) { 
            for (int j = 0; j < n; j++)
            {
                board[i, j] = Type.None;
               
            }
        }
        //BOT here
        minimaxBOT = new TicTacToe_Minimax_AI.TicTacToeGame(m, n);
        /////////////////////////////////////////////////////////
        whoseTurn = Type.Player;
        stateMessage.text = "It's your turn!";
    }
    void InitWith(int _n, int _m) {
        for (int i = 0; i < _m; i++) {
            for (int j = 0; j < _n; j++) {
                GameObject temp = Instantiate(tilePrefab);
                temp.transform.SetParent(transform);
                temp.transform.localScale = new Vector3(1,1,1);
                temp.name = "Tile" + i + j;
                TileController1 tileControl = temp.transform.GetChild(0).GetComponent<TileController1>();
                temp.transform.GetChild(0).GetComponent<BoxCollider2D>().size = new Vector2(width / n, height / m);
                if (tileControl) {
                    tileControl.setCoodinate(i, j);
                }
               
                boardItem[i, j] = temp;
            }
        }
        Debug.Log("Init done!");
    }
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
