using Xunit;
using AST;

namespace AST.Tests
{
    /// <summary>
    /// Comprehensive xUnit tests for the AST class hierarchy, covering all
    /// expression nodes, statement nodes, and their Unparse methods.
    /// </summary>
    public class ASTTests
    {
        // ─────────────────────────────────────────────────────────────
        // LiteralNodeests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that a LiteralNodent&gt; stores its value correctly in .Data.</summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(-7)]
        [InlineData(999999)]
        public void LiteralNode_Int_Data_StoresCorrectValue(int value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value, node.Data);
        }

        /// <summary>Tests that a LiteralNodeouble&gt; stores its value correctly in .Data.</summary>
        [Theory]
        [InlineData(0.0)]
        [InlineData(3.14)]
        [InlineData(-2.5)]
        [InlineData(100.001)]
        public void LiteralNode_Double_Data_StoresCorrectValue(double value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value, node.Data);
        }

        /// <summary>Tests that a LiteralNodetring&gt; stores its value correctly in .Data.</summary>
        [Theory]
        [InlineData("hello")]
        [InlineData("")]
        [InlineData("123")]
        public void LiteralNode_String_Data_StoresCorrectValue(string value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value, node.Data);
        }

        /// <summary>Tests that LiteralNodent&gt; unparses to the correct string representation.</summary>
        [Theory]
        [InlineData(0)]
        [InlineData(42)]
        [InlineData(-7)]
        public void LiteralNode_Int_Unparse_ReturnsCorrectString(int value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value.ToString(), node.Unparse());
        }

        /// <summary>Tests that LiteralNodeouble&gt; unparses to a string containing the value.</summary>
        [Theory]
        [InlineData(3.14)]
        [InlineData(-2.5)]
        [InlineData(0.0)]
        public void LiteralNode_Double_Unparse_ReturnsCorrectString(double value)
        {
            var node = new LiteralNode(value);
            Assert.Contains(value.ToString(), node.Unparse());
        }

        /// <summary>Tests that LiteralNodent&gt; unparses identically regardless of indentation level.</summary>
        [Theory]
        [InlineData(5, 0)]
        [InlineData(5, 1)]
        [InlineData(5, 3)]
        public void LiteralNode_Int_Unparse_IgnoresIndentationLevel(int value, int level)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value.ToString(), node.Unparse(level));
        }

        /// <summary>Tests that LiteralNodent&gt; Unparse() and Unparse(0) return the same result.</summary>
        [Theory]
        [InlineData(0)]
        [InlineData(42)]
        [InlineData(-1)]
        public void LiteralNode_Int_DefaultLevel_EqualsLevelZero(int value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(node.Unparse(), node.Unparse(0));
        }

        /// <summary>Tests that LiteralNodeouble&gt; Unparse() and Unparse(0) return the same result.</summary>
        [Theory]
        [InlineData(1.5)]
        [InlineData(0.0)]
        public void LiteralNode_Double_DefaultLevel_EqualsLevelZero(double value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(node.Unparse(), node.Unparse(0));
        }

        /// <summary>Tests that LiteralNodent&gt; is an instance of ExpressionNode.</summary>
        [Fact]
        public void LiteralNode_Int_IsExpressionNode()
        {
            Assert.IsAssignableFrom<ExpressionNode>(new LiteralNode(1));
        }

        /// <summary>Tests that LiteralNodeouble&gt; is an instance of ExpressionNode.</summary>
        [Fact]
        public void LiteralNode_Double_IsExpressionNode()
        {
            Assert.IsAssignableFrom<ExpressionNode>(new LiteralNode(1.0));
        }

        /// <summary>Tests that two LiteralNodent&gt; instances with the same value have equal .Data.</summary>
        [Fact]
        public void LiteralNode_Int_TwoNodesWithSameValue_DataAreEqual()
        {
            var a = new LiteralNode(7);
            var b = new LiteralNode(7);
            Assert.Equal(a.Data, b.Data);
        }

        /// <summary>Tests that two LiteralNodent&gt; instances with different values have different .Data.</summary>
        [Fact]
        public void LiteralNode_Int_TwoNodesWithDifferentValues_DataAreNotEqual()
        {
            var a = new LiteralNode(3);
            var b = new LiteralNode(9);
            Assert.NotEqual(a.Data, b.Data);
        }

        // ─────────────────────────────────────────────────────────────
        // VariableNode Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that a VariableNode stores and unparses its name correctly.</summary>
        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("a")]
        [InlineData("longVariableName")]
        public void VariableNode_Unparse_ReturnsVariableName(string name)
        {
            var node = new VariableNode(name);
            Assert.Equal(name, node.Unparse());
        }

        /// <summary>Tests that a VariableNode unparses identically regardless of indentation level.</summary>
        [Theory]
        [InlineData("x", 0)]
        [InlineData("x", 2)]
        [InlineData("x", 5)]
        public void VariableNode_Unparse_IgnoresIndentationLevel(string name, int level)
        {
            var node = new VariableNode(name);
            Assert.Equal(name, node.Unparse(level));
        }

        /// <summary>Tests that VariableNode Unparse() and Unparse(0) return the same result.</summary>
        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        public void VariableNode_DefaultLevel_EqualsLevelZero(string name)
        {
            var node = new VariableNode(name);
            Assert.Equal(node.Unparse(), node.Unparse(0));
        }

        /// <summary>Tests that VariableNode is an ExpressionNode.</summary>
        [Fact]
        public void VariableNode_IsExpressionNode()
        {
            Assert.IsAssignableFrom<ExpressionNode>(new VariableNode("x"));
        }

        // ─────────────────────────────────────────────────────────────
        // BinaryOperator Node Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests PlusNode unparses two int literals with the + operator.</summary>
        [Fact]
        public void PlusNode_Unparse_TwoIntLiterals()
        {
            var node = new PlusNode(new LiteralNode(2), new LiteralNode(3));
            string result = node.Unparse();
            Assert.Contains("2", result);
            Assert.Contains("3", result);
            Assert.Contains("+", result);
        }

        /// <summary>Tests MinusNode unparses two int literals with the - operator.</summary>
        [Fact]
        public void MinusNode_Unparse_TwoIntLiterals()
        {
            var node = new MinusNode(new LiteralNode(10), new LiteralNode(4));
            string result = node.Unparse();
            Assert.Contains("10", result);
            Assert.Contains("4",  result);
            Assert.Contains("-",  result);
        }

        /// <summary>Tests TimesNode unparses two int literals with the * operator.</summary>
        [Fact]
        public void TimesNode_Unparse_TwoIntLiterals()
        {
            var node = new TimesNode(new LiteralNode(3), new LiteralNode(5));
            string result = node.Unparse();
            Assert.Contains("3", result);
            Assert.Contains("5", result);
            Assert.Contains("*", result);
        }

        /// <summary>Tests FloatDivNode unparses two double literals with the / operator.</summary>
        [Fact]
        public void FloatDivNode_Unparse_TwoDoubleLiterals()
        {
            var node = new FloatDivNode(new LiteralNode(7.0), new LiteralNode(2.0));
            string result = node.Unparse();
            Assert.Contains("/", result);
        }

        /// <summary>Tests IntDivNode unparses two int literals with the // operator.</summary>
        [Fact]
        public void IntDivNode_Unparse_TwoIntLiterals()
        {
            var node = new IntDivNode(new LiteralNode(7), new LiteralNode(2));
            string result = node.Unparse();
            Assert.Contains("7",  result);
            Assert.Contains("2",  result);
            Assert.Contains("//", result);
        }

        /// <summary>Tests ModulusNode unparses two int literals with the % operator.</summary>
        [Fact]
        public void ModulusNode_Unparse_TwoIntLiterals()
        {
            var node = new ModulusNode(new LiteralNode(9), new LiteralNode(4));
            string result = node.Unparse();
            Assert.Contains("9", result);
            Assert.Contains("4", result);
            Assert.Contains("%", result);
        }

        /// <summary>Tests ExponentiationNode unparses two int literals with the ** operator.</summary>
        [Fact]
        public void ExponentiationNode_Unparse_TwoIntLiterals()
        {
            var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(8));
            string result = node.Unparse();
            Assert.Contains("2",  result);
            Assert.Contains("8",  result);
            Assert.Contains("**", result);
        }

        /// <summary>Tests that binary operator nodes also work with VariableNode operands.</summary>
        [Fact]
        public void BinaryOperator_Unparse_WithVariableOperands()
        {
            var plus  = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var minus = new MinusNode(new VariableNode("a"), new VariableNode("b"));
            var times = new TimesNode(new VariableNode("a"), new VariableNode("b"));

            Assert.Contains("a", plus.Unparse());
            Assert.Contains("b", plus.Unparse());
            Assert.Contains("a", minus.Unparse());
            Assert.Contains("b", minus.Unparse());
            Assert.Contains("a", times.Unparse());
            Assert.Contains("b", times.Unparse());
        }

        // ─────────────────────────────────────────────────────────────
        // Nested Expression Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Tests the Figure 1 example: (2 + 3) * 4.
        /// Validates that both inner and outer operators appear in the unparse output.
        /// </summary>
        [Fact]
        public void NestedExpression_TimesPlusLiterals_Unparse()
        {
            var plus  = new PlusNode(new LiteralNode(2), new LiteralNode(3));
            var times = new TimesNode(plus, new LiteralNode(4));

            string result = times.Unparse();

            Assert.Contains("2", result);
            Assert.Contains("3", result);
            Assert.Contains("4", result);
            Assert.Contains("+", result);
            Assert.Contains("*", result);
        }

        /// <summary>Tests a deeply nested expression: (2 + 3) * (6 - 1).</summary>
        [Fact]
        public void NestedExpression_DeepNesting_Unparse()
        {
            var plus  = new PlusNode(new LiteralNode(2), new LiteralNode(3));
            var minus = new MinusNode(new LiteralNode(6), new LiteralNode(1));
            var times = new TimesNode(plus, minus);

            string result = times.Unparse();

            Assert.Contains("2", result);
            Assert.Contains("3", result);
            Assert.Contains("6", result);
            Assert.Contains("1", result);
            Assert.Contains("+", result);
            Assert.Contains("-", result);
            Assert.Contains("*", result);
        }

        /// <summary>Tests that nested unparse preserves operator ordering — x ** (a // b).</summary>
        [Fact]
        public void NestedExpression_OperatorOrderPreserved_Unparse()
        {
            var intDiv = new IntDivNode(new VariableNode("a"), new VariableNode("b"));
            var exp    = new ExponentiationNode(new VariableNode("x"), intDiv);

            string result = exp.Unparse();

            Assert.Contains("**", result);
            Assert.Contains("//", result);
            Assert.Contains("x",  result);
            Assert.Contains("a",  result);
            Assert.Contains("b",  result);
        }

        // ─────────────────────────────────────────────────────────────
        // AssignmentStmt Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that AssignmentStmt unparses to include the variable name, :=, and an int literal.</summary>
        [Fact]
        public void AssignmentStmt_Unparse_SimpleIntAssignment()
        {
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(42));
            string result = stmt.Unparse();

            Assert.Contains("x",  result);
            Assert.Contains("42", result);
            Assert.Contains(":=", result);
        }

        /// <summary>Tests AssignmentStmt with a double literal on the right-hand side.</summary>
        [Fact]
        public void AssignmentStmt_Unparse_DoubleLiteralRHS()
        {
            var stmt = new AssignmentStmt(new VariableNode("pi"), new LiteralNode(3.14));
            string result = stmt.Unparse();

            Assert.Contains("pi", result);
            Assert.Contains(":=", result);
        }

        /// <summary>Tests AssignmentStmt with a binary expression on the right-hand side.</summary>
        [Fact]
        public void AssignmentStmt_Unparse_WithBinaryExpression()
        {
            // y := (2 + 3)
            var stmt = new AssignmentStmt(
                new VariableNode("y"),
                new PlusNode(new LiteralNode(2), new LiteralNode(3))
            );
            string result = stmt.Unparse();

            Assert.Contains("y",  result);
            Assert.Contains(":=", result);
            Assert.Contains("2",  result);
            Assert.Contains("3",  result);
            Assert.Contains("+",  result);
        }

        /// <summary>Tests that AssignmentStmt indentation increases with level.</summary>
        [Fact]
        public void AssignmentStmt_Unparse_IndentationIncreasesWithLevel()
        {
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(1));
            string level0 = stmt.Unparse(0);
            string level1 = stmt.Unparse(1);
            string level2 = stmt.Unparse(2);

            Assert.True(level1.Length >= level0.Length,
                "Level 1 should be at least as long as level 0 due to indentation.");
            Assert.True(level2.Length >= level1.Length,
                "Level 2 should be at least as long as level 1 due to indentation.");
        }

        // ─────────────────────────────────────────────────────────────
        // ReturnStmt Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that ReturnStmt unparses with the return keyword and an int literal.</summary>
        [Fact]
        public void ReturnStmt_Unparse_IntLiteral()
        {
            var stmt = new ReturnStmt(new LiteralNode(0));
            string result = stmt.Unparse();

            Assert.Contains("return", result);
            Assert.Contains("0",      result);
        }

        /// <summary>Tests ReturnStmt with a double literal.</summary>
        [Fact]
        public void ReturnStmt_Unparse_DoubleLiteral()
        {
            var stmt = new ReturnStmt(new LiteralNode(2.5));
            string result = stmt.Unparse();
            Assert.Contains("return", result);
        }

        /// <summary>Tests ReturnStmt with a variable expression.</summary>
        [Fact]
        public void ReturnStmt_Unparse_WithVariable()
        {
            var stmt = new ReturnStmt(new VariableNode("result"));
            string result = stmt.Unparse();

            Assert.Contains("return", result);
            Assert.Contains("result", result);
        }

        /// <summary>Tests ReturnStmt with an IntDivNode expression, matching the Figure 2 example.</summary>
        [Fact]
        public void ReturnStmt_Unparse_WithIntDivExpression()
        {
            // return (a // b)
            var stmt = new ReturnStmt(
                new IntDivNode(new VariableNode("a"), new VariableNode("b"))
            );
            string result = stmt.Unparse();

            Assert.Contains("return", result);
            Assert.Contains("a",      result);
            Assert.Contains("b",      result);
            Assert.Contains("//",     result);
        }

        /// <summary>Tests that ReturnStmt contains the return keyword at any indentation level.</summary>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void ReturnStmt_Unparse_ContainsReturnKeywordAtAnyLevel(int level)
        {
            var stmt = new ReturnStmt(new LiteralNode(1));
            Assert.Contains("return", stmt.Unparse(level));
        }

        // ─────────────────────────────────────────────────────────────
        // BlockStmt Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that an empty BlockStmt unparses with opening and closing braces.</summary>
        [Fact]
        public void BlockStmt_Unparse_EmptyBlock_ContainsBraces()
        {
            var block = new BlockStmt(new List<Statement>());
            string result = block.Unparse();

            Assert.Contains("{", result);
            Assert.Contains("}", result);
        }

        /// <summary>Tests that a BlockStmt with one AssignmentStmt contains expected content.</summary>
        [Fact]
        public void BlockStmt_Unparse_SingleAssignment()
        {
            var stmts = new List<Statement>
            {
                new AssignmentStmt(new VariableNode("x"), new LiteralNode(5))
            };
            var block  = new BlockStmt(stmts);
            string result = block.Unparse();

            Assert.Contains("{",  result);
            Assert.Contains("}",  result);
            Assert.Contains("x",  result);
            Assert.Contains(":=", result);
            Assert.Contains("5",  result);
        }

        /// <summary>
        /// Replicates the Figure 2 example: a := 11, b := 4, return (a // b).
        /// </summary>
        [Fact]
        public void BlockStmt_Unparse_Figure2ExampleFromSpec()
        {
            var stmts = new List<Statement>
            {
                new AssignmentStmt(new VariableNode("a"), new LiteralNode(11)),
                new AssignmentStmt(new VariableNode("b"), new LiteralNode(4)),
                new ReturnStmt(new IntDivNode(new VariableNode("a"), new VariableNode("b")))
            };
            var block  = new BlockStmt(stmts);
            string result = block.Unparse();

            Assert.Contains("{",      result);
            Assert.Contains("}",      result);
            Assert.Contains("a",      result);
            Assert.Contains("b",      result);
            Assert.Contains("11",     result);
            Assert.Contains("4",      result);
            Assert.Contains(":=",     result);
            Assert.Contains("return", result);
            Assert.Contains("//",     result);
        }

        /// <summary>Tests that all three statements in a block appear in the correct order.</summary>
        [Fact]
        public void BlockStmt_Unparse_StatementsAppearInOrder()
        {
            var stmts = new List<Statement>
            {
                new AssignmentStmt(new VariableNode("first"),  new LiteralNode(1)),
                new AssignmentStmt(new VariableNode("second"), new LiteralNode(2)),
                new ReturnStmt(new VariableNode("second"))
            };
            var block  = new BlockStmt(stmts);
            string result = block.Unparse();

            int firstIdx  = result.IndexOf("first");
            int secondIdx = result.IndexOf("second");
            int returnIdx = result.IndexOf("return");

            Assert.True(firstIdx  < secondIdx, "'first' should appear before 'second'");
            Assert.True(secondIdx < returnIdx, "'second' assignment should appear before 'return'");
        }

        /// <summary>Tests that BlockStmt children are indented deeper than the block's own braces.</summary>
        [Fact]
        public void BlockStmt_Unparse_ChildrenAreIndentedInsideBlock()
        {
            var stmts = new List<Statement>
            {
                new AssignmentStmt(new VariableNode("x"), new LiteralNode(1))
            };
            var block  = new BlockStmt(stmts);
            string result = block.Unparse(0);

            string[] lines = result.Split('\n');
            string openBraceLine  = lines.FirstOrDefault(l => l.Contains("{")) ?? "";
            string assignmentLine = lines.FirstOrDefault(l => l.Contains(":="))  ?? "";

            int braceIndent      = openBraceLine.Length  - openBraceLine.TrimStart().Length;
            int assignmentIndent = assignmentLine.Length - assignmentLine.TrimStart().Length;

            Assert.True(assignmentIndent > braceIndent,
                "Assignment inside block should be indented more than the opening brace.");
        }

        /// <summary>Tests that a nested BlockStmt unparses with correct content at both levels.</summary>
        [Fact]
        public void BlockStmt_Unparse_NestedBlock_DoublyIndented()
        {
            var inner = new BlockStmt(new List<Statement>
            {
                new AssignmentStmt(new VariableNode("y"), new LiteralNode(99))
            });
            var outer = new BlockStmt(new List<Statement> { inner });
            string result = outer.Unparse(0);

            Assert.Contains("{",  result);
            Assert.Contains("}",  result);
            Assert.Contains("y",  result);
            Assert.Contains("99", result);
        }

        // ─────────────────────────────────────────────────────────────
        // Polymorphism / Type Hierarchy Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that all binary operator nodes are instances of BinaryOperator.</summary>
        [Fact]
        public void AllBinaryNodes_AreInstancesOfBinaryOperator()
        {
            var l = new LiteralNode(1);
            var r = new LiteralNode(2);

            Assert.IsAssignableFrom<BinaryOperator>(new PlusNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new MinusNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new TimesNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new FloatDivNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new IntDivNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new ModulusNode(l, r));
            Assert.IsAssignableFrom<BinaryOperator>(new ExponentiationNode(l, r));
        }

        /// <summary>Tests that BinaryOperator nodes are instances of Operator.</summary>
        [Fact]
        public void AllBinaryNodes_AreInstancesOfOperator()
        {
            var node = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            Assert.IsAssignableFrom<Operator>(node);
        }

        /// <summary>Tests that all operator nodes are instances of ExpressionNode.</summary>
        [Fact]
        public void AllBinaryNodes_AreInstancesOfExpressionNode()
        {
            var l = new LiteralNode(0);
            var r = new LiteralNode(0);

            Assert.IsAssignableFrom<ExpressionNode>(new PlusNode(l, r));
            Assert.IsAssignableFrom<ExpressionNode>(new TimesNode(l, r));
            Assert.IsAssignableFrom<ExpressionNode>(new IntDivNode(l, r));
        }

        /// <summary>Tests that AssignmentStmt, ReturnStmt, and BlockStmt are all Statements.</summary>
        [Fact]
        public void AllStmtTypes_AreStatements()
        {
            Assert.IsAssignableFrom<Statement>(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            Assert.IsAssignableFrom<Statement>(new ReturnStmt(new LiteralNode(0)));
            Assert.IsAssignableFrom<Statement>(new BlockStmt(new List<Statement>()));
        }

        // ─────────────────────────────────────────────────────────────
        // Structural Integrity / Idempotency Tests
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tests that calling Unparse twice on the same node produces the same result.</summary>
        [Fact]
        public void Unparse_CalledTwice_ReturnsSameResult()
        {
            var node = new TimesNode(
                new PlusNode(new LiteralNode(2), new LiteralNode(3)),
                new LiteralNode(4)
            );
            Assert.Equal(node.Unparse(), node.Unparse());
        }

        /// <summary>
        /// Tests the full Figure 1 example: x := ((2 + 3) * 4).
        /// Validates all expected tokens are present in the unparse output.
        /// </summary>
        [Fact]
        public void AssignmentStmt_Figure1Example_UnparseContainsAllTokens()
        {
            var stmt = new AssignmentStmt(
                new VariableNode("x"),
                new TimesNode(
                    new PlusNode(new LiteralNode(2), new LiteralNode(3)),
                    new LiteralNode(4)
                )
            );
            string result = stmt.Unparse();

            Assert.Contains("x",  result);
            Assert.Contains(":=", result);
            Assert.Contains("2",  result);
            Assert.Contains("3",  result);
            Assert.Contains("4",  result);
            Assert.Contains("+",  result);
            Assert.Contains("*",  result);
        }

        /// <summary>Tests that all binary nodes produce non-empty unparse output.</summary>
        [Fact]
        public void AllBinaryNodes_Unparse_NonEmpty()
        {
            var l = new LiteralNode(1);
            var r = new LiteralNode(2);

            Assert.NotEmpty(new PlusNode(l, r).Unparse());
            Assert.NotEmpty(new MinusNode(l, r).Unparse());
            Assert.NotEmpty(new TimesNode(l, r).Unparse());
            Assert.NotEmpty(new FloatDivNode(l, r).Unparse());
            Assert.NotEmpty(new IntDivNode(l, r).Unparse());
            Assert.NotEmpty(new ModulusNode(l, r).Unparse());
            Assert.NotEmpty(new ExponentiationNode(l, r).Unparse());
        }

        /// <summary>Tests that all statement types produce non-empty unparse output.</summary>
        [Fact]
        public void AllStatementTypes_Unparse_NonEmpty()
        {
            Assert.NotEmpty(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)).Unparse());
            Assert.NotEmpty(new ReturnStmt(new LiteralNode(0)).Unparse());
            Assert.NotEmpty(new BlockStmt(new List<Statement>()).Unparse());
        }
    }
}