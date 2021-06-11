using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeLogic
{
    public class Player
    {
        public enum ePlayerType { Human = 1, Computer };

        private readonly string m_Name;
        private readonly char m_Sign;
        private ePlayerType m_Type;
        private int m_Score;

        public Player(char i_Sign, int i_Type, string i_Name)
        {
            m_Sign = i_Sign;
            m_Type = (ePlayerType)i_Type;
            m_Score = 0;
            m_Name = i_Name;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public char Sign
        {
            get { return m_Sign; }
        }

        public int Score
        {
            get { return m_Score; }
        }

        public ePlayerType Type
        {
            get { return m_Type; }
        }

        public void UpdateScore()
        {
            m_Score++;
        }
    }
}
