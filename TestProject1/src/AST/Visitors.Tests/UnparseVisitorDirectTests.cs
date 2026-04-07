// =============================================================================
// UnparseVisitor — Direct Unit Tests
// Manually constructs AST nodes and calls Accept to verify each Visit method
// produces the correct string output in isolation.
//
// Bugs: None known.
//
// @author Graham Fink, Mridul Agrawal
// @date   4/2/2026
// =============================================================================

using Xunit;
using AST;
using Utilities.Containers;

namespace AST.Visitors.Tests
{
    public class UnparseVisitorDirectTests
    {
        private readonly UnparseVisitor _visitor = new UnparseVisitor();

        // =====================================================================
        //  Literal Node Tests
        // =====================================================================

        #region Literal Node Tests

        [Theory]
        [InlineData(0, "0")]
        [InlineData(1, "1")]
        [InlineData(-1, "-1")]
        [InlineData(42, "42")]
        [InlineData(int.MaxValue, "2147483647")]
        [InlineData(int.MinValue, "-2147483648")]
        public void Literal_IntValues_ReturnCorrectString(int value, string expected)
        {
            var node = new LiteralNode(value);
            Assert.Equal(expected, node.Accept(_visitor, 0));
        }

        [Theory]
        [InlineData(3.14)]
        [InlineData(0.0)]
        [InlineData(-1.5)]
        [InlineData(100.001)]
        [InlineData(0.1)]
        public void Literal_DoubleValues_ReturnCorrectString(double value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value.ToString(), node.Accept(_visitor, 0));
        }

        [Fact]
        public void Literal_String_ReturnsExactString()
        {
            var node = new LiteralNode("hello world");
            Assert.Equal("hello world", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Literal_IgnoresIndentationLevel()
        {
            var node = new LiteralNode(99);
            string atZero = node.Accept(_visitor, 0);
            string atThree = node.Accept(_visitor, 3);
            Assert.Equal(atZero, atThree);
        }

        [Fact]
        public void Literal_Zero_ReturnsZeroString()
        {
            var node = new LiteralNode(0);
            Assert.Equal("0", node.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  Variable Node Tests
        // =====================================================================

        #region Variable Node Tests

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("_count")]
        [InlineData("abc123")]
        [InlineData("longVariableNameWithManyChars")]
        public void Variable_ReturnsName(string name)
        {
            var node = new VariableNode(name);
            Assert.Equal(name, node.Accept(_visitor, 0));
        }

        [Fact]
        public void Variable_IgnoresIndentationLevel()
        {
            var node = new VariableNode("foo");
            Assert.Equal(node.Accept(_visitor, 0), node.Accept(_visitor, 5));
        }

        [Fact]
        public void Variable_SingleCharName()
        {
            var node = new VariableNode("a");
            Assert.Equal("a", node.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  Binary Operator — Parenthesization (all 7 operators)
        // =====================================================================

        #region Binary Operator Parenthesization Tests

        [Fact]
        public void Plus_LiteralsOnly_CorrectFormat()
        {
            var node = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            Assert.Equal("(1 + 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Minus_LiteralsOnly_CorrectFormat()
        {
            var node = new MinusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.Equal("(10 - 3)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Times_LiteralsOnly_CorrectFormat()
        {
            var node = new TimesNode(new LiteralNode(4), new LiteralNode(5));
            Assert.Equal("(4 * 5)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void FloatDiv_LiteralsOnly_CorrectFormat()
        {
            var node = new FloatDivNode(new LiteralNode(7), new LiteralNode(2));
            Assert.Equal("(7 / 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void IntDiv_LiteralsOnly_CorrectFormat()
        {
            var node = new IntDivNode(new LiteralNode(7), new LiteralNode(2));
            Assert.Equal("(7 // 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Modulus_LiteralsOnly_CorrectFormat()
        {
            var node = new ModulusNode(new LiteralNode(9), new LiteralNode(4));
            Assert.Equal("(9 % 4)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Exponentiation_LiteralsOnly_CorrectFormat()
        {
            var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(8));
            Assert.Equal("(2 ** 8)", node.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  Binary Operators with Variables and Mixed Operands
        // =====================================================================

        #region Binary Operators with Variables

        [Fact]
        public void Plus_WithVariables()
        {
            var node = new PlusNode(new VariableNode("x"), new VariableNode("y"));
            Assert.Equal("(x + y)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Minus_VariableAndLiteral()
        {
            var node = new MinusNode(new VariableNode("total"), new LiteralNode(1));
            Assert.Equal("(total - 1)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Times_LiteralAndVariable()
        {
            var node = new TimesNode(new LiteralNode(2), new VariableNode("pi"));
            Assert.Equal("(2 * pi)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void FloatDiv_VariableByLiteral()
        {
            var node = new FloatDivNode(new VariableNode("sum"), new LiteralNode(10));
            Assert.Equal("(sum / 10)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void IntDiv_VariableByVariable()
        {
            var node = new IntDivNode(new VariableNode("a"), new VariableNode("b"));
            Assert.Equal("(a // b)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Modulus_VariableByLiteral()
        {
            var node = new ModulusNode(new VariableNode("n"), new LiteralNode(2));
            Assert.Equal("(n % 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Exponentiation_VariableByVariable()
        {
            var node = new ExponentiationNode(new VariableNode("base"), new VariableNode("exp"));
            Assert.Equal("(base ** exp)", node.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  Nested Expressions
        // =====================================================================

        #region Nested Expressions

        [Fact]
        public void Nested_LeftAssociativePlus()
        {
            // (1 + 2) + 3  →  ((1 + 2) + 3)
            var inner = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            var outer = new PlusNode(inner, new LiteralNode(3));
            Assert.Equal("((1 + 2) + 3)", outer.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_RightAssociativePlus()
        {
            // 1 + (2 + 3)  →  (1 + (2 + 3))
            var inner = new PlusNode(new LiteralNode(2), new LiteralNode(3));
            var outer = new PlusNode(new LiteralNode(1), inner);
            Assert.Equal("(1 + (2 + 3))", outer.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_MixedOperators()
        {
            // (x + 1) * (y - 2)
            var left = new PlusNode(new VariableNode("x"), new LiteralNode(1));
            var right = new MinusNode(new VariableNode("y"), new LiteralNode(2));
            var node = new TimesNode(left, right);
            Assert.Equal("((x + 1) * (y - 2))", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_ThreeLevels()
        {
            // ((2 ** 3) + (4 * 5))
            var pow = new ExponentiationNode(new LiteralNode(2), new LiteralNode(3));
            var mul = new TimesNode(new LiteralNode(4), new LiteralNode(5));
            var node = new PlusNode(pow, mul);
            Assert.Equal("((2 ** 3) + (4 * 5))", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_FourLevels()
        {
            // (((1 + 2) * 3) - (4 / 5))
            var add = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            var mul = new TimesNode(add, new LiteralNode(3));
            var div = new FloatDivNode(new LiteralNode(4), new LiteralNode(5));
            var node = new MinusNode(mul, div);
            Assert.Equal("(((1 + 2) * 3) - (4 / 5))", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_AllOperatorsCombined()
        {
            // ((a + b) % (c // d))
            var plus = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var intdiv = new IntDivNode(new VariableNode("c"), new VariableNode("d"));
            var node = new ModulusNode(plus, intdiv);
            Assert.Equal("((a + b) % (c // d))", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Nested_FiveOperators()
        {
            // (((a * b) + (c - d)) ** (e % f))
            var mul = new TimesNode(new VariableNode("a"), new VariableNode("b"));
            var sub = new MinusNode(new VariableNode("c"), new VariableNode("d"));
            var add = new PlusNode(mul, sub);
            var mod = new ModulusNode(new VariableNode("e"), new VariableNode("f"));
            var node = new ExponentiationNode(add, mod);
            Assert.Equal("(((a * b) + (c - d)) ** (e % f))", node.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  AssignmentStmt Tests
        // =====================================================================

        #region AssignmentStmt Tests

        [Fact]
        public void Assignment_BareLiteral_IsWrappedInParens()
        {
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(5));
            Assert.Equal("x := (5)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Assignment_BareVariable_IsWrappedInParens()
        {
            var stmt = new AssignmentStmt(new VariableNode("a"), new VariableNode("b"));
            Assert.Equal("a := (b)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Assignment_Expression_NotDoubleWrapped()
        {
            var expr = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            var stmt = new AssignmentStmt(new VariableNode("y"), expr);
            Assert.Equal("y := (1 + 2)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Assignment_NestedExpression()
        {
            var left = new PlusNode(new VariableNode("x"), new LiteralNode(1));
            var right = new MinusNode(new VariableNode("y"), new LiteralNode(2));
            var expr = new TimesNode(left, right);
            var stmt = new AssignmentStmt(new VariableNode("result"), expr);
            Assert.Equal("result := ((x + 1) * (y - 2))", stmt.Accept(_visitor, 0));
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "    ")]
        [InlineData(2, "        ")]
        [InlineData(3, "            ")]
        public void Assignment_IndentationScales(int level, string indent)
        {
            var stmt = new AssignmentStmt(new VariableNode("n"), new LiteralNode(0));
            Assert.Equal(indent + "n := (0)", stmt.Accept(_visitor, level));
        }

        [Fact]
        public void Assignment_NegativeLiteralRHS()
        {
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(-42));
            Assert.Equal("x := (-42)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Assignment_DoubleLiteralRHS()
        {
            var stmt = new AssignmentStmt(new VariableNode("pi"), new LiteralNode(3.14));
            Assert.Equal("pi := (3.14)", stmt.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  ReturnStmt Tests
        // =====================================================================

        #region ReturnStmt Tests

        [Fact]
        public void Return_BareLiteral_IsWrappedInParens()
        {
            var stmt = new ReturnStmt(new LiteralNode(42));
            Assert.Equal("return (42)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Return_BareVariable_IsWrappedInParens()
        {
            var stmt = new ReturnStmt(new VariableNode("x"));
            Assert.Equal("return (x)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Return_Expression_NotDoubleWrapped()
        {
            var expr = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var stmt = new ReturnStmt(expr);
            Assert.Equal("return (a + b)", stmt.Accept(_visitor, 0));
        }

        [Fact]
        public void Return_NestedExpression()
        {
            var xSq = new ExponentiationNode(new VariableNode("x"), new LiteralNode(2));
            var ySq = new ExponentiationNode(new VariableNode("y"), new LiteralNode(2));
            var expr = new PlusNode(xSq, ySq);
            var stmt = new ReturnStmt(expr);
            Assert.Equal("return ((x ** 2) + (y ** 2))", stmt.Accept(_visitor, 0));
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "    ")]
        [InlineData(2, "        ")]
        [InlineData(3, "            ")]
        public void Return_IndentationScales(int level, string indent)
        {
            var stmt = new ReturnStmt(new LiteralNode(1));
            Assert.Equal(indent + "return (1)", stmt.Accept(_visitor, level));
        }

        [Fact]
        public void Return_NegativeLiteral()
        {
            var stmt = new ReturnStmt(new LiteralNode(-99));
            Assert.Equal("return (-99)", stmt.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  BlockStmt Tests
        // =====================================================================

        #region BlockStmt Tests

        [Fact]
        public void Block_Empty()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            Assert.Equal("{\n}", block.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_SingleAssignment()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(5)));
            Assert.Equal("{\n    x := (5)\n}", block.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_SingleReturn()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new ReturnStmt(new LiteralNode(0)));
            Assert.Equal("{\n    return (0)\n}", block.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_MultipleStatements()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            block.Add(new AssignmentStmt(new VariableNode("y"), new LiteralNode(2)));
            block.Add(new ReturnStmt(new PlusNode(new VariableNode("x"), new VariableNode("y"))));

            string expected = "{\n    x := (1)\n    y := (2)\n    return (x + y)\n}";
            Assert.Equal(expected, block.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_NestedOneLevelDeep()
        {
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));

            var outer = new BlockStmt(new SymbolTable<string, object>());
            outer.Add(inner);

            string expected = "{\n    {\n        x := (1)\n    }\n}";
            Assert.Equal(expected, outer.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_NestedTwoLevelsDeep()
        {
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
                "            return (99)\n" +
                "        }\n" +
                "    }\n" +
                "}";
            Assert.Equal(expected, outer.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_NestedThreeLevelsDeep()
        {
            var level4 = new BlockStmt(new SymbolTable<string, object>());
            level4.Add(new ReturnStmt(new VariableNode("d")));

            var level3 = new BlockStmt(new SymbolTable<string, object>());
            level3.Add(new AssignmentStmt(new VariableNode("d"), new LiteralNode(4)));
            level3.Add(level4);

            var level2 = new BlockStmt(new SymbolTable<string, object>());
            level2.Add(new AssignmentStmt(new VariableNode("c"), new LiteralNode(3)));
            level2.Add(level3);

            var level1 = new BlockStmt(new SymbolTable<string, object>());
            level1.Add(new AssignmentStmt(new VariableNode("b"), new LiteralNode(2)));
            level1.Add(level2);

            string result = level1.Accept(_visitor, 0);

            // Verify indent levels
            Assert.Contains("    b := (2)", result);              // level 1
            Assert.Contains("        c := (3)", result);          // level 2
            Assert.Contains("            d := (4)", result);      // level 3
            Assert.Contains("                return (d)", result); // level 4
        }

        [Fact]
        public void Block_AtIndentLevel1()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(7)));

            string expected = "    {\n        a := (7)\n    }";
            Assert.Equal(expected, block.Accept(_visitor, 1));
        }

        [Fact]
        public void Block_MixedStatementsAndNestedBlock()
        {
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
                "    x := (10)\n" +
                "    {\n" +
                "        y := (x + 1)\n" +
                "        return (y)\n" +
                "    }\n" +
                "}";
            Assert.Equal(expected, outer.Accept(_visitor, 0));
        }

        [Fact]
        public void Block_ComplexExpressionAssignment()
        {
            var mul = new TimesNode(new VariableNode("a"), new VariableNode("b"));
            var div = new FloatDivNode(new VariableNode("c"), new VariableNode("d"));
            var expr = new PlusNode(mul, div);

            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new AssignmentStmt(new VariableNode("result"), expr));

            string expected = "{\n    result := ((a * b) + (c / d))\n}";
            Assert.Equal(expected, block.Accept(_visitor, 0));
        }

        #endregion

        // =====================================================================
        //  Structural Property Tests
        // =====================================================================

        #region Structural Property Tests

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        public void Block_NStatements_HasCorrectLineCount(int n)
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            for (int i = 0; i < n; i++)
            {
                block.Add(new AssignmentStmt(new VariableNode("v" + i), new LiteralNode(i)));
            }
            string result = block.Accept(_visitor, 0);
            int lineCount = result.Split('\n').Length;
            // opening brace + n statements + closing brace
            Assert.Equal(n + 2, lineCount);
        }

        [Fact]
        public void Block_BraceCountMatchesNesting()
        {
            // 3 levels of nesting → 3 open braces and 3 close braces
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.Add(new ReturnStmt(new LiteralNode(1)));

            var middle = new BlockStmt(new SymbolTable<string, object>());
            middle.Add(inner);

            var outer = new BlockStmt(new SymbolTable<string, object>());
            outer.Add(middle);

            string result = outer.Accept(_visitor, 0);
            Assert.Equal(3, result.Count(c => c == '{'));
            Assert.Equal(3, result.Count(c => c == '}'));
        }

        [Fact]
        public void Block_StartsWithOpenBrace_EndsWithCloseBrace()
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));

            string result = block.Accept(_visitor, 0);
            Assert.StartsWith("{", result);
            Assert.EndsWith("}", result);
        }

        #endregion
    }
}
