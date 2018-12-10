using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe_Minimax_AI
{
    enum GridEntry:byte
    {
        Empty,
        PlayerX,
        PlayerO
    }

    sealed class Board
    {
        GridEntry[] m_Values;
        int m_Score ;
        bool m_TurnForPlayerX ;
        int m_height;
        int m_width;
        int m_win_num;
        public int RecursiveScore
        {
            get;
            private set;
        }
        public bool GameOver
        {
            get;
            private set;
        }

        public Board(GridEntry[] values, bool turnForPlayerX, int height, int width)
        {
            m_TurnForPlayerX = turnForPlayerX;
            m_Values = values;
            m_height = height;
            m_width = width;
            int min_hw = (height > width ? width : height);
            m_win_num = (min_hw > 5 ? 5 : min_hw);
            ComputeScore();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_height; i++)
            {
                for (int j = 0; j < m_width; j++)
                {
                    GridEntry v = m_Values[i* m_width + j];
                    char c = '-';
                    if (v == GridEntry.PlayerX)
                        c = 'X';
                    else if (v == GridEntry.PlayerO)
                        c = 'O';
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            sb.AppendFormat("score={0},ret={1},{2}", m_Score, RecursiveScore, m_TurnForPlayerX);
            return sb.ToString();
        }

        public Board GetChildAtPosition(int x, int y)
        {
            int i = x + y*m_width;
            GridEntry[] newValues = (GridEntry[])m_Values.Clone();

            if (m_Values[i] != GridEntry.Empty) 
                throw new Exception(string.Format("invalid index [{0},{1}] is taken by {2}",x, y, m_Values[i]));

            newValues[i] = m_TurnForPlayerX?GridEntry.PlayerX:GridEntry.PlayerO;
            return new Board(newValues, !m_TurnForPlayerX, m_height, m_width);
        }

        static public int[] getDifferenceXY(Board a, Board b)
        {
            if (a.m_Values.Length == b.m_Values.Length)
            {
                for (int i = 0; i < a.m_Values.Length; i++)
                {
                    if (a.m_Values[i] != b.m_Values[i])
                    {
                        int m_y = i / a.m_width;
                        int m_x = i - m_y * a.m_width;
                        return new int[2] { m_x, m_y };
                    }
                }
            }
            return null;
        }

        public bool IsTerminalNode()
        {
            if (GameOver)
                return true;
            //if all entries are set, then it is a leaf node
            foreach (GridEntry v in m_Values)
            {
                if (v == GridEntry.Empty)
                    return false;
            }
            return true;
        }
        public IEnumerable<Board> GetChildren()
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                if (m_Values[i] == GridEntry.Empty)
                {
                    GridEntry[] newValues = (GridEntry[])m_Values.Clone();
                    newValues[i] = m_TurnForPlayerX ? GridEntry.PlayerX : GridEntry.PlayerO;
                    yield return new Board(newValues, !m_TurnForPlayerX,m_height,m_width);
                }
            }
        }


        public int MiniMaxShortVersion(int depth, int alpha, int beta, out Board childWithMax)
        {
            childWithMax = null;
            if (depth == 0 || IsTerminalNode())
            {
                //When it is turn for PlayO, we need to find the minimum score.
                RecursiveScore = m_Score;
                return m_TurnForPlayerX ? m_Score : -m_Score;
            }

            foreach (Board cur in GetChildren())
            {
                Board dummy;
                int score = -cur.MiniMaxShortVersion(depth - 1, -beta, -alpha, out dummy);
                if (alpha < score)
                {
                    alpha = score;
                    childWithMax = cur;
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
            }

            RecursiveScore = alpha;
            return alpha;
        }
        
        public int MiniMax(int depth, bool needMax, int alpha, int beta, out Board childWithMax)
        {
            childWithMax = null;
            System.Diagnostics.Debug.Assert(m_TurnForPlayerX == needMax);
            if (depth == 0 || IsTerminalNode())
            {
                RecursiveScore = m_Score;
                return m_Score;
            }

            foreach (Board cur in GetChildren())
            {
                Board dummy;
                int score = cur.MiniMax(depth - 1, !needMax, alpha, beta, out dummy);
                if (!needMax)
                {
                    if (beta > score)
                    {
                        beta = score;
                        childWithMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (alpha < score)
                    {
                        alpha = score;
                        childWithMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            }

            RecursiveScore = needMax ? alpha : beta;
            return RecursiveScore;
        }

        public Board FindNextMove(int depth)
        {
            Board ret = null;
            MiniMax(depth, m_TurnForPlayerX, int.MinValue + 1, int.MaxValue - 1, out ret);
            return ret;
        }

        int GetScoreForOneLine(GridEntry[] values)
        {
            int countX=0, countO=0;
            foreach (GridEntry v in values)
            {
                if (v == GridEntry.PlayerX)
                    countX++;
                else if (v == GridEntry.PlayerO)
                    countO++;
            }

            if (countO == m_win_num || countX == m_win_num)
            {
                GameOver = true;
            }

            //The player who has turn should have more advantage.
            //What we should have done
            int advantage = 1;
            if (countO == 0)
            {
                if (m_TurnForPlayerX)
                    advantage = m_win_num;
                return (int)System.Math.Pow(10, countX) * advantage;
            }
            else if (countX == 0)
            {
                if (!m_TurnForPlayerX)
                    advantage = m_win_num;
                return -(int)System.Math.Pow(10, countO) * advantage;
            }
            return 0;
        }

        void ComputeScore()
        {
            int ret = 0;
            //horizontal
            for (int i = 0;i< m_width*(m_height - m_win_num + 1); i++)
            {
                GridEntry[] line = new GridEntry[m_win_num];
                for (int j = 0; j < m_win_num; j++)
                {
                    line[j] = m_Values[i + j * m_width];
                }
                ret += GetScoreForOneLine(line);
            }
            //vertical
            for (int i = 0; i < m_height; i++)
            {
                for (int k = 0; k < m_width-m_win_num+1; k++) {
                    GridEntry[] line = new GridEntry[m_win_num];
                    for (int j = 0; j < m_win_num; j++)
                    {
                        line[j] = m_Values[i*m_width+k+j];
                    }
                    ret += GetScoreForOneLine(line);
                }
            }
            //cross 1
            for (int i = 0; i < m_height - m_win_num + 1; i++)
            {
                for (int k = 0; k < m_width - m_win_num + 1; k++)
                {
                    GridEntry[] line = new GridEntry[m_win_num];
                    for (int j = 0; j < m_win_num; j++)
                    {
                        line[j] = m_Values[i * m_width + k + j* m_width + j];
                    }
                    ret += GetScoreForOneLine(line);
                }
            }
            //cross 2
            for (int i = 0; i < m_height - m_win_num + 1; i++)
            {
                for (int k = m_width - 1; k > m_win_num - 2; k--)
                {
                    GridEntry[] line = new GridEntry[m_win_num];
                    for (int j = 0; j < m_win_num; j++)
                    {
                        line[j] = m_Values[i * m_width + k + j * m_width - j];
                    }
                    ret += GetScoreForOneLine(line);
                }
            }
            m_Score = ret;
        }
    }


    

    class TicTacToeGame
    {

        public Board Current
        {
            get;
            private set;
        }
        Board init;

        public TicTacToeGame(int height, int width)
        {
            //mxn
            GridEntry[] values = Enumerable.Repeat(GridEntry.Empty, height * width).ToArray();
            init = new Board(values, true, height, width);
            Current = init;
        }

        public int[] ComputerMakeMove(int depth)
        {
            Board next = Current.FindNextMove(depth);
            if (next != null)
            {
                int[] aXY = Board.getDifferenceXY(Current, next);
                Current = next;
                return aXY;
            }
            else return null;
        }

        public Board GetInitNode()
        {
            return init;
        }
        public void GetNextMoveFromUser(int x,int y)
        {
            if (Current.IsTerminalNode())
                return;
            try
            {
                Current = Current.GetChildAtPosition(x, y);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void GetNextMoveFromUser()
        {
            if (Current.IsTerminalNode())
                return;

            while (true)
            {
                try
                {
                    Console.WriteLine("Current Node is\n{0}\n Please type in x:[0-2]", Current);
                    int x = int.Parse(Console.ReadLine());
                    Console.WriteLine("Please type in y:[0-2]");
                    int y = int.Parse(Console.ReadLine());
                    Console.WriteLine("x={0},y={1}", x, y);
                    Current = Current.GetChildAtPosition(x, y);
                    Console.WriteLine(Current);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int width = 8;
            int height = 8;
            TicTacToeGame game = new TicTacToeGame(height,width);
            bool stop = false;
            while (!stop)
            {
                bool userFirst = false;
                game = new TicTacToeGame(height, width);
                Console.WriteLine("User play against computer, Do you place the first step?[y/n]");
                if (Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    userFirst = true;
                }

                int depth = 8;
                Console.WriteLine("Please select level:[1..8]. 1 is easiet, 8 is hardest");
                int.TryParse(Console.ReadLine(), out depth);

                Console.WriteLine("{0} play first, level={1}", userFirst ? "User" : "Computer", depth);

                while (!game.Current.IsTerminalNode())
                {
                    if (userFirst)
                    {
                        game.GetNextMoveFromUser();
                        game.ComputerMakeMove(depth);
                    }
                    else
                    {
                        game.ComputerMakeMove(depth);
                        game.GetNextMoveFromUser();
                    }
                }
                Console.WriteLine("The final result is \n" + game.Current);
                if (game.Current.RecursiveScore < -200)
                    Console.WriteLine("PlayerO has won.");
                else if (game.Current.RecursiveScore > 200)
                    Console.WriteLine("PlayerX has won.");
                else
                    Console.WriteLine("It is a tie.");

                Console.WriteLine("Try again?[y/n]");
                if (!Console.ReadLine().StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
                {
                    stop = true;
                }
            }

            Console.WriteLine("bye");
        }
    }
}
