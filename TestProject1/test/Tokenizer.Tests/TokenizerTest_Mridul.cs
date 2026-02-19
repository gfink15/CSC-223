using Tokenizer_Mridul;
using Xunit.Abstractions;

namespace Tokenizer_Mridul.Tests;

public class TokenizerTest
{
    private readonly ITestOutputHelper _output;

    // xUnit injects this automatically
    public TokenizerTest(ITestOutputHelper output) {
        _output = output;
    }

    [Fact]
    public void TestTokenize()
    {
        string source_code = "x"; //:= 5 + 3.14 * (y - 2)
        List<Token> expectedTokens = new List<Token>
        {
            new Token("x", TokenType.VARIABLE),
            // new Token(":=", TokenType.ASSIGNMENT),
            // new Token("5", TokenType.INTEGER),
            // new Token("+", TokenType.OPERATOR),
            // new Token("3.14", TokenType.DOUBLE),
            // new Token("*", TokenType.OPERATOR),
            // new Token("(", TokenType.LEFT_PAREN),
            // new Token("y", TokenType.VARIABLE),
            // new Token("-", TokenType.OPERATOR),
            // new Token("2", TokenType.INTEGER),
            // new Token(")", TokenType.RIGHT_PAREN)
        };

        List<Token> actualTokens = TokenizerImpl.Tokenize(source_code);

        Assert.Equal(expectedTokens.Count, actualTokens.Count);
        for (int i = 0; i < expectedTokens.Count; i++)
        {
            Assert.Equal(expectedTokens[i].Type, actualTokens[i].Type);
            Assert.Equal(expectedTokens[i].Value, actualTokens[i].Value);
        }
    }

    public static IEnumerable<object[]> TokenizeData =>
    [
        // Single variable
        ["x", new List<Token> { new("x", TokenType.VARIABLE) }],
        // Variable followed by assignment
        ["y :=", new List<Token> { new("y", TokenType.VARIABLE), new(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT) }],
        // Full expression with curly braces
        ["{x := 5 + 3.14 * (y - 2)}", new List<Token> {
            new("{", TokenType.LEFT_CURLY),
            new("x", TokenType.VARIABLE),
            new(":=", TokenType.ASSIGNMENT),
            new("5", TokenType.INTEGER),
            new("+", TokenType.OPERATOR),
            new("3.14", TokenType.DOUBLE),
            new("*", TokenType.OPERATOR),
            new("(", TokenType.LEFT_PAREN),
            new("y", TokenType.VARIABLE),
            new("-", TokenType.OPERATOR),
            new("2", TokenType.INTEGER),
            new(")", TokenType.RIGHT_PAREN),
            new("}", TokenType.RIGHT_CURLY)
        }],
        // Single integer
        ["42", new List<Token> { new("42", TokenType.INTEGER) }],
        // Single double
        ["3.14", new List<Token> { new("3.14", TokenType.DOUBLE) }],
        // Return keyword
        ["return", new List<Token> { new(TokenConstants.RETURN, TokenType.RETURN) }],
        // Return in expression
        ["return x", new List<Token> { new(TokenConstants.RETURN, TokenType.RETURN), new("x", TokenType.VARIABLE) }],
        // All operators
        ["a + b", new List<Token> { new("a", TokenType.VARIABLE), new(TokenConstants.PLUS, TokenType.OPERATOR), new("b", TokenType.VARIABLE) }],
        ["a - b", new List<Token> { new("a", TokenType.VARIABLE), new(TokenConstants.MINUS, TokenType.OPERATOR), new("b", TokenType.VARIABLE) }],
        ["a * b", new List<Token> { new("a", TokenType.VARIABLE), new(TokenConstants.MULTIPLY, TokenType.OPERATOR), new("b", TokenType.VARIABLE) }],
        ["a / b", new List<Token> { new("a", TokenType.VARIABLE), new(TokenConstants.DIVIDE, TokenType.OPERATOR), new("b", TokenType.VARIABLE) }],
        // Parentheses
        ["(x)", new List<Token> { new("(", TokenType.LEFT_PAREN), new("x", TokenType.VARIABLE), new(")", TokenType.RIGHT_PAREN) }],
        // Curly braces alone
        ["{}", new List<Token> { new("{", TokenType.LEFT_CURLY), new("}", TokenType.RIGHT_CURLY) }],
        // Multi-digit integer
        ["123", new List<Token> { new("123", TokenType.INTEGER) }],
        // Multi-character variable
        ["myvar", new List<Token> { new("myvar", TokenType.VARIABLE) }],
        // Assignment operator alone
        [":=", new List<Token> { new(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT) }],
        // Empty string
        ["", new List<Token>()],
    ];

    [Theory]
    [MemberData(nameof(TokenizeData))]
    public void Tokenize_ReturnsCorrectTokens(string source_code, List<Token> expectedTokens)
    {
        List<Token> actualTokens = TokenizerImpl.Tokenize(source_code);

        _output.WriteLine($"Input: \"{source_code}\"");
        foreach (var token in actualTokens)
            _output.WriteLine("  " + token.ToString());

        Assert.Equal(expectedTokens.Count, actualTokens.Count);
        for (int i = 0; i < expectedTokens.Count; i++)
        {
            Assert.Equal(expectedTokens[i].Type, actualTokens[i].Type);
            Assert.Equal(expectedTokens[i].Value, actualTokens[i].Value);
        }
    }

    // --- Unit tests for individual handler methods ---

    [Fact]
    public void HandleIntegerOrDouble_Integer()
    {
        Token t = TokenizerImpl.HandleIntegerOrDouble("99");
        Assert.Equal(TokenType.INTEGER, t.Type);
        Assert.Equal("99", t.Value);
    }

    [Fact]
    public void HandleIntegerOrDouble_Double()
    {
        Token t = TokenizerImpl.HandleIntegerOrDouble("2.71");
        Assert.Equal(TokenType.DOUBLE, t.Type);
        Assert.Equal("2.71", t.Value);
    }

    [Fact]
    public void HandleString_Variable()
    {
        Token t = TokenizerImpl.HandleString("foo");
        Assert.Equal(TokenType.VARIABLE, t.Type);
        Assert.Equal("foo", t.Value);
    }

    [Fact]
    public void HandleString_ReturnKeyword()
    {
        Token t = TokenizerImpl.HandleString("return");
        Assert.Equal(TokenType.RETURN, t.Type);
        Assert.Equal(TokenConstants.RETURN, t.Value);
    }

    [Fact]
    public void HandleAssignment_ReturnsAssignmentToken()
    {
        Token t = TokenizerImpl.HandleAssignment(":=");
        Assert.Equal(TokenType.ASSIGNMENT, t.Type);
        Assert.Equal(TokenConstants.ASSIGNMENT, t.Value);
    }

    [Theory]
    [InlineData("(", TokenType.LEFT_PAREN)]
    [InlineData(")", TokenType.RIGHT_PAREN)]
    [InlineData("{", TokenType.LEFT_CURLY)]
    [InlineData("}", TokenType.RIGHT_CURLY)]
    public void HandleParensAndCurly_AllVariants(string input, TokenType expected)
    {
        Token t = TokenizerImpl.HandleParensAndCurly(input);
        Assert.Equal(expected, t.Type);
        Assert.Equal(input, t.Value);
    }

    [Fact]
    public void HandleParensAndCurly_InvalidThrows()
    {
        Assert.Throws<Exception>(() => TokenizerImpl.HandleParensAndCurly("x"));
    }

    [Theory]
    [InlineData("+", TokenType.OPERATOR)]
    [InlineData("-", TokenType.OPERATOR)]
    [InlineData("*", TokenType.OPERATOR)]
    [InlineData("/", TokenType.OPERATOR)]
    public void HandleOperator_AllVariants(string input, TokenType expected)
    {
        Token t = TokenizerImpl.HandleOperator(input);
        Assert.Equal(expected, t.Type);
    }

    [Fact]
    public void HandleOperator_InvalidThrows()
    {
        Assert.Throws<Exception>(() => TokenizerImpl.HandleOperator("^"));
    }
}