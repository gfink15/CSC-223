using Xunit;
using AST;
using Utilities.Containers;

namespace AST.Visitors.Tests;

// =============================================================================
// EvaluateVisitorTest
//
// Direct unit tests — construct AST nodes manually and invoke via Evaluate().
// All tests go through EvaluateVisitor.Evaluate(Statement), which is the
// public entry point. It resets visitor state and calls ast.Accept(this, null).
//
// BlockStmt construction (from AST.cs):
//   var block = new BlockStmt(new SymbolTable<string, object>());
//   block.Add(someStatement);
//
// AssignmentStmt construction (from AST.cs):
//   new AssignmentStmt(new VariableNode("x"), someExpression)
//
// EvaluateVisitor.Visit(BlockStmt) uses node.SymbolTable as the scope for that
// block. For scope-chaining tests the inner BlockStmt is given a child
// SymbolTable constructed with the outer table as its parent.
//
// Intended correct semantics tested here:
//   - Arithmetic on two ints should yield an int when the result is whole
//   - Arithmetic involving any double operand should yield a double
//   - IntDivNode truncates toward zero and should yield an int
//   - ReturnStmt immediately halts block execution; its expression is the result
//   - When no return is present, the block returns the value of its last statement
//   - Inner block scope does not bleed into the outer scope
//   - Evaluate() resets visitor state between calls
// =============================================================================

public class EvaluateVisitorTest
{
    private readonly EvaluateVisitor _visitor;

    public EvaluateVisitorTest()
    {
        _visitor = new EvaluateVisitor();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Builds a BlockStmt with its own fresh SymbolTable and adds each
    /// supplied statement in order. This is the single correct way to
    /// construct a BlockStmt per AST.cs (constructor takes a SymbolTable;
    /// statements are appended via Add()).
    /// </summary>
    private static BlockStmt MakeBlock(params Statement[] stmts)
    {
        var block = new BlockStmt(new SymbolTable<string, object>());
        foreach (var s in stmts)
            block.Add(s);
        return block;
    }

    /// <summary>
    /// Builds an inner BlockStmt whose SymbolTable is a child of the given
    /// parent table, so variables defined in the outer scope are visible inside.
    /// </summary>
    private static BlockStmt MakeInnerBlock(
        SymbolTable<string, object> parentTable, params Statement[] stmts)
    {
        var block = new BlockStmt(new SymbolTable<string, object>(parentTable));
        foreach (var s in stmts)
            block.Add(s);
        return block;
    }

    /// <summary>Shorthand: evaluate a top-level block built from the given statements.</summary>
    private object Eval(params Statement[] stmts)
        => _visitor.Evaluate(MakeBlock(stmts));

    // -----------------------------------------------------------------------
    // LiteralNode
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_LiteralInt_ReturnsCorrectValue()
    {
        var result = Eval(new ReturnStmt(new LiteralNode(7)));
        Assert.Equal(7, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_LiteralDouble_ReturnsCorrectValue()
    {
        var result = Eval(new ReturnStmt(new LiteralNode(3.14)));
        Assert.Equal(3.14, Convert.ToDouble(result), precision: 10);
    }

    [Fact]
    public void Visit_LiteralZero_ReturnsZero()
    {
        var result = Eval(new ReturnStmt(new LiteralNode(0)));
        Assert.Equal(0, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_LiteralNegativeInt_ReturnsCorrectValue()
    {
        var result = Eval(new ReturnStmt(new LiteralNode(-42)));
        Assert.Equal(-42, Convert.ToInt32(result));
    }

    // -----------------------------------------------------------------------
    // VariableNode — read after assignment
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_Variable_AfterAssignment_ReturnsStoredValue()
    {
        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(99)),
            new ReturnStmt(new VariableNode("x")));
        Assert.Equal(99, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Variable_AfterReassignment_ReturnsLatestValue()
    {
        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)),
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(2)),
            new ReturnStmt(new VariableNode("x")));
        Assert.Equal(2, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Variable_StoredDoubleValue_ReturnsDouble()
    {
        var result = Eval(
            new AssignmentStmt(new VariableNode("pi"), new LiteralNode(3.14)),
            new ReturnStmt(new VariableNode("pi")));
        Assert.Equal(3.14, Convert.ToDouble(result), precision: 10);
    }

    // -----------------------------------------------------------------------
    // PlusNode
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(3, 4, 7)]
    [InlineData(0, 0, 0)]
    [InlineData(-5, 5, 0)]
    [InlineData(100, -1, 99)]
    public void Visit_Plus_IntPlusInt_ReturnsCorrectSum(int a, int b, int expected)
    {
        var result = Eval(new ReturnStmt(
            new PlusNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Plus_IntPlusDouble_ReturnsCorrectSum()
    {
        var result = Eval(new ReturnStmt(
            new PlusNode(new LiteralNode(1), new LiteralNode(0.5))));
        Assert.Equal(1.5, Convert.ToDouble(result), precision: 10);
    }

    [Fact]
    public void Visit_Plus_DoublePlusDouble_ReturnsCorrectSum()
    {
        var result = Eval(new ReturnStmt(
            new PlusNode(new LiteralNode(1.5), new LiteralNode(2.5))));
        Assert.Equal(4.0, Convert.ToDouble(result), precision: 10);
    }

    // -----------------------------------------------------------------------
    // MinusNode
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(0, 0, 0)]
    [InlineData(-3, -7, 4)]
    [InlineData(5, 10, -5)]
    public void Visit_Minus_IntMinusInt_ReturnsCorrectDifference(int a, int b, int expected)
    {
        var result = Eval(new ReturnStmt(
            new MinusNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Minus_DoubleMinusDouble_ReturnsCorrectDifference()
    {
        var result = Eval(new ReturnStmt(
            new MinusNode(new LiteralNode(5.5), new LiteralNode(2.5))));
        Assert.Equal(3.0, Convert.ToDouble(result), precision: 10);
    }

    // -----------------------------------------------------------------------
    // TimesNode
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(3, 4, 12)]
    [InlineData(0, 100, 0)]
    [InlineData(-3, 4, -12)]
    [InlineData(-3, -4, 12)]
    public void Visit_Times_IntTimesInt_ReturnsCorrectProduct(int a, int b, int expected)
    {
        var result = Eval(new ReturnStmt(
            new TimesNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Times_DoubleTimesDouble_ReturnsCorrectProduct()
    {
        var result = Eval(new ReturnStmt(
            new TimesNode(new LiteralNode(2.5), new LiteralNode(4.0))));
        Assert.Equal(10.0, Convert.ToDouble(result), precision: 10);
    }

    [Fact]
    public void Visit_Times_IntTimesDouble_ReturnsCorrectProduct()
    {
        var result = Eval(new ReturnStmt(
            new TimesNode(new LiteralNode(3), new LiteralNode(0.5))));
        Assert.Equal(1.5, Convert.ToDouble(result), precision: 10);
    }

    // -----------------------------------------------------------------------
    // FloatDivNode — always produces a double; throws on zero denominator
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(10, 4, 2.5)]
    [InlineData(7, 2, 3.5)]
    [InlineData(9, 3, 3.0)]
    [InlineData(1, 4, 0.25)]
    public void Visit_FloatDiv_ReturnsCorrectQuotient(int a, int b, double expected)
    {
        var result = Eval(new ReturnStmt(
            new FloatDivNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToDouble(result), precision: 10);
    }

    [Fact]
    public void Visit_FloatDiv_ByZero_ThrowsEvaluationException()
    {
        var block = MakeBlock(new ReturnStmt(
            new FloatDivNode(new LiteralNode(5), new LiteralNode(0))));
        var ex = Assert.Throws<EvaluationException>(() => _visitor.Evaluate(block));
        Assert.Equal("Can't FloatDiv by 0", ex.Message);
    }

    [Fact]
    public void Visit_FloatDiv_ByZeroDouble_ThrowsEvaluationException()
    {
        var block = MakeBlock(new ReturnStmt(
            new FloatDivNode(new LiteralNode(5.0), new LiteralNode(0.0))));
        Assert.Throws<EvaluationException>(() => _visitor.Evaluate(block));
    }

    // -----------------------------------------------------------------------
    // IntDivNode — truncates toward zero; throws on zero denominator
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 3)]
    [InlineData(7, 2, 3)]
    [InlineData(9, 3, 3)]
    [InlineData(0, 5, 0)]
    [InlineData(-7, 2, -3)]
    [InlineData(-9, 3, -3)]
    public void Visit_IntDiv_ReturnsCorrectTruncatedQuotient(int a, int b, int expected)
    {
        var result = Eval(new ReturnStmt(
            new IntDivNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_IntDiv_ByZero_ThrowsEvaluationException()
    {
        var block = MakeBlock(new ReturnStmt(
            new IntDivNode(new LiteralNode(10), new LiteralNode(0))));
        var ex = Assert.Throws<EvaluationException>(() => _visitor.Evaluate(block));
        Assert.Equal("Can't IntDiv by 0", ex.Message);
    }

    // -----------------------------------------------------------------------
    // ModulusNode — correct remainder; throws on zero denominator
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 1)]
    [InlineData(9, 3, 0)]
    [InlineData(0, 5, 0)]
    [InlineData(-10, 3, -1)]
    public void Visit_Modulus_ReturnsCorrectRemainder(int a, int b, int expected)
    {
        var result = Eval(new ReturnStmt(
            new ModulusNode(new LiteralNode(a), new LiteralNode(b))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Modulus_ByZero_ThrowsEvaluationException()
    {
        var block = MakeBlock(new ReturnStmt(
            new ModulusNode(new LiteralNode(10), new LiteralNode(0))));
        var ex = Assert.Throws<EvaluationException>(() => _visitor.Evaluate(block));
        Assert.Equal("Can't Mod by 0", ex.Message);
    }

    // -----------------------------------------------------------------------
    // ExponentiationNode
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(2, 10, 1024)]
    [InlineData(5, 0, 1)]
    [InlineData(3, 3, 27)]
    [InlineData(1, 100, 1)]
    public void Visit_Exponentiation_WholeResult_ReturnsCorrectValue(int b, int exp, int expected)
    {
        var result = Eval(new ReturnStmt(
            new ExponentiationNode(new LiteralNode(b), new LiteralNode(exp))));
        Assert.Equal(expected, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Exponentiation_NegativeExponent_ReturnsCorrectDouble()
    {
        // 2 ^ -1 = 0.5
        var result = Eval(new ReturnStmt(
            new ExponentiationNode(new LiteralNode(2), new LiteralNode(-1))));
        Assert.Equal(0.5, Convert.ToDouble(result), precision: 10);
    }

    [Fact]
    public void Visit_Exponentiation_FractionalExponent_ReturnsCorrectDouble()
    {
        // 4 ^ 0.5 = 2.0
        var result = Eval(new ReturnStmt(
            new ExponentiationNode(new LiteralNode(4), new LiteralNode(0.5))));
        Assert.Equal(2.0, Convert.ToDouble(result), precision: 10);
    }

    // -----------------------------------------------------------------------
    // AssignmentStmt
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_Assignment_StoresValueReadableByLaterStatement()
    {
        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(42)),
            new ReturnStmt(new VariableNode("x")));
        Assert.Equal(42, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Assignment_ExpressionIsFullyEvaluatedBeforeStoring()
    {
        // z := (3 + 4) → z should equal 7
        var result = Eval(
            new AssignmentStmt(new VariableNode("z"),
                new PlusNode(new LiteralNode(3), new LiteralNode(4))),
            new ReturnStmt(new VariableNode("z")));
        Assert.Equal(7, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Assignment_CanReferenceEarlierVariableOnRhs()
    {
        // x := 5; y := (x + 1) → y should equal 6
        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(5)),
            new AssignmentStmt(new VariableNode("y"),
                new PlusNode(new VariableNode("x"), new LiteralNode(1))),
            new ReturnStmt(new VariableNode("y")));
        Assert.Equal(6, Convert.ToInt32(result));
    }

    // -----------------------------------------------------------------------
    // ReturnStmt — immediately halts block execution
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_Return_HaltsExecutionOfSubsequentStatements()
    {
        // The second return must never be reached
        var result = Eval(
            new ReturnStmt(new LiteralNode(1)),
            new ReturnStmt(new LiteralNode(999)));
        Assert.Equal(1, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Return_ExpressionIsEvaluatedCorrectly()
    {
        // return (3 * 4) → 12
        var result = Eval(new ReturnStmt(
            new TimesNode(new LiteralNode(3), new LiteralNode(4))));
        Assert.Equal(12, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Return_ValueIsBlockResult()
    {
        var result = Eval(new ReturnStmt(new LiteralNode(55)));
        Assert.Equal(55, Convert.ToInt32(result));
    }

    // -----------------------------------------------------------------------
    // BlockStmt — last-statement semantics, scope, nested return propagation
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_Block_NoReturn_YieldsValueOfLastStatement()
    {
        // Last executed statement is x := 7, so the block result should be 7
        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(3)),
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(7)));
        Assert.Equal(7, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_Block_ReturnInsideNestedBlock_PropagatesOutward()
    {
        // Build the outer block's symbol table first so we can chain the inner one to it
        var outerTable = new SymbolTable<string, object>();
        var outerBlock = new BlockStmt(outerTable);

        var innerBlock = new BlockStmt(new SymbolTable<string, object>(outerTable));
        innerBlock.Add(new ReturnStmt(new LiteralNode(77)));

        outerBlock.Add(innerBlock);
        outerBlock.Add(new ReturnStmt(new LiteralNode(0))); // must not be reached

        Assert.Equal(77, Convert.ToInt32(_visitor.Evaluate(outerBlock)));
    }

    [Fact]
    public void Visit_Block_InnerScopeCanReadOuterVariable()
    {
        // x is pre-populated in the outer table so the inner block can read it
        var outerTable = new SymbolTable<string, object>();
        var outerBlock = new BlockStmt(outerTable);
        outerBlock.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(7)));

        // Inner block chains to outerTable so x is visible
        var innerBlock = MakeInnerBlock(outerTable,
            new ReturnStmt(new VariableNode("x")));
        outerBlock.Add(innerBlock);

        Assert.Equal(7, Convert.ToInt32(_visitor.Evaluate(outerBlock)));
    }

    [Fact]
    public void Visit_Block_InnerScopeAssignmentDoesNotAffectOuterVariable()
    {
        // Outer x = 1; inner block shadows x with 99; after inner block, outer x should still be 1
        var outerTable = new SymbolTable<string, object>();
        var outerBlock = new BlockStmt(outerTable);
        outerBlock.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));

        var innerBlock = MakeInnerBlock(outerTable,
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(99)));
        outerBlock.Add(innerBlock);
        outerBlock.Add(new ReturnStmt(new VariableNode("x")));

        Assert.Equal(1, Convert.ToInt32(_visitor.Evaluate(outerBlock)));
    }

    [Fact]
    public void Visit_Block_VisitorStateIsResetBetweenEvaluateCalls()
    {
        // First call returns 42; second call must not carry over _returnEncountered
        _visitor.Evaluate(MakeBlock(new ReturnStmt(new LiteralNode(42))));

        var result = Eval(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(10)),
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(20)));
        Assert.Equal(20, Convert.ToInt32(result));
    }

    // -----------------------------------------------------------------------
    // Compound / integration-style AST compositions
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_NestedArithmetic_ReturnsCorrectResult()
    {
        // ((2 + 3) * (4 - 1)) = 15
        var expr = new TimesNode(
            new PlusNode(new LiteralNode(2), new LiteralNode(3)),
            new MinusNode(new LiteralNode(4), new LiteralNode(1)));
        Assert.Equal(15, Convert.ToInt32(Eval(new ReturnStmt(expr))));
    }

    [Fact]
    public void Visit_MultiVariableExpression_ReturnsCorrectResult()
    {
        // a := 3; b := 4; return (a + b) → 7
        var result = Eval(
            new AssignmentStmt(new VariableNode("a"), new LiteralNode(3)),
            new AssignmentStmt(new VariableNode("b"), new LiteralNode(4)),
            new ReturnStmt(new PlusNode(new VariableNode("a"), new VariableNode("b"))));
        Assert.Equal(7, Convert.ToInt32(result));
    }

    [Fact]
    public void Visit_DivisionByZeroInsideNestedExpression_ThrowsEvaluationException()
    {
        // (10 + (5 // 0)) — exception must propagate out through the wrapping PlusNode
        var block = MakeBlock(new ReturnStmt(
            new PlusNode(
                new LiteralNode(10),
                new IntDivNode(new LiteralNode(5), new LiteralNode(0)))));
        Assert.Throws<EvaluationException>(() => _visitor.Evaluate(block));
    }

    [Fact]
    public void Visit_ThreeLevelNestedBlocks_InnermostReturnPropagatesAllTheWayOut()
    {
        var outerTable  = new SymbolTable<string, object>();
        var middleTable = new SymbolTable<string, object>(outerTable);
        var innerTable  = new SymbolTable<string, object>(middleTable);

        var innermost = new BlockStmt(innerTable);
        innermost.Add(new ReturnStmt(new LiteralNode(42)));

        var middle = new BlockStmt(middleTable);
        middle.Add(innermost);
        middle.Add(new ReturnStmt(new LiteralNode(0))); // must not be reached

        var outer = new BlockStmt(outerTable);
        outer.Add(middle);
        outer.Add(new ReturnStmt(new LiteralNode(0))); // must not be reached

        Assert.Equal(42, Convert.ToInt32(_visitor.Evaluate(outer)));
    }
}