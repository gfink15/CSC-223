using System;
using System.Collections.Generic;
using Xunit;
using Tokenizer;

namespace Tokenizer.Tests
{
    /// <summary>
    /// Comprehensive xUnit test suite for the DEC language TokenizerImpl.
    /// Covers all token types, edge cases, error handling, and combinations.
    /// </summary>
    public class TokenizerManualTest
    {
        // ─────────────────────────────────────────────────────────────
        // TOKEN CLASS TESTS
        // ─────────────────────────────────────────────────────────────

        #region Token Class Tests

        [Fact]
        public void Token_Constructor_SetsValueAndType()
        {
            var token = new Token("x", TokenType.VARIABLE);
            Assert.Equal("x", token.Value);
            Assert.Equal(TokenType.VARIABLE, token.Type);
        }

        [Fact]
        public void Token_Equals_SameValueAndType_ReturnsTrue()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            var t2 = new Token("x", TokenType.VARIABLE);
            Assert.Equal(t1, t2);
        }

        [Fact]
        public void Token_Equals_DifferentValue_ReturnsFalse()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            var t2 = new Token("y", TokenType.VARIABLE);
            Assert.NotEqual(t1, t2);
        }

        [Fact]
        public void Token_Equals_DifferentType_ReturnsFalse()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            var t2 = new Token("x", TokenType.RETURN);
            Assert.NotEqual(t1, t2);
        }

        [Fact]
        public void Token_ToString_ContainsValueAndType()
        {
            var token = new Token("x", TokenType.VARIABLE);
            string result = token.ToString();
            Assert.Contains("x", result);
            Assert.Contains("VARIABLE", result);
        }

        [Fact]
        public void Token_Equals_Null_ReturnsFalse()
        {
            var token = new Token("x", TokenType.VARIABLE);
            Assert.Throws<ArgumentException>(() => token.Equals(null));
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // TOKEN CONSTANTS TESTS
        // ─────────────────────────────────────────────────────────────

        #region TokenConstants Tests

        [Fact]
        public void TokenConstants_Plus_IsCorrect()
        {
            Assert.Equal("+", TokenConstants.PLUS);
        }

        [Fact]
        public void TokenConstants_LeftParen_IsCorrect()
        {
            Assert.Equal("(", TokenConstants.LEFT_PAREN);
        }

        [Fact]
        public void TokenConstants_LeftCurly_IsCorrect()
        {
            Assert.Equal("{", TokenConstants.LEFT_CURLY);
        }

        [Fact]
        public void TokenConstants_Assignment_IsCorrect()
        {
            Assert.Equal(":=", TokenConstants.ASSIGNMENT);
        }

        [Fact]
        public void TokenConstants_DecimalPoint_IsCorrect()
        {
            Assert.Equal(".", TokenConstants.DECIMAL_POINT);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // WHITESPACE / EMPTY INPUT
        // ─────────────────────────────────────────────────────────────

        #region Whitespace and Empty Input

        [Fact]
        public void Tokenize_EmptyString_ReturnsEmptyList()
        {
            var tokens = TokenizerImpl.Tokenize("");
            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_OnlyWhitespace_ReturnsEmptyList()
        {
            var tokens = TokenizerImpl.Tokenize("   \t\n\r  ");
            Assert.Empty(tokens);
        }

        [Theory]
        [InlineData("x")]
        [InlineData(" x")]
        [InlineData("x ")]
        [InlineData("  x  ")]
        [InlineData("\tx\t")]
        [InlineData("\nx\n")]
        public void Tokenize_VariableWithSurroundingWhitespace_ReturnsSingleVariableToken(string input)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal("x", tokens[0].Value);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // VARIABLES
        // ─────────────────────────────────────────────────────────────

        #region Variable Tokens

        [Theory]
        [InlineData("x", "x")]
        [InlineData("abc", "abc")]
        [InlineData("myvar", "myvar")]
        [InlineData("z", "z")]
        [InlineData("longvariablename", "longvariablename")]
        public void Tokenize_LowercaseVariable_ReturnsVariableToken(string input, string expectedValue)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal(expectedValue, tokens[0].Value);
        }

        [Fact]
        public void Tokenize_MultipleVariables_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("x y z");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t => Assert.Equal(TokenType.VARIABLE, t.Type));
            Assert.Equal("x", tokens[0].Value);
            Assert.Equal("y", tokens[1].Value);
            Assert.Equal("z", tokens[2].Value);
        }

        [Fact]
        public void Tokenize_SingleLetterVariables_AllRecognized()
        {
            // All 26 letters should be valid variables
            for (char c = 'a'; c <= 'z'; c++)
            {
                var tokens = TokenizerImpl.Tokenize(c.ToString());
                Assert.Single(tokens);
                Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
                Assert.Equal(c.ToString(), tokens[0].Value);
            }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // KEYWORD: return
        // ─────────────────────────────────────────────────────────────

        #region Keyword Tokens

        [Fact]
        public void Tokenize_ReturnKeyword_ReturnsReturnToken()
        {
            var tokens = TokenizerImpl.Tokenize("return");
            Assert.Single(tokens);
            Assert.Equal(TokenType.RETURN, tokens[0].Type);
            Assert.Equal("return", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_ReturnNotAKeyword_WhenPartOfLargerWord()
        {
            // "returns" is a variable, not the return keyword
            var tokens = TokenizerImpl.Tokenize("returns");
            Assert.Single(tokens);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_ReturnFollowedByVariable_ReturnsBothTokens()
        {
            var tokens = TokenizerImpl.Tokenize("return x");
            Assert.Equal(2, tokens.Count);
            Assert.Equal(TokenType.RETURN, tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_ReturnInExpression_RecognizedAsKeyword()
        {
            var tokens = TokenizerImpl.Tokenize("return x + y");
            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.RETURN, tokens[0].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // INTEGER LITERALS
        // ─────────────────────────────────────────────────────────────

        #region Integer Tokens

        [Theory]
        [InlineData("0", "0")]
        [InlineData("1", "1")]
        [InlineData("42", "42")]
        [InlineData("100", "100")]
        [InlineData("999999", "999999")]
        public void Tokenize_Integer_ReturnsIntegerToken(string input, string expectedValue)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(TokenType.INTEGER, tokens[0].Type);
            Assert.Equal(expectedValue, tokens[0].Value);
        }

        [Fact]
        public void Tokenize_MultipleIntegers_ReturnsAllIntegerTokens()
        {
            var tokens = TokenizerImpl.Tokenize("1 2 3");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t => Assert.Equal(TokenType.INTEGER, t.Type));
        }

        [Fact]
        public void Tokenize_LargeInteger_ReturnsCorrectToken()
        {
            var tokens = TokenizerImpl.Tokenize("1234567890");
            Assert.Single(tokens);
            Assert.Equal(TokenType.INTEGER, tokens[0].Type);
            Assert.Equal("1234567890", tokens[0].Value);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // FLOAT LITERALS
        // ─────────────────────────────────────────────────────────────

        #region Float Tokens

        [Theory]
        [InlineData("1.0", "1.0")]
        [InlineData("3.14", "3.14")]
        [InlineData("0.5", "0.5")]
        [InlineData("100.001", "100.001")]
        [InlineData("99.99", "99.99")]
        public void Tokenize_Float_ReturnsFloatToken(string input, string expectedValue)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(TokenType.DOUBLE, tokens[0].Type);
            Assert.Equal(expectedValue, tokens[0].Value);
        }

        [Fact]
        public void Tokenize_FloatVsInteger_DistinguishedCorrectly()
        {
            var intTokens = TokenizerImpl.Tokenize("5");
            var floatTokens = TokenizerImpl.Tokenize("5.0");

            Assert.Equal(TokenType.INTEGER, intTokens[0].Type);
            Assert.Equal(TokenType.DOUBLE, floatTokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultipleFloats_ReturnsAllFloatTokens()
        {
            var tokens = TokenizerImpl.Tokenize("1.1 2.2 3.3");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t => Assert.Equal(TokenType.DOUBLE, t.Type));
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // ASSIGNMENT OPERATOR
        // ─────────────────────────────────────────────────────────────

        #region Assignment Operator

        [Fact]
        public void Tokenize_AssignmentOperator_ReturnsAssignmentToken()
        {
            var tokens = TokenizerImpl.Tokenize(":=");
            Assert.Single(tokens);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[0].Type);
            Assert.Equal(":=", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_AssignmentWithSpaces_ReturnsAssignmentToken()
        {
            var tokens = TokenizerImpl.Tokenize(" := ");
            Assert.Single(tokens);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_ColonWithoutEquals_ThrowsArgumentException2()
        {
            // A lone colon is not valid in DEC
            Assert.Throws<ArgumentException>(() => TokenizerImpl.Tokenize(":"));
        }

        [Fact]
        public void Tokenize_AssignmentStatement_ReturnsCorrectSequence()
        {
            var tokens = TokenizerImpl.Tokenize("x := 5");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[1].Type);
            Assert.Equal(TokenType.INTEGER, tokens[2].Type);
        }

        [Fact]
        public void Tokenize_FloatAssignment_ReturnsCorrectSequence()
        {
            var tokens = TokenizerImpl.Tokenize("x := 3.14");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[1].Type);
            Assert.Equal(TokenType.DOUBLE, tokens[2].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // ARITHMETIC OPERATORS
        // ─────────────────────────────────────────────────────────────

        #region Arithmetic Operator Tokens

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        public void Tokenize_BasicArithmeticOperator_ReturnsOperatorToken(string op)
        {
            var tokens = TokenizerImpl.Tokenize(op);
            Assert.Single(tokens);
            Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            Assert.Equal(op, tokens[0].Value);
        }

        [Fact]
        public void Tokenize_FloatDivision_ReturnsOperatorToken()
        {
            // Single slash = float division
            var tokens = TokenizerImpl.Tokenize("/");
            Assert.Single(tokens);
            Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            Assert.Equal("/", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_IntegerDivision_ReturnsOperatorToken()
        {
            // Double slash = integer division
            var tokens = TokenizerImpl.Tokenize("//");
            Assert.Single(tokens);
            Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            Assert.Equal("//", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_Modulus_ReturnsOperatorToken()
        {
            var tokens = TokenizerImpl.Tokenize("%");
            Assert.Single(tokens);
            Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            Assert.Equal("%", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_Exponentiation_ReturnsOperatorToken()
        {
            var tokens = TokenizerImpl.Tokenize("**");
            Assert.Single(tokens);
            Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            Assert.Equal("**", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_FloatDivisionVsIntegerDivision_DistinguishedCorrectly()
        {
            var floatDiv = TokenizerImpl.Tokenize("/");
            var intDiv = TokenizerImpl.Tokenize("//");

            Assert.Equal("/", floatDiv[0].Value);
            Assert.Equal("//", intDiv[0].Value);
        }

        [Fact]
        public void Tokenize_AllOperators_AllHaveOperatorType()
        {
            string[] operators = { "+", "-", "*", "/", "//", "%", "**" };
            foreach (var op in operators)
            {
                var tokens = TokenizerImpl.Tokenize(op);
                Assert.Single(tokens);
                Assert.Equal(TokenType.OPERATOR, tokens[0].Type);
            }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // PARENTHESES
        // ─────────────────────────────────────────────────────────────

        #region Parenthesis Tokens

        [Fact]
        public void Tokenize_LeftParen_ReturnsLeftParenToken()
        {
            var tokens = TokenizerImpl.Tokenize("(");
            Assert.Single(tokens);
            Assert.Equal(TokenType.LEFT_PAREN, tokens[0].Type);
            Assert.Equal("(", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_RightParen_ReturnsRightParenToken()
        {
            var tokens = TokenizerImpl.Tokenize(")");
            Assert.Single(tokens);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[0].Type);
            Assert.Equal(")", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_MatchedParens_ReturnsBothTokens()
        {
            var tokens = TokenizerImpl.Tokenize("()");
            Assert.Equal(2, tokens.Count);
            Assert.Equal(TokenType.LEFT_PAREN, tokens[0].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_NestedParens_ReturnsAllTokens()
        {
            var tokens = TokenizerImpl.Tokenize("((()))");
            Assert.Equal(6, tokens.Count);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[0].Type);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[1].Type);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[2].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[3].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[4].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[5].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // CURLY BRACES
        // ─────────────────────────────────────────────────────────────

        #region Curly Brace Tokens

        [Fact]
        public void Tokenize_LeftCurly_ReturnsLeftCurlyToken()
        {
            var tokens = TokenizerImpl.Tokenize("{");
            Assert.Single(tokens);
            Assert.Equal(TokenType.LEFT_CURLY, tokens[0].Type);
            Assert.Equal("{", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_RightCurly_ReturnsRightCurlyToken()
        {
            var tokens = TokenizerImpl.Tokenize("}");
            Assert.Single(tokens);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[0].Type);
            Assert.Equal("}", tokens[0].Value);
        }

        [Fact]
        public void Tokenize_MatchedCurlies_ReturnsBothTokens()
        {
            var tokens = TokenizerImpl.Tokenize("{}");
            Assert.Equal(2, tokens.Count);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[0].Type);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_NestedCurlies_ReturnsAllTokens()
        {
            var tokens = TokenizerImpl.Tokenize("{{}}");
            Assert.Equal(4, tokens.Count);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[0].Type);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[1].Type);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[2].Type);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[3].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // COMPLEX EXPRESSIONS
        // ─────────────────────────────────────────────────────────────

        #region Complex Expression Tests

        [Fact]
        public void Tokenize_ExampleFromSpec_xAssignParenOnePlusTwoRightParen()
        {
            // From the assignment PDF: x := (1 + 2)
            var tokens = TokenizerImpl.Tokenize("x := (1 + 2)");
            Assert.Equal(7, tokens.Count);
            Assert.Equal(new Token("x",  TokenType.VARIABLE),   tokens[0]);
            Assert.Equal(new Token(":=", TokenType.ASSIGNMENT), tokens[1]);
            Assert.Equal(new Token("(",  TokenType.LEFT_PAREN), tokens[2]);
            Assert.Equal(new Token("1",  TokenType.INTEGER),    tokens[3]);
            Assert.Equal(new Token("+",  TokenType.OPERATOR),   tokens[4]);
            Assert.Equal(new Token("2",  TokenType.INTEGER),    tokens[5]);
            Assert.Equal(new Token(")",  TokenType.RIGHT_PAREN),tokens[6]);
        }

        [Fact]
        public void Tokenize_NestedArithmeticExpression_ReturnsCorrectTokens()
        {
            // (a + b) * (c - d)
            var tokens = TokenizerImpl.Tokenize("(a + b) * (c - d)");
            Assert.Equal(11, tokens.Count);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[1].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[2].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[3].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[4].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[5].Type);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[6].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[7].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[8].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[9].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[10].Type);
        }

        [Fact]
        public void Tokenize_FloatArithmetic_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("x := 1.5 + 2.5");
            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.VARIABLE,   tokens[0].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[1].Type);
            Assert.Equal(TokenType.DOUBLE,      tokens[2].Type);
            Assert.Equal(TokenType.OPERATOR,   tokens[3].Type);
            Assert.Equal(TokenType.DOUBLE,      tokens[4].Type);
        }

        [Fact]
        public void Tokenize_IntegerDivisionExpression_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("a // b");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal(TokenType.OPERATOR, tokens[1].Type);
            Assert.Equal("//",              tokens[1].Value);
            Assert.Equal(TokenType.VARIABLE, tokens[2].Type);
        }

        [Fact]
        public void Tokenize_ModulusExpression_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("a % b");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.OPERATOR, tokens[1].Type);
            Assert.Equal("%", tokens[1].Value);
        }

        [Fact]
        public void Tokenize_ExponentiationExpression_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("x ** 2");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.OPERATOR, tokens[1].Type);
            Assert.Equal("**", tokens[1].Value);
        }

        [Fact]
        public void Tokenize_ReturnStatement_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("return x");
            Assert.Equal(2, tokens.Count);
            Assert.Equal(TokenType.RETURN,   tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_ScopedBlock_ReturnsCorrectTokens()
        {
            // { x := 1 }
            var tokens = TokenizerImpl.Tokenize("{ x := 1 }");
            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[1].Type);
            Assert.Equal(TokenType.ASSIGNMENT,  tokens[2].Type);
            Assert.Equal(TokenType.INTEGER,     tokens[3].Type);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_ComplexProgram_ReturnsAllTokensInOrder()
        {
            string program = "{ x := (a + b) // 2 return x }";
            var tokens = TokenizerImpl.Tokenize(program);

            Assert.Equal(13, tokens.Count);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[1].Type);
            Assert.Equal(TokenType.ASSIGNMENT,  tokens[2].Type);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[3].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[4].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[5].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[6].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[7].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[8].Type);
            Assert.Equal("//",                  tokens[8].Value);
            Assert.Equal(TokenType.INTEGER,     tokens[9].Type);
            Assert.Equal(TokenType.RETURN,      tokens[10].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[11].Type);
            Assert.Equal(TokenType.RIGHT_CURLY,    tokens[12].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // NO SPACES BETWEEN TOKENS
        // ─────────────────────────────────────────────────────────────

        #region No-Whitespace Tokenization

        [Fact]
        public void Tokenize_NoSpaces_ParenExpression_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("(1+2)");
            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.LEFT_PAREN,  tokens[0].Type);
            Assert.Equal(TokenType.INTEGER,     tokens[1].Type);
            Assert.Equal(TokenType.OPERATOR,    tokens[2].Type);
            Assert.Equal(TokenType.INTEGER,     tokens[3].Type);
            Assert.Equal(TokenType.RIGHT_PAREN, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_NoSpaces_AssignmentExpression_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("x:=    5");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.VARIABLE,   tokens[0].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[1].Type);
            Assert.Equal(TokenType.INTEGER,    tokens[2].Type);
        }

        [Fact]
        public void Tokenize_NoSpaces_CurlyWithContent_ReturnsCorrectTokens()
        {
            var tokens = TokenizerImpl.Tokenize("{x:=1}");
            Assert.Equal(5, tokens.Count);
            Assert.Equal(TokenType.LEFT_CURLY,  tokens[0].Type);
            Assert.Equal(TokenType.VARIABLE,    tokens[1].Type);
            Assert.Equal(TokenType.ASSIGNMENT,  tokens[2].Type);
            Assert.Equal(TokenType.INTEGER,     tokens[3].Type);
            Assert.Equal(TokenType.RIGHT_CURLY, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_NoSpaces_IntegerDivision_ReturnsCorrectTokens()
        {
            // a//b — must not be confused with two separate slashes
            var tokens = TokenizerImpl.Tokenize("a//b");
            Assert.Equal(3, tokens.Count);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal(TokenType.OPERATOR, tokens[1].Type);
            Assert.Equal("//",              tokens[1].Value);
            Assert.Equal(TokenType.VARIABLE, tokens[2].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // MULTILINE INPUT
        // ─────────────────────────────────────────────────────────────

        #region Multiline Input

        [Fact]
        public void Tokenize_MultilineInput_ReturnsAllTokens()
        {
            string input = "x := 1\ny := 2\nreturn x";
            var tokens = TokenizerImpl.Tokenize(input);

            Assert.Equal(8, tokens.Count);
            Assert.Equal(TokenType.VARIABLE,   tokens[0].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[1].Type);
            Assert.Equal(TokenType.INTEGER,    tokens[2].Type);
            Assert.Equal(TokenType.VARIABLE,   tokens[3].Type);
            Assert.Equal(TokenType.ASSIGNMENT, tokens[4].Type);
            Assert.Equal(TokenType.INTEGER,    tokens[5].Type);
            Assert.Equal(TokenType.RETURN,     tokens[6].Type);
            Assert.Equal(TokenType.VARIABLE,   tokens[7].Type);
        }

        [Fact]
        public void Tokenize_WindowsLineEndings_ReturnsAllTokens()
        {
            string input = "x := 1\r\ny := 2";
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Equal(6, tokens.Count);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // ERROR / INVALID INPUT
        // ─────────────────────────────────────────────────────────────

        #region Error and Invalid Input

        [Theory]
        [InlineData("@")]
        [InlineData("#")]
        [InlineData("$")]
        [InlineData("!")]
        [InlineData("&")]
        [InlineData("~")]
        [InlineData("`")]
        [InlineData("?")]
        public void Tokenize_InvalidCharacter_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => TokenizerImpl.Tokenize(input));
        }

        [Fact]
        public void Tokenize_ColonWithoutEquals_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => TokenizerImpl.Tokenize(": "));
        }

        [Fact]
        public void Tokenize_FloatWithLeadingDot_ThrowsArgumentException()
        {
            // ".5" without a leading digit is invalid (only positive floats allowed)
            Assert.Throws<ArgumentException>(() => TokenizerImpl.Tokenize(".5"));
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // TOKEN COUNT / COVERAGE
        // ─────────────────────────────────────────────────────────────

        #region Token Type Coverage

        [Fact]
        public void Tokenize_AllTokenTypesCovered_EachTypeCanBeProduced()
        {
            // VARIABLE
            Assert.Equal(TokenType.VARIABLE, TokenizerImpl.Tokenize("x")[0].Type);
            // RETURN
            Assert.Equal(TokenType.RETURN, TokenizerImpl.Tokenize("return")[0].Type);
            // INTEGER
            Assert.Equal(TokenType.INTEGER, TokenizerImpl.Tokenize("42")[0].Type);
            // FLOAT
            Assert.Equal(TokenType.DOUBLE, TokenizerImpl.Tokenize("3.14")[0].Type);
            // OPERATOR (plus)
            Assert.Equal(TokenType.OPERATOR, TokenizerImpl.Tokenize("+")[0].Type);
            // ASSIGNMENT
            Assert.Equal(TokenType.ASSIGNMENT, TokenizerImpl.Tokenize(":=")[0].Type);
            // LEFT_PAREN
            Assert.Equal(TokenType.LEFT_PAREN, TokenizerImpl.Tokenize("(")[0].Type);
            // RIGHT_PAREN
            Assert.Equal(TokenType.RIGHT_PAREN, TokenizerImpl.Tokenize(")")[0].Type);
            // LEFT_CURLY
            Assert.Equal(TokenType.LEFT_CURLY, TokenizerImpl.Tokenize("{")[0].Type);
            // RIGHT_CURLY
            Assert.Equal(TokenType.RIGHT_CURLY, TokenizerImpl.Tokenize("}")[0].Type);
        }

        [Theory]
        [InlineData("+",  TokenType.OPERATOR)]
        [InlineData("-",  TokenType.OPERATOR)]
        [InlineData("*",  TokenType.OPERATOR)]
        [InlineData("/",  TokenType.OPERATOR)]
        [InlineData("//", TokenType.OPERATOR)]
        [InlineData("%",  TokenType.OPERATOR)]
        [InlineData("**",  TokenType.OPERATOR)]
        public void Tokenize_EachArithmeticOperator_ReturnsOperatorType(string op, TokenType expected)
        {
            var tokens = TokenizerImpl.Tokenize(op);
            Assert.Single(tokens);
            Assert.Equal(expected, tokens[0].Type);
            Assert.Equal(op, tokens[0].Value);
        }

        [Theory]
        [InlineData("(", TokenType.LEFT_PAREN)]
        [InlineData(")", TokenType.RIGHT_PAREN)]
        [InlineData("{", TokenType.LEFT_CURLY)]
        [InlineData("}", TokenType.RIGHT_CURLY)]
        public void Tokenize_StructuralTokens_ReturnsCorrectType(string input, TokenType expected)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(expected, tokens[0].Type);
        }

        [Theory]
        [InlineData("0",       TokenType.INTEGER)]
        [InlineData("1",       TokenType.INTEGER)]
        [InlineData("999",     TokenType.INTEGER)]
        [InlineData("0.0",     TokenType.DOUBLE)]
        [InlineData("1.5",     TokenType.DOUBLE)]
        [InlineData("123.456", TokenType.DOUBLE)]
        public void Tokenize_NumericLiterals_ReturnsCorrectType(string input, TokenType expected)
        {
            var tokens = TokenizerImpl.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(expected, tokens[0].Type);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // RETURN TYPE AND COUNT SANITY
        // ─────────────────────────────────────────────────────────────

        #region Return Type Sanity

        [Fact]
        public void Tokenize_AlwaysReturnsList_NotNull()
        {
            var tokens = TokenizerImpl.Tokenize("x := 1");
            Assert.NotNull(tokens);
            Assert.IsType<List<Token>>(tokens);
        }

        [Fact]
        public void Tokenize_LongExpression_CountMatchesExpected()
        {
            // x := ( a + b ) * ( c - d ) // 2
            string input = "x := ( a + b ) * ( c - d ) // 2";
            var tokens = TokenizerImpl.Tokenize(input);
            // x  :=  (  a  +  b  )  *  (  c  -  d  )  //  2  => 15 tokens
            Assert.Equal(15, tokens.Count);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // DUPLICATE / REPEATED TOKENS
        // ─────────────────────────────────────────────────────────────

        #region Duplicate Token Tests

        [Fact]
        public void Tokenize_RepeatedVariable_ReturnsDuplicateTokens()
        {
            var tokens = TokenizerImpl.Tokenize("x x x");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t =>
            {
                Assert.Equal(TokenType.VARIABLE, t.Type);
                Assert.Equal("x", t.Value);
            });
        }

        [Fact]
        public void Tokenize_RepeatedIntegers_ReturnsDuplicateTokens()
        {
            var tokens = TokenizerImpl.Tokenize("1 1 1");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t =>
            {
                Assert.Equal(TokenType.INTEGER, t.Type);
                Assert.Equal("1", t.Value);
            });
        }

        [Fact]
        public void Tokenize_RepeatedOperators_ReturnsDuplicateTokens()
        {
            var tokens = TokenizerImpl.Tokenize("+ + +");
            Assert.Equal(3, tokens.Count);
            Assert.All(tokens, t => Assert.Equal(TokenType.OPERATOR, t.Type));
        }

        [Fact]
        public void Tokenize_RepeatedReturn_ReturnsMultipleReturnTokens()
        {
            var tokens = TokenizerImpl.Tokenize("return return");
            Assert.Equal(2, tokens.Count);
            Assert.All(tokens, t => Assert.Equal(TokenType.RETURN, t.Type));
        }

        #endregion
    }
}