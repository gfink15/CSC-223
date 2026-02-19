using System;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace Tokenizer
{
    public enum TokenType
    {
        VARIABLE,
        RETURN,
        INTEGER,
        FLOAT,
        OPERATOR,
        ASSIGNMENT,
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_CURLY,
        RIGHT_CURLY
    }
    public static class TokenConstants
    {
        public const string ASSIGNMENT = ":=";
        public const string PLUS = "+";
        public const string MINUS = "-";
        public const string TIMES = "*";
        public const string FLOAT_DIVISION = "/";
        public const string INT_DIVISION = "//";
        public const string MODULUS = "%";
        public const string EXPONENT = "**";
        public const string LEFT_PAREN = "(";
        public const string RIGHT_PAREN = ")";
        public const string LEFT_CURLY = "{";

        public const string RIGHT_CURLY = "}";
        public const string DECIMAL_POINT = ".";
    }
    public class Token
    {
        private string _value;
        private TokenType _type;
        private int _line;
        private int _column;
        private int _len;
        public string Value
        {
            get { return _value; }
        }
        public TokenType Type
        {
            get { return _type; }
        }
        public int Line
        {
            get { return _line; }
        }
        public int Column
        {
            get { return _column; }
        }
        public int Length
        {
            get { return _len; }
        }
        public Token(string v, TokenType t, int line, int col, int len)
        {
            _value = v;
            _type = t;
            _line = line;
            _column = col;
            _len = len;
        }
        public override string ToString()
        {
            return "Value: " + _value + "Type: " + _type + "Line: " + _line + "Column: " + _column + "Length: " + _len;
        }
        public bool Equals(Token t)
        {
            if (_value == t.Value && _type == t.Type && _line == t.Length && _column == t.Column && _len == t.Length) return true;
            return false;
        }
    }
}