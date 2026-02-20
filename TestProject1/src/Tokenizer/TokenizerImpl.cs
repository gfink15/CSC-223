using System;
using Utilities;

namespace Tokenizer
{
    public static class TokenizerImpl
    {
        private static  List<Token> tokens = new List<Token>();
        private static Token newToken;
        private static int currentRow = 0;
        private static int currentColumn = 0;
        private static bool isNumber = false;
        private static bool isVar = false;
        private static int numberPoints = 0;
        private static string currentToken = "";
        private static char lastChar;
        public static List<Token> Tokenize(string input)
        {
            Console.WriteLine("tokenizer activate");
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
                    if (isVar)
                    {
                        VariableHelper(currentToken, currentRow, currentColumn);
                    }
                    if (isNumber)
                    {
                        NumberHelper(currentToken, currentRow, currentColumn);
                    }
                    currentToken = "";
                    isVar = false;
                    isNumber = false;
                    numberPoints = 0;
                    if (c.Equals(TokenConstants.LEFT_PAREN) ||
                        c.Equals(TokenConstants.RIGHT_PAREN) ||
                        c.Equals(TokenConstants.LEFT_CURLY) ||
                        c.Equals(TokenConstants.RIGHT_CURLY)) ParenCurlyHelper(c.ToString(), currentRow, currentColumn);
                    else if (c.Equals(TokenConstants.PLUS) || 
                        c.Equals(TokenConstants.MINUS) || 
                        c.Equals(TokenConstants.TIMES) || 
                        c.Equals(TokenConstants.INT_DIVISION) ||  
                        c.Equals(TokenConstants.MODULUS)) OperatorHelper(c.ToString(), currentRow, currentColumn);
                    else if (lastChar.ToString() + c.ToString() == TokenConstants.ASSIGNMENT) AssignmentHelper(lastChar.ToString() + c.ToString(), currentRow, currentColumn);
                    lastChar = c;
                }
                else if (c == '\n')
                {
                    currentColumn = 0;
                    currentRow++;
                }
                else currentColumn++;
                
            }
            Console.WriteLine("Returning "+tokens);
            return tokens;
        }
        private static void VariableHelper(string t, int r, int c)
        {
            throw new Exception("Variable detected: "+t);
            if (t == TokenConstants.RETURN) tokens.Append(new Token(t, TokenType.RETURN, r, c, t.Length));
            else if (GeneralUtils.IsValidVariable(t)) tokens.Append(new Token(t, TokenType.VARIABLE, r, c, 1));
            else throw new ArgumentException("Invalid variable '"+t+"' passed to VariableHelper at Row "+currentRow+" Col "+currentColumn);
            throw new Exception("After variable helper: "+tokens);
        }
        private static void OperatorHelper(string t, int r, int c)
        {
            if (t + lastChar == TokenConstants.FLOAT_DIVISION) tokens.Add(new Token(TokenConstants.FLOAT_DIVISION, TokenType.OPERATOR, r, c, 2));
            else if (t + lastChar == TokenConstants.EXPONENT) tokens.Add(new Token(TokenConstants.EXPONENT, TokenType.OPERATOR, r, c, 2));
            else if (t.Equals(TokenConstants.PLUS)) tokens.Add(new Token(TokenConstants.PLUS, TokenType.OPERATOR, r, c, 1));
            else if (t.Equals(TokenConstants.MINUS)) tokens.Add(new Token(TokenConstants.MINUS, TokenType.OPERATOR, r, c, 1));
            else if (t.Equals(TokenConstants.TIMES)) tokens.Add(new Token(TokenConstants.TIMES, TokenType.OPERATOR, r, c, 1));
            else if (t.Equals(TokenConstants.INT_DIVISION)) tokens.Add(new Token(TokenConstants.INT_DIVISION, TokenType.OPERATOR, r, c, 1));
            else if (t.Equals(TokenConstants.MODULUS)) tokens.Add(new Token(TokenConstants.MODULUS, TokenType.OPERATOR, r, c, 1));
            else throw new ArgumentException("Invalid operator '"+t+"' passed to OperatorHelper at Row "+currentRow+" Col "+currentColumn);
        }
        private static void AssignmentHelper(string t, int r, int c)
        {
            if (t + lastChar == TokenConstants.ASSIGNMENT) tokens.Add(new Token(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT, r, c, 2));
            else throw new ArgumentException("Invalid assigner '"+t+"' passed to AssignmentHelper at Row "+currentRow+" Col "+currentColumn);
        }
        private static void NumberHelper(string t, int r, int c)
        {
            if (numberPoints == 0) tokens.Add(new Token(t, TokenType.INTEGER, r, c, t.Length));
            else if (numberPoints == 1) tokens.Add(new Token(t, TokenType.FLOAT, r, c, t.Length));
            else throw new ArgumentException("Invalid character '"+t+"' passed to NumberHelper at Row "+currentRow+" Col "+currentColumn);
        }
        private static void ParenCurlyHelper(string t, int r, int c)
        {
            if (t.Equals(TokenConstants.LEFT_PAREN)) tokens.Add(new Token(TokenConstants.LEFT_PAREN, TokenType.LEFT_PAREN, r, c, 1));
            else if (t.Equals(TokenConstants.RIGHT_PAREN)) tokens.Add(new Token(TokenConstants.RIGHT_PAREN, TokenType.RIGHT_PAREN, r, c, 1));
            else if (t.Equals(TokenConstants.LEFT_CURLY)) tokens.Add(new Token(TokenConstants.LEFT_CURLY, TokenType.LEFT_CURLY, r, c, 1));
            else if (t.Equals(TokenConstants.RIGHT_CURLY)) tokens.Add(new Token(TokenConstants.RIGHT_CURLY, TokenType.RIGHT_CURLY, r, c, 1));
            else throw new ArgumentException("Invalid character '"+t+"' passed to ParenCurlyHelper at Row "+currentRow+" Col "+currentColumn);
        }
    }
}