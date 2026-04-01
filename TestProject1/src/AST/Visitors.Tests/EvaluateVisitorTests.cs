using Xunit;
using AST;
using AST.Visitors;
using Utilities.Containers;

namespace AST.Visitors.Tests;

/// <summary>
/// Direct unit tests for EvaluateVisitor — each test manually constructs AST nodes
/// and calls Accept to verify a single Visit method in isolation.
/// </summary>
public class EvaluateVisitorTest
{
    private readonly EvaluateVisitor _visitor;
    private readonly SymbolTable<string, object> _table;

    public EvaluateVisitorTest()
    {
        _visitor = new EvaluateVisitor();
        _table = new SymbolTable<string, object>();
    }

    // -------------------------------------------------------------------------
    // LiteralNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-42)]
    [InlineData(int.MaxValue)]
    public void Visit_LiteralNode_IntReturnsInt(int value)
    {
        var node = new LiteralNode(value);
        var result = node.Accept(_visitor, _table);
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(3.14)]
    [InlineData(-2.718)]
    public void Visit_LiteralNode_DoubleReturnsDouble(double value)
    {
        var node = new LiteralNode(value);
        var result = node.Accept(_visitor, _table);
        Assert.Equal(value, result);
    }

    // -------------------------------------------------------------------------
    // VariableNode
    // -------------------------------------------------------------------------

    [Fact]
    public void Visit_VariableNode_ReturnsStoredValue()
    {
        _table["x"] = 99;
        var node = new VariableNode("x");
        var result = node.Accept(_visitor, _table);
        Assert.Equal(99, result);
    }

    [Fact]
    public void Visit_VariableNode_UndefinedThrows()
    {
        var node = new VariableNode("undefined");
        Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
    }

    // -------------------------------------------------------------------------
    // PlusNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(3, 4, 7)]
    [InlineData(0, 0, 0)]
    [InlineData(-5, 5, 0)]
    [InlineData(int.MaxValue - 1, 1, int.MaxValue)]
    public void Visit_PlusNode_IntPlusInt(int left, int right, int expected)
    {
        var node = new PlusNode(new LiteralNode(left), new LiteralNode(right));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_PlusNode_IntPlusDouble_ReturnsDouble()
    {
        var node = new PlusNode(new LiteralNode(1), new LiteralNode(1.5));
        Assert.Equal(2.5, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_PlusNode_DoublePlusDouble_ReturnsDouble()
    {
        var node = new PlusNode(new LiteralNode(1.1), new LiteralNode(2.2));
        var result = (double)node.Accept(_visitor, _table);
        Assert.InRange(result, 3.29, 3.31);
    }

    // -------------------------------------------------------------------------
    // MinusNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(0, 0, 0)]
    [InlineData(-3, -7, 4)]
    public void Visit_MinusNode_IntMinusInt(int left, int right, int expected)
    {
        var node = new MinusNode(new LiteralNode(left), new LiteralNode(right));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_MinusNode_DoubleMinusDouble()
    {
        var node = new MinusNode(new LiteralNode(5.5), new LiteralNode(2.5));
        Assert.Equal(3.0, node.Accept(_visitor, _table));
    }

    // -------------------------------------------------------------------------
    // TimesNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(3, 4, 12)]
    [InlineData(0, 100, 0)]
    [InlineData(-3, 4, -12)]
    [InlineData(-3, -4, 12)]
    public void Visit_TimesNode_IntTimesInt(int left, int right, int expected)
    {
        var node = new TimesNode(new LiteralNode(left), new LiteralNode(right));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_TimesNode_DoubleTimesDouble()
    {
        var node = new TimesNode(new LiteralNode(2.5), new LiteralNode(4.0));
        Assert.Equal(10.0, node.Accept(_visitor, _table));
    }

    // -------------------------------------------------------------------------
    // FloatDivNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(10, 4, 2.5)]
    [InlineData(7, 2, 3.5)]
    [InlineData(1, 3)]          // result checked separately (infinite precision)
    public void Visit_FloatDivNode_ReturnsDouble(int left, int right, double? expected = null)
    {
        var node = new FloatDivNode(new LiteralNode(left), new LiteralNode(right));
        var result = node.Accept(_visitor, _table);
        Assert.IsType<double>(result);
        if (expected.HasValue)
            Assert.Equal(expected.Value, (double)result, precision: 10);
    }

    [Fact]
    public void Visit_FloatDivNode_ByZeroThrows()
    {
        var node = new FloatDivNode(new LiteralNode(5), new LiteralNode(0));
        var ex = Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
        Assert.Equal("Division by zero", ex.Message);
    }

    [Fact]
    public void Visit_FloatDivNode_ByZeroDouble_Throws()
    {
        var node = new FloatDivNode(new LiteralNode(5.0), new LiteralNode(0.0));
        Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
    }

    // -------------------------------------------------------------------------
    // IntDivNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 3)]
    [InlineData(7, 2, 3)]
    [InlineData(-7, 2, -3)]
    [InlineData(9, 3, 3)]
    public void Visit_IntDivNode_Truncates(int left, int right, int expected)
    {
        var node = new IntDivNode(new LiteralNode(left), new LiteralNode(right));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_IntDivNode_ByZeroThrows()
    {
        var node = new IntDivNode(new LiteralNode(10), new LiteralNode(0));
        var ex = Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
        Assert.Equal("Division by zero", ex.Message);
    }

    // -------------------------------------------------------------------------
    // ModulusNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(10, 3, 1)]
    [InlineData(9, 3, 0)]
    [InlineData(-10, 3, -1)]
    [InlineData(0, 5, 0)]
    public void Visit_ModulusNode_IntMod(int left, int right, int expected)
    {
        var node = new ModulusNode(new LiteralNode(left), new LiteralNode(right));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_ModulusNode_ByZeroThrows()
    {
        var node = new ModulusNode(new LiteralNode(10), new LiteralNode(0));
        Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
    }

    // -------------------------------------------------------------------------
    // ExponentiationNode
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(2, 10, 1024)]
    [InlineData(5, 0, 1)]
    [InlineData(3, 3, 27)]
    [InlineData(1, 100, 1)]
    public void Visit_ExponentiationNode_IntPow(int baseVal, int exp, int expected)
    {
        var node = new ExponentiationNode(new LiteralNode(baseVal), new LiteralNode(exp));
        Assert.Equal(expected, node.Accept(_visitor, _table));
    }

    [Fact]
    public void Visit_ExponentiationNode_NegativeExponent_ReturnsDouble()
    {
        var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(-1));
        var result = node.Accept(_visitor, _table);
        Assert.IsType<double>(result);
        Assert.Equal(0.5, (double)result, precision: 10);
    }

    [Fact]
    public void Visit_ExponentiationNode_DoubleBase()
    {
        var node = new ExponentiationNode(new LiteralNode(4.0), new LiteralNode(0.5));
        var result = (double)node.Accept(_visitor, _table);
        Assert.InRange(result, 1.99, 2.01);
    }

    // -------------------------------------------------------------------------
    // AssignmentStmt
    // -------------------------------------------------------------------------

    [Fact]
    public void Visit_AssignmentStmt_StoresValueInTable()
    {
        var stmt = new AssignmentStmt(new VariableNode("y"), new LiteralNode(42));
        stmt.Accept(_visitor, _table);
        Assert.Equal(42, _table["y"]);
    }

    [Fact]
    public void Visit_AssignmentStmt_OverwritesExistingValue()
    {
        _table["z"] = 1;
        var stmt = new AssignmentStmt(new VariableNode("z"), new LiteralNode(999));
        stmt.Accept(_visitor, _table);
        Assert.Equal(999, _table["z"]);
    }

    [Fact]
    public void Visit_AssignmentStmt_ExpressionIsEvaluated()
    {
        // z := 3 + 4  =>  z == 7
        var stmt = new AssignmentStmt(new VariableNode("z"),
            new PlusNode(new LiteralNode(3), new LiteralNode(4)));
        stmt.Accept(_visitor, _table);
        Assert.Equal(7, _table["z"]);
    }

    // -------------------------------------------------------------------------
    // ReturnStmt
    // -------------------------------------------------------------------------

    [Fact]
    public void Visit_ReturnStmt_ReturnsCorrectValue()
    {
        var stmt = new ReturnStmt(new LiteralNode(55));
        var result = stmt.Accept(_visitor, _table);
        Assert.Equal(55, result);
    }

    // [Fact]
    // public void Visit_ReturnStmt_HaltsBlockExecution()
    // {
    //     // Only the return should run; the subsequent assignment must not execute.
    //     var block = new BlockStmt(new SymbolTable<>
    //     {
    //         new ReturnStmt(new LiteralNode(1)),
    //         new AssignmentStmt(new VariableNode("sideEffect"), new LiteralNode(99))
    //     });
    //     var result = block.Accept(_visitor, _table);
    //     Assert.Equal(1, result);
    //     Assert.False(_table.ContainsKey("sideEffect"));
    // }

    // -------------------------------------------------------------------------
    // BlockStmt — scope and last-statement semantics
    // -------------------------------------------------------------------------

    // [Fact]
    // public void Visit_BlockStmt_ReturnsLastStatementValue()
    // {
    //     var block = new BlockStmt(new List<Statement>
    //     {
    //         new AssignmentStmt("a", new LiteralNode(10)),
    //         new AssignmentStmt("b", new LiteralNode(20))
    //     });
    //     var result = block.Accept(_visitor, _table);
    //     Assert.Equal(20, result);
    // }

    // [Fact]
    // public void Visit_BlockStmt_VariablesShadowedInInnerScope()
    // {
    //     // Outer: x = 1. Inner block: x = 2. After inner block, outer x should still be 1.
    //     _table["x"] = 1;
    //     var inner = new BlockStmt(new List<Statement>
    //     {
    //         new AssignmentStmt("x", new LiteralNode(2))
    //     });
    //     var outer = new BlockStmt(new List<Statement> { inner });
    //     outer.Accept(_visitor, _table);
    //     Assert.Equal(1, _table["x"]);
    // }

    // [Fact]
    // public void Visit_BlockStmt_EmptyBlockReturnsNull()
    // {
    //     var block = new BlockStmt(new List<Statement>());
    //     var result = block.Accept(_visitor, _table);
    //     Assert.Null(result);
    // }

    // [Fact]
    // public void Visit_BlockStmt_ReturnPropagatesThroughNesting()
    // {
    //     var inner = new BlockStmt(new List<Statement>
    //     {
    //         new ReturnStmt(new LiteralNode(77))
    //     });
    //     var outer = new BlockStmt(new List<Statement>
    //     {
    //         inner,
    //         new AssignmentStmt("neverRuns", new LiteralNode(0))
    //     });
    //     var result = outer.Accept(_visitor, _table);
    //     Assert.Equal(77, result);
    //     Assert.False(_table.ContainsKey("neverRuns"));
    // }
}

/// <summary>
/// Integration tests for EvaluateVisitor — parses a complete DEC source string
/// and evaluates the resulting BlockStmt AST.
/// </summary>
public class EvaluateTest
{
    private readonly EvaluateVisitor _evaluator;

    public EvaluateTest()
    {
        _evaluator = new EvaluateVisitor();
    }

    // -------------------------------------------------------------------------
    // Arithmetic
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_SimpleReturn()
    {
        string program = @"{
            return (2 + 3)
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(5, _evaluator.Evaluate(ast));
    }

    [Theory]
    [InlineData("(10 + 5)", 15)]
    [InlineData("(10 - 5)", 5)]
    [InlineData("(10 * 5)", 50)]
    [InlineData("(10 // 3)", 3)]
    [InlineData("(10 % 3)", 1)]
    public void Evaluate_BasicArithmeticOps(string expr, int expected)
    {
        string program = $"{{ return {expr} }}";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(expected, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_FloatDivision_ReturnsDouble()
    {
        string program = @"{ return (7 / 2) }";
        BlockStmt ast = Parser.Parser.Parse(program);
        var result = _evaluator.Evaluate(ast);
        Assert.Equal(3.5, result);
    }

    [Fact]
    public void Evaluate_NestedArithmetic()
    {
        // ((2 + 3) * (4 - 1)) = 15
        string program = @"{ return ((2 + 3) * (4 - 1)) }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(15, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_Exponentiation()
    {
        string program = @"{ return (2 ^ 8) }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(256, _evaluator.Evaluate(ast));
    }

    // -------------------------------------------------------------------------
    // Variables
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_AssignAndReturn()
    {
        string program = @"{
            x := 10
            return x
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(10, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_MultipleAssignments()
    {
        string program = @"{
            x := 3
            y := 4
            return (x + y)
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(7, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_ReassignVariable()
    {
        string program = @"{
            x := 1
            x := 2
            return x
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(2, _evaluator.Evaluate(ast));
    }

    // -------------------------------------------------------------------------
    // Scope
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_InnerScopeDoesNotLeakVariable()
    {
        string program = @"{
            x := 5
            {
                y := 10
            }
            return x
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(5, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_InnerScopeCanReadOuterVariable()
    {
        string program = @"{
            x := 7
            {
                return x
            }
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(7, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_ShadowedVariableDoesNotAffectOuter()
    {
        string program = @"{
            x := 1
            {
                x := 99
            }
            return x
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(1, _evaluator.Evaluate(ast));
    }

    // -------------------------------------------------------------------------
    // Return semantics
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_ReturnStopsExecution()
    {
        string program = @"{
            return 42
            x := 999
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(42, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_NoReturnYieldsLastStatementValue()
    {
        string program = @"{
            x := 3
            y := 7
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(7, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_ReturnInsideNestedBlock_PropagatesOut()
    {
        string program = @"{
            x := 1
            {
                return 55
            }
            x := 2
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(55, _evaluator.Evaluate(ast));
    }

    // -------------------------------------------------------------------------
    // Division by zero
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_IntDivisionByZero_Throws()
    {
        string program = @"{
            return (10 // 0)
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        var ex = Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
        Assert.Equal("Division by zero", ex.Message);
    }

    [Fact]
    public void Evaluate_FloatDivisionByZero_Throws()
    {
        string program = @"{
            return (10 / 0)
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_ModByZero_Throws()
    {
        string program = @"{
            return (5 % 0)
        }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
    }

    // -------------------------------------------------------------------------
    // Mixed int / double promotion
    // -------------------------------------------------------------------------

    [Fact]
    public void Evaluate_IntPlusDouble_ReturnsDouble()
    {
        string program = @"{ return (1 + 1.5) }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(2.5, _evaluator.Evaluate(ast));
    }

    [Fact]
    public void Evaluate_ComplexMixedExpression()
    {
        // (3 * 2.0) - (10 // 4) = 6.0 - 2 = 4.0
        string program = @"{ return ((3 * 2.0) - (10 // 4)) }";
        BlockStmt ast = Parser.Parser.Parse(program);
        Assert.Equal(4.0, _evaluator.Evaluate(ast));
    }
}