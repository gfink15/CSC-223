/**
 * Tokenizer Implementation: Provides functionality for
 * tokenizing source code into a list of tokens
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/19/2026
 */

using System;
using System.Data.Common;
using System.Reflection.Metadata;

namespace Tokenizer_Mridul;

/// <summary>
/// Provides tokenization functionality for parsing source code into tokens.
/// </summary>
public static class TokenizerImpl
{
    /// <summary>
    /// Tokenizes the given source code into a list of tokens.
    /// </summary>
    /// <param name="source_code">The source code string to tokenize.</param>
    /// <returns>A list of tokens parsed from the source code.</returns>
    public static List<Token> Tokenize(string source_code)
    {
        List<Token> Tokens = new List<Token>();
        string curr = "";  // Current token being built
        string currType = "";  // Type of current token being built
        string extendedSourceCode = source_code + " ";  // Add space to process last token
        foreach (char c in extendedSourceCode)
        {
            // Handle completion of number tokens
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
            // Handle completion of string tokens (keywords or variables)
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
            // Handle completion of assignment tokens
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
            // Handle completion of parentheses and curly braces
            else if (currType == "parens")
            {
                Tokens.Add(HandleParensAndCurly(curr));
                curr = "";
                currType = "";
            }
            // Handle completion of operator tokens
            else if (currType == "operator")
            {
                if (c != '/' && c != '*')
                {
                    Tokens.Add(HandleOperator(curr));
                    curr = "";
                    currType = "";
                }
                else
                {
                    curr += c;
                }
            }

            // Start building a new token based on current character
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

    /// <summary>
    /// Creates an assignment token.
    /// </summary>
    /// <param name="curr">The current token string.</param>
    /// <returns>An assignment token.</returns>
    public static Token HandleAssignment(string curr)
    {
        return new Token(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT);
    }

    /// <summary>
    /// Creates an integer or double token based on the presence of a decimal point.
    /// </summary>
    /// <param name="curr">The current token string.</param>
    /// <returns>A token of type INTEGER or DOUBLE.</returns>
    public static Token HandleIntegerOrDouble(string curr)
    {
        if (curr.Contains('.')) return new Token(curr, TokenType.DOUBLE);
        else return new Token(curr, TokenType.INTEGER);
    }

    /// <summary>
    /// Creates a token for keywords or variables.
    /// </summary>
    /// <param name="curr">The current token string.</param>
    /// <returns>A RETURN token if the string is "return", otherwise a VARIABLE token.</returns>
    public static Token HandleString(string curr)
    {
        if (curr == "return") return new Token(TokenConstants.RETURN, TokenType.RETURN);
        else return new Token(curr, TokenType.VARIABLE);
    }

    /// <summary>
    /// Creates a token for parentheses or curly braces.
    /// </summary>
    /// <param name="curr">The current token string.</param>
    /// <returns>The appropriate parenthesis or curly brace token.</returns>
    /// <exception cref="Exception">Thrown when the character is not a valid parenthesis or curly brace.</exception>
    public static Token HandleParensAndCurly(string curr)
    {
        if (curr == "(") return new Token(TokenConstants.LEFT_PAREN, TokenType.LEFT_PAREN);
        else if (curr == ")") return new Token(TokenConstants.RIGHT_PAREN, TokenType.RIGHT_PAREN);
        else if (curr == "{") return new Token(TokenConstants.LEFT_CURLY, TokenType.LEFT_CURLY);
        else if (curr == "}") return new Token(TokenConstants.RIGHT_CURLY, TokenType.RIGHT_CURLY);
        else throw new Exception("Invalid character: " + curr);
    }

    /// <summary>
    /// Creates a token for arithmetic operators.
    /// </summary>
    /// <param name="curr">The current token string.</param>
    /// <returns>An operator token.</returns>
    /// <exception cref="Exception">Thrown when the character is not a valid operator.</exception>
    public static Token HandleOperator(string curr)
    {
        if (curr == "+") return new Token(TokenConstants.PLUS, TokenType.OPERATOR);
        else if (curr == "-") return new Token(TokenConstants.MINUS, TokenType.OPERATOR);
        else if (curr == "*") return new Token(TokenConstants.MULTIPLY, TokenType.OPERATOR);
        else if (curr == "/") return new Token(TokenConstants.DIVIDE, TokenType.OPERATOR);
        else if (curr == "//") return new Token(TokenConstants.INTEGERDIVIDE, TokenType.OPERATOR);
        else if (curr == "**") return new Token(TokenConstants.EXPONENTIATE, TokenType.OPERATOR);
        else throw new Exception("Invalid operator: " + curr);
    }
}