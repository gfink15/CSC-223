using System;
using Utilities;

namespace Tokenizer
{
    public class TokenizerImpl
    {
        private List<Token> tokens = new List<Token>{};
        private int currentRow = 0;
        private int currentColumn = 0;
        private bool isNumber = false;
        private bool isVar = false;
        private int numberPoints = 0;
        private string currentToken = "";
        private char lastChar;
        public List<Token> Tokenizer(string input)
        {
            foreach (char c in input)
            {
                
                if (Char.IsLetter(c))
                {
                    if (isNumber)
                    {
                        throw new ArgumentException("Unexpected character '"+c+"' at Row "+currentRow+" Col "+currentColumn+"; Expected a number");
                    }
                    else
                    {
                        isVar = true;
                        currentToken += c;
                    }  
                }
                else if (Char.IsNumber(c))
                {
                    currentToken += c;
                    if (!isVar) isNumber = true;
                }
                else if (c == '.')
                {
                    if (isNumber && numberPoints > 1)
                    {
                        throw new ArgumentException("Unexpected character '"+c+"' at Row "+currentRow+" Col "+currentColumn+"; Too many decimals");
                    }
                    else if (isNumber)
                    {
                        numberPoints += 1;
                        currentToken += c;
                    }
                    else
                    {
                        throw new ArgumentException("Unexpected character '"+c+"' at Row "+currentRow+" Col "+currentColumn+"; Decimal not allowed here");
                    }
                }
                else if (!Char.IsWhiteSpace(c))
                {
                    isVar = false;
                    if (isVar)
                    if (c.Equals(TokenConstants.LEFT_PAREN)) LeftParenHelper(c.ToString(), currentRow, currentColumn);
                    else if (c.Equals(TokenConstants.RIGHT_PAREN)) RightParenHelper(c.ToString(), currentRow, currentColumn);
                    else if (c.Equals(TokenConstants.LEFT_CURLY)) LeftCurlyHelper(c.ToString(), currentRow, currentColumn);
                    else if (c.Equals(TokenConstants.RIGHT_CURLY)) RightCurlyHelper(c.ToString(), currentRow, currentColumn);
                    else if (c.Equals(TokenConstants.PLUS) || 
                        c.Equals(TokenConstants.MINUS) || 
                        c.Equals(TokenConstants.TIMES) || 
                        c.Equals(TokenConstants.INT_DIVISION) ||  
                        c.Equals(TokenConstants.EXPONENT)) OperatorHelper(c.ToString(), currentRow, currentColumn);
                    else if (c == ':' || c == '=') AssignmentHelper(c.ToString(), currentRow, currentColumn);
                    lastChar = c;
                }
                else if (c == '\n')
                {
                    currentColumn = 0;
                    currentRow++;
                }
                else currentColumn++;
                
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