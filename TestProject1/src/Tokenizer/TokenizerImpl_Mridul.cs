using System;
using System.Data.Common;
using System.Reflection.Metadata;


namespace Tokenizer_Mridul;

public static class TokenizerImpl
{
    public static List<Token> Tokenize(string source_code)
    {
        List<Token> Tokens = new List<Token>();
        string curr = "";
        string currType = "";
        string extendedSourceCode = source_code + " ";
        foreach (char c in extendedSourceCode)
        {
            if (currType == "number")
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    Tokens.Add(HandleIntegerOrDouble(curr));
                    curr = c.ToString();
                    currType = "";
                }
                else curr += c;
            }
            else if (currType == "string")
            {
                if (!char.IsLetter(c))
                {
                    Tokens.Add(HandleString(curr));
                    curr = "";
                    currType = "";
                }
                else curr += c;
            }
            else if (currType == "assignment")
            {
                if (c != '=')
                {
                    Tokens.Add(HandleAssignment(curr));
                    curr = "";
                    currType = "";
                }
                else
                {
                    curr += c;
                }
            }
            else if (currType == "parens")
            {
                Tokens.Add(HandleParensAndCurly(curr));
                curr = "";
                currType = "";
            }
            else if (currType == "operator")
            {
                Tokens.Add(HandleOperator(curr));
                curr = "";
                currType = "";
            }

            // Now, process the current character if we just finished building a token, that it, the last character in the token was the c-1 character.
            if (currType == "")
            {   
                if (c == '(' || c == ')' || c == '{' || c == '}') (curr, currType) = (c.ToString(), "parens");
                else if (c == '+' || c == '-' || c == '*' || c == '/') (curr, currType) = (c.ToString(), "operator");
                else if (char.IsDigit(c)) (curr, currType) = (c.ToString(), "number");
                else if (char.IsLetter(c)) (curr, currType) = (c.ToString(), "string");
                else if (char.IsWhiteSpace(c)) continue;
                else if (c == ':') (curr, currType) = (c.ToString(), "assignment");
            }

        }

        return Tokens;
    }

    public static Token HandleAssignment(string curr)
    {
        return new Token(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT);
    }

    public static Token HandleIntegerOrDouble(string curr)
    {
        if (curr.Contains('.')) return new Token(curr, TokenType.DOUBLE);
        else return new Token(curr, TokenType.INTEGER);
    }

    public static Token HandleString(string curr)
    {
        if (curr == "return") return new Token(TokenConstants.RETURN, TokenType.RETURN);
        else return new Token(curr, TokenType.VARIABLE);
    }

    public static Token HandleParensAndCurly(string curr)
    {
        if (curr == "(") return new Token(TokenConstants.LEFT_PAREN, TokenType.LEFT_PAREN);
        else if (curr == ")") return new Token(TokenConstants.RIGHT_PAREN, TokenType.RIGHT_PAREN);
        else if (curr == "{") return new Token(TokenConstants.LEFT_CURLY, TokenType.LEFT_CURLY);
        else if (curr == "}") return new Token(TokenConstants.RIGHT_CURLY, TokenType.RIGHT_CURLY);
        else throw new Exception("Invalid character: " + curr);
    }

    public static Token HandleOperator(string curr)
    {
        if (curr == "+") return new Token(TokenConstants.PLUS, TokenType.OPERATOR);
        else if (curr == "-") return new Token(TokenConstants.MINUS, TokenType.OPERATOR);
        else if (curr == "*") return new Token(TokenConstants.MULTIPLY, TokenType.OPERATOR);
        else if (curr == "/") return new Token(TokenConstants.DIVIDE, TokenType.OPERATOR);
        else throw new Exception("Invalid operator: " + curr);
    }
}