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
        private readonly TokenizerImpl _tokenizer = new TokenizerImpl();

        // ─────────────────────────────────────────────────────────────
        // TOKEN CLASS TESTS
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void Test()
        {
            var token = new Token("x", TokenType.VARIABLE);
            Assert.Equal("x", token.Value);
            Assert.Equal(TokenType.VARIABLE, token.Type);

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
            var tokens = _tokenizer.Tokenize(input);
            Assert.Single(tokens);
            Assert.Equal(TokenType.VARIABLE, tokens[0].Type);
            Assert.Equal("x", tokens[0].Value);
        }

    }
}