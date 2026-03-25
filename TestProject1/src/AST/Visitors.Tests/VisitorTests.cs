// =============================================================================
// AST Visitor Test Suite — CSC-223 Assignment 6
// Covers: UnparseVisitor, EvaluateVisitor, NameAnalysisVisitor
// Each visitor has (1) direct/unit tests and (2) integration tests.
// =============================================================================

using Xunit;
using AST;          // adjust namespace to match your project
using Parser;       // adjust namespace to match your project

// =============================================================================
// UNPARSE VISITOR — Direct Tests
// Manually build AST nodes and verify the visitor produces correct strings.
// =============================================================================

public class UnparseVisitorTest
{
    private readonly UnparseVisitor _visitor = new UnparseVisitor();

    // -------------------------------------------------------------------------
    // Literal nodes
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-7)]
    public void LiteralInt_ReturnsCorrectString(int value)
    {
        var node = new LiteralNode(value);
        string result = node.Accept(_visitor, 0);
        Assert.Equal(value.ToString(), result);
    }

    [Theory]
    [InlineData(3.14)]
    [InlineData(0.0)]
    [InlineData(-1.5)]
    public void LiteralFloat_ReturnsCorrectString(double value)
    {
        var node = new LiteralNode(value);
        string result = node.Accept(_visitor, 0);
        Assert.Contains(value.ToString(), result);
    }

    // -------------------------------------------------------------------------
    // Variable nodes
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData("x")]
    [InlineData("myVar")]
    [InlineData("_count")]
    public void Variable_ReturnsName(string name)
    {
        var node = new VariableNode(name);
        string result = node.Accept(_visitor, 0);
        Assert.Equal(name, result);
    }

    // -------------------------------------------------------------------------
    // Binary expression nodes — parenthesization
    // -------------------------------------------------------------------------

    [Fact]
    public void PlusNode_IsParenthesized()
    {
        var node = new PlusNode(new LiteralNode(1), new LiteralNode(2));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(1 + 2)", result);
    }

    [Fact]
    public void MinusNode_IsParenthesized()
    {
        var node = new MinusNode(new LiteralNode(10), new LiteralNode(3));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(10 - 3)", result);
    }

    [Fact]
    public void TimesNode_IsParenthesized()
    {
        var node = new TimesNode(new LiteralNode(4), new LiteralNode(5));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(4 * 5)", result);
    }

    [Fact]
    public void FloatDivNode_IsParenthesized()
    {
        var node = new FloatDivNode(new LiteralNode(7), new LiteralNode(2));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(7 / 2)", result);
    }

    [Fact]
    public void IntDivNode_IsParenthesized()
    {
        var node = new IntDivNode(new LiteralNode(7), new LiteralNode(2));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(7 // 2)", result);
    }

    [Fact]
    public void ModulusNode_IsParenthesized()
    {
        var node = new ModulusNode(new LiteralNode(9), new LiteralNode(4));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(9 % 4)", result);
    }

    [Fact]
    public void ExponentiationNode_IsParenthesized()
    {
        var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(8));
        string result = node.Accept(_visitor, 0);
        Assert.Equal("(2 ** 8)", result);
    }

}