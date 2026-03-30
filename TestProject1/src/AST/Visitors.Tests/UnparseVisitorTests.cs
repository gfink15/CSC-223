// =============================================================================
// AST Visitor Test Suite — CSC-223 Assignment 6
// Covers: UnparseVisitor
// Each section has (1) direct/unit tests and (2) integration tests.
// =============================================================================

using Xunit;
using AST;
using Utilities.Containers;

// =============================================================================
// UNPARSE VISITOR — Direct Tests
// Manually build AST nodes and verify the visitor produces correct strings.
// =============================================================================

public class UnparseVisitorTest
{
    private readonly UnparseVisitor _visitor = new UnparseVisitor();

    // =========================================================================
    //  SECTION 1 — Literal nodes
    // =========================================================================

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-7)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
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
    [InlineData(100.001)]
    public void LiteralFloat_ReturnsCorrectString(double value)
    {
        var node = new LiteralNode(value);
        string result = node.Accept(_visitor, 0);
        Assert.Contains(value.ToString(), result);
    }

    [Fact]
    public void LiteralString_ReturnsCorrectString()
    {
        var node = new LiteralNode("hello");
        string result = node.Accept(_visitor, 0);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void LiteralInt_IgnoresIndentationLevel()
    {
        var node = new LiteralNode(99);
        // Literal output should be the same regardless of indentation level
        Assert.Equal(node.Accept(_visitor, 0), node.Accept(_visitor, 3));
    }

    // =========================================================================
    //  SECTION 2 — Variable nodes
    // =========================================================================

    [Theory]
    [InlineData("x")]
    [InlineData("myVar")]
    [InlineData("_count")]
    [InlineData("a")]
    [InlineData("longvariablename")]
    public void Variable_ReturnsName(string name)
    {
        var node = new VariableNode(name);
        string result = node.Accept(_visitor, 0);
        Assert.Equal(name, result);
    }

    [Fact]
    public void Variable_IgnoresIndentationLevel()
    {
        var node = new VariableNode("foo");
        Assert.Equal(node.Accept(_visitor, 0), node.Accept(_visitor, 5));
    }

    // =========================================================================
    //  SECTION 3 — Binary expression nodes (parenthesization)
    // =========================================================================

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

    // =========================================================================
    //  SECTION 4 — Binary operators with variables
    // =========================================================================

    [Fact]
    public void PlusNode_WithVariables()
    {
        var node = new PlusNode(new VariableNode("x"), new VariableNode("y"));
        Assert.Equal("(x + y)", node.Accept(_visitor, 0));
    }

    [Fact]
    public void MinusNode_WithMixedOperands()
    {
        var node = new MinusNode(new VariableNode("total"), new LiteralNode(1));
        Assert.Equal("(total - 1)", node.Accept(_visitor, 0));
    }

    [Fact]
    public void TimesNode_WithVariableAndLiteral()
    {
        var node = new TimesNode(new LiteralNode(2), new VariableNode("pi"));
        Assert.Equal("(2 * pi)", node.Accept(_visitor, 0));
    }

    // =========================================================================
    //  SECTION 5 — Nested / deeply nested expressions
    // =========================================================================

    [Fact]
    public void NestedPlus_LeftAssociation()
    {
        // (1 + 2) + 3  →  ((1 + 2) + 3)
        var inner = new PlusNode(new LiteralNode(1), new LiteralNode(2));
        var outer = new PlusNode(inner, new LiteralNode(3));
        Assert.Equal("((1 + 2) + 3)", outer.Accept(_visitor, 0));
    }

    [Fact]
    public void NestedPlus_RightAssociation()
    {
        // 1 + (2 + 3)  →  (1 + (2 + 3))
        var inner = new PlusNode(new LiteralNode(2), new LiteralNode(3));
        var outer = new PlusNode(new LiteralNode(1), inner);
        Assert.Equal("(1 + (2 + 3))", outer.Accept(_visitor, 0));
    }

    [Fact]
    public void NestedMixedOperators()
    {
        // (x + 1) * (y - 2)  →  ((x + 1) * (y - 2))
        var left  = new PlusNode(new VariableNode("x"), new LiteralNode(1));
        var right = new MinusNode(new VariableNode("y"), new LiteralNode(2));
        var node  = new TimesNode(left, right);
        Assert.Equal("((x + 1) * (y - 2))", node.Accept(_visitor, 0));
    }

    [Fact]
    public void DeeplyNestedExpression_ThreeLevels()
    {
        // ((2 ** 3) + (4 * 5))  — two levels deep
        var pow  = new ExponentiationNode(new LiteralNode(2), new LiteralNode(3));
        var mul  = new TimesNode(new LiteralNode(4), new LiteralNode(5));
        var node = new PlusNode(pow, mul);
        Assert.Equal("((2 ** 3) + (4 * 5))", node.Accept(_visitor, 0));
    }

    [Fact]
    public void DeeplyNestedExpression_FourLevels()
    {
        // (((1 + 2) * 3) - (4 / 5))
        var add   = new PlusNode(new LiteralNode(1), new LiteralNode(2));
        var mul   = new TimesNode(add, new LiteralNode(3));
        var div   = new FloatDivNode(new LiteralNode(4), new LiteralNode(5));
        var node  = new MinusNode(mul, div);
        Assert.Equal("(((1 + 2) * 3) - (4 / 5))", node.Accept(_visitor, 0));
    }

    [Fact]
    public void ComplexExpression_AllOperators()
    {
        // ((a + b) % (c // d))
        var plus   = new PlusNode(new VariableNode("a"), new VariableNode("b"));
        var intdiv = new IntDivNode(new VariableNode("c"), new VariableNode("d"));
        var node   = new ModulusNode(plus, intdiv);
        Assert.Equal("((a + b) % (c // d))", node.Accept(_visitor, 0));
    }

    // =========================================================================
    //  SECTION 6 — AssignmentStmt tests
    // =========================================================================

    [Fact]
    public void Assignment_SimpleLiteral()
    {
        // x := 5
        var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(5));
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("x := 5", result);
    }

    [Fact]
    public void Assignment_WithExpression()
    {
        // y := (1 + 2)
        var expr = new PlusNode(new LiteralNode(1), new LiteralNode(2));
        var stmt = new AssignmentStmt(new VariableNode("y"), expr);
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("y := (1 + 2)", result);
    }

    [Fact]
    public void Assignment_WithVariable()
    {
        // a := b
        var stmt = new AssignmentStmt(new VariableNode("a"), new VariableNode("b"));
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("a := b", result);
    }

    [Fact]
    public void Assignment_WithNestedExpression()
    {
        // result := ((x + 1) * (y - 2))
        var left  = new PlusNode(new VariableNode("x"), new LiteralNode(1));
        var right = new MinusNode(new VariableNode("y"), new LiteralNode(2));
        var expr  = new TimesNode(left, right);
        var stmt  = new AssignmentStmt(new VariableNode("result"), expr);
        Assert.Equal("result := ((x + 1) * (y - 2))", stmt.Accept(_visitor, 0));
    }

    [Fact]
    public void Assignment_AtIndentLevel1()
    {
        // 4 spaces + x := 10
        var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(10));
        string result = stmt.Accept(_visitor, 1);
        Assert.Equal("    x := 10", result);
    }

    [Fact]
    public void Assignment_AtIndentLevel2()
    {
        // 8 spaces + val := 42
        var stmt = new AssignmentStmt(new VariableNode("val"), new LiteralNode(42));
        string result = stmt.Accept(_visitor, 2);
        Assert.Equal("        val := 42", result);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "    ")]
    [InlineData(2, "        ")]
    [InlineData(3, "            ")]
    public void Assignment_IndentationScales(int level, string expectedIndent)
    {
        var stmt = new AssignmentStmt(new VariableNode("n"), new LiteralNode(0));
        string result = stmt.Accept(_visitor, level);
        Assert.Equal(expectedIndent + "n := 0", result);
    }

    // =========================================================================
    //  SECTION 7 — ReturnStmt tests
    // =========================================================================

    [Fact]
    public void Return_SimpleLiteral()
    {
        // return 42
        var stmt = new ReturnStmt(new LiteralNode(42));
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("return 42", result);
    }

    [Fact]
    public void Return_WithVariable()
    {
        // return x
        var stmt = new ReturnStmt(new VariableNode("x"));
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("return x", result);
    }

    [Fact]
    public void Return_WithExpression()
    {
        // return (a + b)
        var expr = new PlusNode(new VariableNode("a"), new VariableNode("b"));
        var stmt = new ReturnStmt(expr);
        string result = stmt.Accept(_visitor, 0);
        Assert.Equal("return (a + b)", result);
    }

    [Fact]
    public void Return_WithNestedExpression()
    {
        // return ((x ** 2) + (y ** 2))
        var xSq  = new ExponentiationNode(new VariableNode("x"), new LiteralNode(2));
        var ySq  = new ExponentiationNode(new VariableNode("y"), new LiteralNode(2));
        var expr = new PlusNode(xSq, ySq);
        var stmt = new ReturnStmt(expr);
        Assert.Equal("return ((x ** 2) + (y ** 2))", stmt.Accept(_visitor, 0));
    }

    [Fact]
    public void Return_AtIndentLevel1()
    {
        var stmt = new ReturnStmt(new LiteralNode(0));
        string result = stmt.Accept(_visitor, 1);
        Assert.Equal("    return 0", result);
    }

    [Theory]
    [InlineData(0, "")]
    [InlineData(1, "    ")]
    [InlineData(2, "        ")]
    public void Return_IndentationScales(int level, string expectedIndent)
    {
        var stmt = new ReturnStmt(new LiteralNode(1));
        string result = stmt.Accept(_visitor, level);
        Assert.Equal(expectedIndent + "return 1", result);
    }

    // =========================================================================
    //  SECTION 8 — BlockStmt tests
    // =========================================================================

    [Fact]
    public void Block_Empty()
    {
        // {
        // }
        var block = new BlockStmt(new SymbolTable<string, object>());
        string result = block.Accept(_visitor, 0);
        Assert.Equal("{\n}", result);
    }

    [Fact]
    public void Block_SingleAssignment()
    {
        // {
        //     x := 5
        // }
        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(5)));
        string result = block.Accept(_visitor, 0);
        Assert.Equal("{\n    x := 5\n}", result);
    }

    [Fact]
    public void Block_SingleReturn()
    {
        // {
        //     return 0
        // }
        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new ReturnStmt(new LiteralNode(0)));
        string result = block.Accept(_visitor, 0);
        Assert.Equal("{\n    return 0\n}", result);
    }

    [Fact]
    public void Block_MultipleStatements()
    {
        // {
        //     x := 1
        //     y := 2
        //     return (x + y)
        // }
        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
        block.Add(new AssignmentStmt(new VariableNode("y"), new LiteralNode(2)));
        block.Add(new ReturnStmt(new PlusNode(new VariableNode("x"), new VariableNode("y"))));

        string expected = "{\n    x := 1\n    y := 2\n    return (x + y)\n}";
        Assert.Equal(expected, block.Accept(_visitor, 0));
    }

    [Fact]
    public void Block_NestedBlock()
    {
        // {
        //     {
        //         x := 1
        //     }
        // }
        var inner = new BlockStmt(new SymbolTable<string, object>());
        inner.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));

        var outer = new BlockStmt(new SymbolTable<string, object>());
        outer.Add(inner);

        string expected = "{\n    {\n        x := 1\n    }\n}";
        Assert.Equal(expected, outer.Accept(_visitor, 0));
    }

    [Fact]
    public void Block_DoubleNestedBlock()
    {
        // {
        //     {
        //         {
        //             return 99
        //         }
        //     }
        // }
        var innermost = new BlockStmt(new SymbolTable<string, object>());
        innermost.Add(new ReturnStmt(new LiteralNode(99)));

        var middle = new BlockStmt(new SymbolTable<string, object>());
        middle.Add(innermost);

        var outer = new BlockStmt(new SymbolTable<string, object>());
        outer.Add(middle);

        string expected =
            "{\n" +
            "    {\n" +
            "        {\n" +
            "            return 99\n" +
            "        }\n" +
            "    }\n" +
            "}";
        Assert.Equal(expected, outer.Accept(_visitor, 0));
    }

    [Fact]
    public void Block_AtIndentLevel1()
    {
        // Starting the block at level 1 means the braces get 4 spaces,
        // and inner statements get 8 spaces.
        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(7)));

        string expected = "    {\n        a := 7\n    }";
        Assert.Equal(expected, block.Accept(_visitor, 1));
    }

    [Fact]
    public void Block_MixedStatementsAndNestedBlock()
    {
        // {
        //     x := 10
        //     {
        //         y := (x + 1)
        //         return y
        //     }
        // }
        var inner = new BlockStmt(new SymbolTable<string, object>());
        inner.Add(new AssignmentStmt(
            new VariableNode("y"),
            new PlusNode(new VariableNode("x"), new LiteralNode(1))));
        inner.Add(new ReturnStmt(new VariableNode("y")));

        var outer = new BlockStmt(new SymbolTable<string, object>());
        outer.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(10)));
        outer.Add(inner);

        string expected =
            "{\n" +
            "    x := 10\n" +
            "    {\n" +
            "        y := (x + 1)\n" +
            "        return y\n" +
            "    }\n" +
            "}";
        Assert.Equal(expected, outer.Accept(_visitor, 0));
    }

    // =========================================================================
    //  SECTION 9 — Block with complex expressions
    // =========================================================================

    [Fact]
    public void Block_AssignmentWithComplexExpression()
    {
        // {
        //     result := ((a * b) + (c / d))
        // }
        var mul  = new TimesNode(new VariableNode("a"), new VariableNode("b"));
        var div  = new FloatDivNode(new VariableNode("c"), new VariableNode("d"));
        var expr = new PlusNode(mul, div);

        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new AssignmentStmt(new VariableNode("result"), expr));

        string expected = "{\n    result := ((a * b) + (c / d))\n}";
        Assert.Equal(expected, block.Accept(_visitor, 0));
    }

    [Fact]
    public void Block_ReturnWithModulusAndIntDiv()
    {
        // {
        //     return ((n % 2) // 1)
        // }
        var mod    = new ModulusNode(new VariableNode("n"), new LiteralNode(2));
        var intdiv = new IntDivNode(mod, new LiteralNode(1));

        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new ReturnStmt(intdiv));

        string expected = "{\n    return ((n % 2) // 1)\n}";
        Assert.Equal(expected, block.Accept(_visitor, 0));
    }

    // =========================================================================
    //  SECTION 10 — Multiple sequential assignments
    // =========================================================================

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Block_NAssignments_HasCorrectLineCount(int n)
    {
        var block = new BlockStmt(new SymbolTable<string, object>());
        for (int i = 0; i < n; i++)
        {
            block.Add(new AssignmentStmt(new VariableNode("v" + i), new LiteralNode(i)));
        }
        string result = block.Accept(_visitor, 0);

        // The output should have n+2 lines: opening brace + n statements + closing brace
        int lineCount = result.Split('\n').Length;
        Assert.Equal(n + 2, lineCount);
    }

    // =========================================================================
    //  SECTION 11 — Edge case: very deep nesting
    // =========================================================================

    [Fact]
    public void Block_ThreeLevelsDeep_IndentationCorrect()
    {
        // Build 3-deep nested blocks, each with one assignment
        var level3 = new BlockStmt(new SymbolTable<string, object>());
        level3.Add(new AssignmentStmt(new VariableNode("z"), new LiteralNode(3)));

        var level2 = new BlockStmt(new SymbolTable<string, object>());
        level2.Add(new AssignmentStmt(new VariableNode("y"), new LiteralNode(2)));
        level2.Add(level3);

        var level1 = new BlockStmt(new SymbolTable<string, object>());
        level1.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
        level1.Add(level2);

        string result = level1.Accept(_visitor, 0);

        // Verify key indentation properties
        Assert.StartsWith("{", result);
        Assert.EndsWith("}", result);
        Assert.Contains("    x := 1", result);          // level 1 indent
        Assert.Contains("        y := 2", result);      // level 2 indent
        Assert.Contains("            z := 3", result);  // level 3 indent
    }

    // =========================================================================
    //  SECTION 12 — Full-program integration test
    // =========================================================================

    [Fact]
    public void FullProgram_AssignComputeReturn()
    {
        // {
        //     a := 10
        //     b := 20
        //     sum := (a + b)
        //     return sum
        // }
        var block = new BlockStmt(new SymbolTable<string, object>());
        block.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(10)));
        block.Add(new AssignmentStmt(new VariableNode("b"), new LiteralNode(20)));
        block.Add(new AssignmentStmt(
            new VariableNode("sum"),
            new PlusNode(new VariableNode("a"), new VariableNode("b"))));
        block.Add(new ReturnStmt(new VariableNode("sum")));

        string expected =
            "{\n" +
            "    a := 10\n" +
            "    b := 20\n" +
            "    sum := (a + b)\n" +
            "    return sum\n" +
            "}";
        Assert.Equal(expected, block.Accept(_visitor, 0));
    }

    [Fact]
    public void FullProgram_NestedScopes()
    {
        // {
        //     x := 1
        //     {
        //         y := (x + 1)
        //         {
        //             z := (y * 2)
        //             return z
        //         }
        //     }
        // }
        var innermost = new BlockStmt(new SymbolTable<string, object>());
        innermost.Add(new AssignmentStmt(
            new VariableNode("z"),
            new TimesNode(new VariableNode("y"), new LiteralNode(2))));
        innermost.Add(new ReturnStmt(new VariableNode("z")));

        var middle = new BlockStmt(new SymbolTable<string, object>());
        middle.Add(new AssignmentStmt(
            new VariableNode("y"),
            new PlusNode(new VariableNode("x"), new LiteralNode(1))));
        middle.Add(innermost);

        var outer = new BlockStmt(new SymbolTable<string, object>());
        outer.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
        outer.Add(middle);

        string expected =
            "{\n" +
            "    x := 1\n" +
            "    {\n" +
            "        y := (x + 1)\n" +
            "        {\n" +
            "            z := (y * 2)\n" +
            "            return z\n" +
            "        }\n" +
            "    }\n" +
            "}";
        Assert.Equal(expected, outer.Accept(_visitor, 0));
    }
}