using System;

namespace Tokenizer
{
    public class TokenizerImpl
    {
        private List<Token> tokens = new List<Token>{};
        private int currentRow = 0;
        private int currentColumn = 0;
        private bool isNumber = false;
        private int numberPoints = 0;
        public List<Token> Tokenizer(string input)
        {
            foreach (char c in input)
            {
                if (c == '\n')
                {
                    currentRow = 0;
                    currentColumn++;
                }
                else currentRow++;
                if (!Char.IsWhiteSpace(c))
                {
                
                if (c.Equals(TokenConstants.LEFT_PAREN)) LeftParenHelper(c.ToString(), currentRow, currentColumn);
                else if (c.Equals(TokenConstants.RIGHT_PAREN)) RightParenHelper(c.ToString(), currentRow, currentColumn);
                else if (c.Equals(TokenConstants.LEFT_CURLY)) LeftCurlyHelper(c.ToString(), currentRow, currentColumn);
                else if (c.Equals(TokenConstants.RIGHT_CURLY)) RightCurlyHelper(c.ToString(), currentRow, currentColumn);
                else if (c.Equals(TokenConstants.PLUS) || 
                    c.Equals(TokenConstants.MINUS) || 
                    c.Equals(TokenConstants.TIMES) || 
                    c.Equals(TokenConstants.INT_DIVISION) ||  
                    c.Equals(TokenConstants.MODULUS) || 
                    c.Equals(TokenConstants.EXPONENT)) OperatorHelper(c.ToString(), currentRow, currentColumn);
                else if (c == ':' || c == '=') AssignmentHelper(c.ToString(), currentRow, currentColumn);
                else if ((Char.IsDigit(c) || c == '.') && isNumber) NumberHelper(c.ToString(), currentRow, currentColumn);
                else VariableHelper(c.ToString(), currentRow, currentColumn);
                }
            }
            return tokens;
        }
        private void VariableHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void OperatorHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void AssignmentHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void ReturnHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void NumberHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void FloatHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void LeftParenHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void RightParenHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void LeftCurlyHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
        private void RightCurlyHelper(string t, int r, int c)
        {
            throw new NotImplementedException();
        }
    }
}