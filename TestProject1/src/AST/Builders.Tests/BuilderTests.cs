using Xunit;
using AST;
using Utilities.Containers;
using System;
using System.IO;

namespace AST.Builders.Tests
{
    /// <summary>
    /// Comprehensive xUnit tests for the Builder classes (DefaultBuilder,
    /// NullBuilder, DebugBuilder), covering all creation methods with both
    /// Fact and Theory attributes.
    /// </summary>
    public class BuilderTests
    {
        // ─────────────────────────────────────────────────────────────
        // Helper methods
        // ─────────────────────────────────────────────────────────────

        private static LiteralNode Lit(object v) => new LiteralNode(v);
        private static VariableNode Var(string n) => new VariableNode(n);

        // ─────────────────────────────────────────────────────────────
        // DefaultBuilder Tests
        // ─────────────────────────────────────────────────────────────

        #region DefaultBuilder Tests

        [Fact]
        public void DefaultBuilder_CreatePlusNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreatePlusNode(Lit(1), Lit(2));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateMinusNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateMinusNode(Lit(5), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateTimesNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateTimesNode(Lit(2), Lit(4));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateFloatDivNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateFloatDivNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateIntDivNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateIntDivNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateModulusNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateModulusNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateExponentiationNode_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateExponentiationNode(Lit(2), Lit(3));
            Assert.Null(result);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(0)]
        [InlineData(-7)]
        [InlineData(3.14)]
        [InlineData("hello")]
        public void DefaultBuilder_CreateLiteralNode_ReturnsNull(object value)
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateLiteralNode(value);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("a")]
        public void DefaultBuilder_CreateVariableNode_ReturnsNull(string name)
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateVariableNode(name);
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateAssignmentStmt_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateAssignmentStmt(Var("x"), Lit(5));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateReturnStmt_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var result = builder.CreateReturnStmt(Lit(10));
            Assert.Null(result);
        }

        [Fact]
        public void DefaultBuilder_CreateBlockStmt_ReturnsNull()
        {
            var builder = new DefaultBuilder();
            var st = new SymbolTable<string, object>(null);
            var result = builder.CreateBlockStmt(st);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-3, 7)]
        public void DefaultBuilder_AllBinaryOperatorMethods_ReturnNull(int left, int right)
        {
            var builder = new DefaultBuilder();
            var l = Lit(left);
            var r = Lit(right);

            Assert.Null(builder.CreatePlusNode(l, r));
            Assert.Null(builder.CreateMinusNode(l, r));
            Assert.Null(builder.CreateTimesNode(l, r));
            Assert.Null(builder.CreateFloatDivNode(l, r));
            Assert.Null(builder.CreateIntDivNode(l, r));
            Assert.Null(builder.CreateModulusNode(l, r));
            Assert.Null(builder.CreateExponentiationNode(l, r));
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // NullBuilder Tests
        // ─────────────────────────────────────────────────────────────

        #region NullBuilder Tests

        [Fact]
        public void NullBuilder_CreatePlusNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreatePlusNode(Lit(1), Lit(2));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateMinusNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateMinusNode(Lit(5), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateTimesNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateTimesNode(Lit(2), Lit(4));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateFloatDivNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateFloatDivNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateIntDivNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateIntDivNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateModulusNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateModulusNode(Lit(10), Lit(3));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateExponentiationNode_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateExponentiationNode(Lit(2), Lit(3));
            Assert.Null(result);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(0)]
        [InlineData(-7)]
        [InlineData(3.14)]
        [InlineData("hello")]
        public void NullBuilder_CreateLiteralNode_ReturnsNull(object value)
        {
            var builder = new NullBuilder();
            var result = builder.CreateLiteralNode(value);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("a")]
        public void NullBuilder_CreateVariableNode_ReturnsNull(string name)
        {
            var builder = new NullBuilder();
            var result = builder.CreateVariableNode(name);
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateAssignmentStmt_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateAssignmentStmt(Var("x"), Lit(5));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateReturnStmt_ReturnsNull()
        {
            var builder = new NullBuilder();
            var result = builder.CreateReturnStmt(Lit(10));
            Assert.Null(result);
        }

        [Fact]
        public void NullBuilder_CreateBlockStmt_ReturnsNull()
        {
            var builder = new NullBuilder();
            var st = new SymbolTable<string, object>(null);
            var result = builder.CreateBlockStmt(st);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-3, 7)]
        public void NullBuilder_AllBinaryOperatorMethods_ReturnNull(int left, int right)
        {
            var builder = new NullBuilder();
            var l = Lit(left);
            var r = Lit(right);

            Assert.Null(builder.CreatePlusNode(l, r));
            Assert.Null(builder.CreateMinusNode(l, r));
            Assert.Null(builder.CreateTimesNode(l, r));
            Assert.Null(builder.CreateFloatDivNode(l, r));
            Assert.Null(builder.CreateIntDivNode(l, r));
            Assert.Null(builder.CreateModulusNode(l, r));
            Assert.Null(builder.CreateExponentiationNode(l, r));
        }

        [Fact]
        public void NullBuilder_IsSubclassOfDefaultBuilder()
        {
            var builder = new NullBuilder();
            Assert.IsAssignableFrom<DefaultBuilder>(builder);
        }

        #endregion

        // ─────────────────────────────────────────────────────────────
        // DebugBuilder Tests
        // ─────────────────────────────────────────────────────────────

        #region DebugBuilder Tests – Fact (node creation & type)

        [Fact]
        public void DebugBuilder_IsSubclassOfDefaultBuilder()
        {
            var builder = new DebugBuilder();
            Assert.IsAssignableFrom<DefaultBuilder>(builder);
        }

        [Fact]
        public void DebugBuilder_CreatePlusNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreatePlusNode(Lit(1), Lit(2));
            Assert.NotNull(result);
            Assert.IsType<PlusNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateMinusNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateMinusNode(Lit(5), Lit(3));
            Assert.NotNull(result);
            Assert.IsType<MinusNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateTimesNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateTimesNode(Lit(2), Lit(4));
            Assert.NotNull(result);
            Assert.IsType<TimesNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateFloatDivNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateFloatDivNode(Lit(10), Lit(3));
            Assert.NotNull(result);
            Assert.IsType<FloatDivNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateIntDivNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateIntDivNode(Lit(10), Lit(3));
            Assert.NotNull(result);
            Assert.IsType<IntDivNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateModulusNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateModulusNode(Lit(10), Lit(3));
            Assert.NotNull(result);
            Assert.IsType<ModulusNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateExponentiationNode_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateExponentiationNode(Lit(2), Lit(3));
            Assert.NotNull(result);
            Assert.IsType<ExponentiationNode>(result);
        }

        [Fact]
        public void DebugBuilder_CreateAssignmentStmt_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateAssignmentStmt(Var("x"), Lit(5));
            Assert.NotNull(result);
            Assert.IsType<AssignmentStmt>(result);
        }

        [Fact]
        public void DebugBuilder_CreateReturnStmt_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var result = builder.CreateReturnStmt(Lit(10));
            Assert.NotNull(result);
            Assert.IsType<ReturnStmt>(result);
        }

        [Fact]
        public void DebugBuilder_CreateBlockStmt_ReturnsNonNull()
        {
            var builder = new DebugBuilder();
            var st = new SymbolTable<string, object>(null);
            var result = builder.CreateBlockStmt(st);
            Assert.NotNull(result);
            Assert.IsType<BlockStmt>(result);
        }

        #endregion

        #region DebugBuilder Tests – Theory (parameterized node values)

        [Theory]
        [InlineData(42)]
        [InlineData(0)]
        [InlineData(-7)]
        [InlineData(3.14)]
        [InlineData("hello")]
        public void DebugBuilder_CreateLiteralNode_ReturnsNodeWithCorrectData(object value)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateLiteralNode(value);

            Assert.NotNull(result);
            Assert.IsType<LiteralNode>(result);
            Assert.Equal(value, result.Data);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("counter")]
        [InlineData("a")]
        public void DebugBuilder_CreateVariableNode_ReturnsNodeWithCorrectName(string name)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateVariableNode(name);

            Assert.NotNull(result);
            Assert.IsType<VariableNode>(result);
            Assert.Equal(name, result.Name);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-3, 7)]
        [InlineData(100, 200)]
        public void DebugBuilder_CreatePlusNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreatePlusNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(10, 3)]
        [InlineData(0, 5)]
        [InlineData(-1, -1)]
        public void DebugBuilder_CreateMinusNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateMinusNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(2, 4)]
        [InlineData(0, 99)]
        [InlineData(-5, 3)]
        public void DebugBuilder_CreateTimesNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateTimesNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(10, 3)]
        [InlineData(7, 2)]
        public void DebugBuilder_CreateFloatDivNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateFloatDivNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(10, 3)]
        [InlineData(7, 2)]
        public void DebugBuilder_CreateIntDivNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateIntDivNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(10, 3)]
        [InlineData(7, 2)]
        public void DebugBuilder_CreateModulusNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateModulusNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(5, 0)]
        public void DebugBuilder_CreateExponentiationNode_SetsLeftAndRight(int left, int right)
        {
            var builder = new DebugBuilder();
            var l = Lit(left);
            var r = Lit(right);
            var result = builder.CreateExponentiationNode(l, r);

            Assert.Same(l, result.Left);
            Assert.Same(r, result.Right);
        }

        #endregion

        #region DebugBuilder Tests – Unparse correctness

        [Theory]
        [InlineData(1, 2, "(1+2)")]
        [InlineData(0, 0, "(0+0)")]
        [InlineData(-3, 7, "(-3+7)")]
        public void DebugBuilder_PlusNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreatePlusNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(5, 3, "(5-3)")]
        [InlineData(0, 1, "(0-1)")]
        public void DebugBuilder_MinusNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateMinusNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(2, 4, "(2*4)")]
        [InlineData(0, 99, "(0*99)")]
        public void DebugBuilder_TimesNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateTimesNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(10, 3, "(10/3)")]
        public void DebugBuilder_FloatDivNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateFloatDivNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(10, 3, "(10//3)")]
        public void DebugBuilder_IntDivNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateIntDivNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(10, 3, "(10%3)")]
        public void DebugBuilder_ModulusNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateModulusNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        [Theory]
        [InlineData(2, 3, "(2**3)")]
        public void DebugBuilder_ExponentiationNode_UnparsesCorrectly(int left, int right, string expected)
        {
            var builder = new DebugBuilder();
            var result = builder.CreateExponentiationNode(Lit(left), Lit(right));
            Assert.Equal(expected, result.Unparse());
        }

        #endregion

        #region DebugBuilder Tests – Console output

        [Fact]
        public void DebugBuilder_CreatePlusNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreatePlusNode(Lit(1), Lit(2));

            var text = output.ToString();
            Assert.Contains("PlusNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateMinusNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateMinusNode(Lit(5), Lit(3));

            var text = output.ToString();
            Assert.Contains("MinusNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateTimesNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateTimesNode(Lit(2), Lit(4));

            var text = output.ToString();
            Assert.Contains("TimesNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateFloatDivNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateFloatDivNode(Lit(10), Lit(3));

            var text = output.ToString();
            Assert.Contains("FloatDivNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateIntDivNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateIntDivNode(Lit(10), Lit(3));

            var text = output.ToString();
            Assert.Contains("IntDivNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateModulusNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateModulusNode(Lit(10), Lit(3));

            var text = output.ToString();
            Assert.Contains("ModulusNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateExponentiationNode_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateExponentiationNode(Lit(2), Lit(3));

            var text = output.ToString();
            Assert.Contains("ExponentiationNode", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Theory]
        [InlineData(42)]
        [InlineData(0)]
        [InlineData("test")]
        public void DebugBuilder_CreateLiteralNode_WritesToConsole(object value)
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateLiteralNode(value);

            var text = output.ToString();
            Assert.Contains("LiteralNode", text);
            Assert.Contains(value.ToString()!, text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        public void DebugBuilder_CreateVariableNode_WritesToConsole(string name)
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateVariableNode(name);

            var text = output.ToString();
            Assert.Contains("VariableNode", text);
            Assert.Contains(name, text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateAssignmentStmt_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateAssignmentStmt(Var("x"), Lit(5));

            var text = output.ToString();
            Assert.Contains("AssignmentStmt", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateReturnStmt_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateReturnStmt(Lit(10));

            var text = output.ToString();
            Assert.Contains("ReturnStmt", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void DebugBuilder_CreateBlockStmt_WritesToConsole()
        {
            var builder = new DebugBuilder();
            var output = new StringWriter();
            Console.SetOut(output);

            builder.CreateBlockStmt(new SymbolTable<string, object>(null));

            var text = output.ToString();
            Assert.Contains("BlockStmt", text);
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        #endregion

        #region DebugBuilder Tests – Statement structure

        [Theory]
        [InlineData("x", 10)]
        [InlineData("y", 0)]
        [InlineData("total", -5)]
        public void DebugBuilder_CreateAssignmentStmt_SetsVariableAndExpression(string varName, int value)
        {
            var builder = new DebugBuilder();
            var variable = Var(varName);
            var expression = Lit(value);
            var result = builder.CreateAssignmentStmt(variable, expression);

            Assert.Same(expression, result.Right);
            Assert.Same(variable, result.Left);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(-99)]
        public void DebugBuilder_CreateReturnStmt_SetsChildExpression(int value)
        {
            var builder = new DebugBuilder();
            var expression = Lit(value);
            var result = builder.CreateReturnStmt(expression);

            Assert.Same(expression, result.Child);
        }

        [Fact]
        public void DebugBuilder_CreateBlockStmt_SetsSymbolTable()
        {
            var builder = new DebugBuilder();
            var st = new SymbolTable<string, object>(null);
            var result = builder.CreateBlockStmt(st);

            Assert.Same(st, result.SymbolTable);
            Assert.Empty(result.children);
        }

        [Fact]
        public void DebugBuilder_CreateBlockStmt_CanAddChildren()
        {
            var builder = new DebugBuilder();
            var st = new SymbolTable<string, object>(null);
            var block = builder.CreateBlockStmt(st);

            var ret = builder.CreateReturnStmt(Lit(42));
            block.Add(ret);

            Assert.Single(block.children);
            Assert.Same(ret, block.children[0]);
        }

        #endregion
    }
}
