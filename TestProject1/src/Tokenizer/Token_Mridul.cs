
// "return" is the only keyword in this language
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Xunit.Sdk;

namespace Tokenizer_Mridul;

public enum TokenType
{
    VARIABLE,
    RETURN,
    INTEGER,
    DOUBLE,
    OPERATOR,
    ASSIGNMENT,
    LEFT_PAREN,
    RIGHT_PAREN,
    LEFT_CURLY,
    RIGHT_CURLY
}
public static class TokenConstants
{
    public const string LEFT_PAREN = "(";
    public const string RIGHT_PAREN = ")";
    public const string RETURN = "return";
    public const string PLUS = "+";
    public const string MINUS = "-";
    public const string MULTIPLY = "*";
    public const string DIVIDE = "/";
    public const string ASSIGNMENT = ":=";
    public const string DECIMAL_POINT = ".";
    public const string LEFT_CURLY = "{";
    public const string RIGHT_CURLY = "}";
}

public class Token
{
    private readonly TokenType _type;
    private readonly string _value;

    public TokenType Type
    {
        get { return _type;}
    }

    public string Value
    {
        get {return _value;}
    }

    public Token(string Value, TokenType Type)
    {
        _value = Value;
        _type = Type;
    }

    public override string ToString()
    {
        return Value + " : " + Type.ToString();
    }

    public override bool Equals(Object? obj)
    {
        if (obj is Token)
        {
            var other = (Token)obj;
            if (this.Value == other.Value && this.Type == other.Type) return true;
            return false;
        }
        throw new ArgumentException("Comparision not legal.");
    }
}


