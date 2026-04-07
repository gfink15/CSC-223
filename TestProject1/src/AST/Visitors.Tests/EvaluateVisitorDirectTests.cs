// =============================================================================
// EvaluateVisitor — Direct Unit Tests
// Manually constructs AST nodes and calls Accept to verify each Visit method
// correctly evaluates expressions, assignments, returns, and blocks.
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
    public class EvaluateVisitorDirectTests
    {
        private readonly EvaluateVisitor _visitor;
        private readonly SymbolTable<string, object> _table;

        public EvaluateVisitorDirectTests()
        {
            _visitor = new EvaluateVisitor();
            _table = new SymbolTable<string, object>();
        }

        // =====================================================================
        //  LiteralNode Tests
        // =====================================================================

        #region LiteralNode Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-42)]
        [InlineData(999)]
        public void Literal_IntReturnsInt(int value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value, node.Accept(_visitor, _table));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(3.14)]
        [InlineData(-2.718)]
        [InlineData(100.5)]
        public void Literal_DoubleReturnsDouble(double value)
        {
            var node = new LiteralNode(value);
            Assert.Equal(value, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Literal_StringReturnsString()
        {
            var node = new LiteralNode("hello");
            Assert.Equal("hello", node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  VariableNode Tests
        // =====================================================================

        #region VariableNode Tests

        [Fact]
        public void Variable_ReturnsStoredValue()
        {
            _table["x"] = 99;
            Assert.Equal(99, new VariableNode("x").Accept(_visitor, _table));
        }

        [Fact]
        public void Variable_ReturnsDoubleValue()
        {
            _table["pi"] = 3.14;
            Assert.Equal(3.14, new VariableNode("pi").Accept(_visitor, _table));
        }

        [Fact]
        public void Variable_UndefinedThrows()
        {
            Assert.Throws<KeyNotFoundException>(
                () => new VariableNode("undefined").Accept(_visitor, _table));
        }

        [Fact]
        public void Variable_FromParentScope()
        {
            _table["x"] = 42;
            var child = new SymbolTable<string, object>(_table);
            Assert.Equal(42, new VariableNode("x").Accept(_visitor, child));
        }

        #endregion

        // =====================================================================
        //  PlusNode Tests
        // =====================================================================

        #region PlusNode Tests

        [Fact]
        public void Plus_IntPlusInt()
        {
            var node = new PlusNode(new LiteralNode(3), new LiteralNode(4));
            var result = Convert.ToDouble(node.Accept(_visitor, _table));
            Assert.Equal(7.0, result);
        }

        [Fact]
        public void Plus_IntPlusDouble()
        {
            var node = new PlusNode(new LiteralNode(1), new LiteralNode(1.5));
            Assert.Equal(2.5, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Plus_DoublePlusDouble()
        {
            var node = new PlusNode(new LiteralNode(1.1), new LiteralNode(2.2));
            var result = (double)node.Accept(_visitor, _table);
            Assert.InRange(result, 3.29, 3.31);
        }

        [Fact]
        public void Plus_WithVariables()
        {
            _table["a"] = 10;
            _table["b"] = 20;
            var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            Assert.Equal(30.0, node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  MinusNode Tests
        // =====================================================================

        #region MinusNode Tests

        [Fact]
        public void Minus_IntMinusInt()
        {
            var node = new MinusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.Equal(7.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Minus_DoubleMinusDouble()
        {
            var node = new MinusNode(new LiteralNode(5.5), new LiteralNode(2.5));
            Assert.Equal(3.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Minus_ResultNegative()
        {
            var node = new MinusNode(new LiteralNode(1), new LiteralNode(5));
            Assert.Equal(-4.0, node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  TimesNode Tests
        // =====================================================================

        #region TimesNode Tests

        [Fact]
        public void Times_IntTimesInt()
        {
            var node = new TimesNode(new LiteralNode(3), new LiteralNode(7));
            Assert.Equal(21.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Times_DoubleTimesDouble()
        {
            var node = new TimesNode(new LiteralNode(2.5), new LiteralNode(4.0));
            Assert.Equal(10.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Times_ByZero()
        {
            var node = new TimesNode(new LiteralNode(100), new LiteralNode(0));
            Assert.Equal(0.0, node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  FloatDivNode Tests
        // =====================================================================

        #region FloatDivNode Tests

        [Theory]
        [InlineData(10, 4, 2.5)]
        [InlineData(7, 2, 3.5)]
        [InlineData(1, 4, 0.25)]
        public void FloatDiv_ReturnsDouble(int left, int right, double expected)
        {
            var node = new FloatDivNode(new LiteralNode(left), new LiteralNode(right));
            Assert.Equal(expected, (double)node.Accept(_visitor, _table), precision: 10);
        }

        [Fact]
        public void FloatDiv_ByZeroThrows()
        {
            var node = new FloatDivNode(new LiteralNode(5), new LiteralNode(0));
            Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  IntDivNode Tests
        // =====================================================================

        #region IntDivNode Tests

        [Theory]
        [InlineData(10, 3, 3)]
        [InlineData(7, 2, 3)]
        [InlineData(-7, 2, -3)]
        [InlineData(9, 3, 3)]
        [InlineData(1, 1, 1)]
        public void IntDiv_Truncates(int left, int right, int expected)
        {
            var node = new IntDivNode(new LiteralNode(left), new LiteralNode(right));
            Assert.Equal(expected, node.Accept(_visitor, _table));
        }

        [Fact]
        public void IntDiv_ByZeroThrows()
        {
            var node = new IntDivNode(new LiteralNode(10), new LiteralNode(0));
            Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  ModulusNode Tests
        // =====================================================================

        #region ModulusNode Tests

        [Fact]
        public void Modulus_ByZeroThrows()
        {
            var node = new ModulusNode(new LiteralNode(10), new LiteralNode(0));
            Assert.Throws<EvaluationException>(() => node.Accept(_visitor, _table));
        }

        [Fact]
        public void Modulus_BasicMod()
        {
            var node = new ModulusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.Equal(1.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Modulus_EvenlyDivisible()
        {
            var node = new ModulusNode(new LiteralNode(9), new LiteralNode(3));
            Assert.Equal(0.0, node.Accept(_visitor, _table));
        }

        #endregion

        // =====================================================================
        //  ExponentiationNode Tests
        // =====================================================================

        #region ExponentiationNode Tests

        [Fact]
        public void Exponentiation_IntToInt()
        {
            var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(10));
            Assert.Equal(1024.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Exponentiation_ZeroExponent()
        {
            var node = new ExponentiationNode(new LiteralNode(5), new LiteralNode(0));
            Assert.Equal(1.0, node.Accept(_visitor, _table));
        }

        [Fact]
        public void Exponentiation_NegativeExponent()
        {
            var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(-1));
            Assert.Equal(0.5, (double)node.Accept(_visitor, _table), precision: 10);
        }

        [Fact]
        public void Exponentiation_DoubleBase()
        {
            var node = new ExponentiationNode(new LiteralNode(4.0), new LiteralNode(0.5));
            var result = (double)node.Accept(_visitor, _table);
            Assert.InRange(result, 1.99, 2.01);
        }

        #endregion

        // =====================================================================
        //  AssignmentStmt Tests
        // =====================================================================

        #region AssignmentStmt Tests

        [Fact]
        public void Assignment_StoresValue()
        {
            var stmt = new AssignmentStmt(new VariableNode("y"), new LiteralNode(42));
            stmt.Accept(_visitor, _table);
            Assert.Equal(42, _table["y"]);
        }

        [Fact]
        public void Assignment_OverwritesExisting()
        {
            _table["z"] = 1;
            var stmt = new AssignmentStmt(new VariableNode("z"), new LiteralNode(999));
            stmt.Accept(_visitor, _table);
            Assert.Equal(999, _table["z"]);
        }

        [Fact]
        public void Assignment_EvaluatesExpression()
        {
            var stmt = new AssignmentStmt(new VariableNode("z"),
                new PlusNode(new LiteralNode(3), new LiteralNode(4)));
            stmt.Accept(_visitor, _table);
            Assert.Equal(7.0, _table["z"]);
        }

        #endregion

        // =====================================================================
        //  Nested Expression Evaluation
        // =====================================================================

        #region Nested Expression Evaluation

        [Fact]
        public void Nested_AddThenMultiply()
        {
            // (2 + 3) * 4 = 20.0
            var add = new PlusNode(new LiteralNode(2), new LiteralNode(3));
            var mul = new TimesNode(add, new LiteralNode(4));
            Assert.Equal(20.0, mul.Accept(_visitor, _table));
        }

        [Fact]
        public void Nested_AllOperators()
        {
            // ((10 + 2) - (3 * 2)) = 6.0
            var add = new PlusNode(new LiteralNode(10), new LiteralNode(2));
            var mul = new TimesNode(new LiteralNode(3), new LiteralNode(2));
            var sub = new MinusNode(add, mul);
            Assert.Equal(6.0, sub.Accept(_visitor, _table));
        }

        #endregion
    }
}
