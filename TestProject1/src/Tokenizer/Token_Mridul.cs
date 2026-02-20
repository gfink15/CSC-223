
// "return" is the only keyword in this language
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Xunit.Sdk;

namespace Tokenizer_Mridul;

/// <summary>
/// Defines the types of tokens recognized by the tokenizer.
/// </summary>
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

/// <summary>
/// Constants for recognized token values and operators.
/// </summary>
public static class TokenConstants
{
    public const string LEFT_PAREN = "(";
    public const string RIGHT_PAREN = ")";
    public const string RETURN = "return";
    public const string PLUS = "+";
    public const string MINUS = "-";
    public const string MULTIPLY = "*";
    public const string DIVIDE = "/";
    public const string MODULUS = "%";
    public const string INTEGERDIVIDE = "//";
    public const string EXPONENTIATE = "**";
    public const string ASSIGNMENT = ":=";
    public const string DECIMAL_POINT = ".";
    public const string LEFT_CURLY = "{";
    public const string RIGHT_CURLY = "}";
}

/// <summary>
/// Represents a single token with a type and value.
/// </summary>
public class Token
{
    private readonly TokenType _type;
    private readonly string _value;

    /// <summary>
    /// Gets the type of this token.
    /// </summary>
    public TokenType Type
    {
        get { return _type;}
    }

    /// <summary>
    /// Gets the string value of this token.
    /// </summary>
    public string Value
    {
        get {return _value;}
    }

    /// <summary>
    /// Constructs a new token with the specified value and type.
    /// </summary>
    /// <param name="Value">The string value of the token.</param>
    /// <param name="Type">The type of the token.</param>
    public Token(string Value, TokenType Type)
    {
        _value = Value;
        _type = Type;
    }

    /// <summary>
    /// Returns a string representation of this token.
    /// </summary>
    /// <returns>String in format "Value : Type".</returns>
    public override string ToString()
    {
        return Value + " : " + Type.ToString();
    }

    /// <summary>
    /// Compares this token to another object for equality based on value and type.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if both tokens have the same value and type.</returns>
    /// <exception cref="ArgumentException">Thrown when obj is not a Token.</exception>
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


