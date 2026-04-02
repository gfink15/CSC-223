// =============================================================================
// UnparseVisitor — Integration Tests (Parse -> Unparse Round-Trip)
// Parses DEC source strings into ASTs via the Parser, then unparses them
// with the UnparseVisitor and verifies canonical output and idempotency.
//
// Bugs: None known.
//
// @author Graham Fink, Mridul Agrawal
// @date   4/2/2026
// =============================================================================

using Xunit;
using AST;
using Parser;
using Utilities.Containers;

namespace AST.Visitors.Tests
{
    public class UnparseVisitorIntegrationTests
    {
        private readonly UnparseVisitor _visitor = new UnparseVisitor();

        private string ParseThenUnparse(string input)
        {
            BlockStmt ast = Parser.Parser.Parse(input);
            return ast.Accept(_visitor, 0);
        }

        #region Simple Literal Programs

        [Fact]
        public void RoundTrip_SingleIntAssignment()
        {
            string input = "{\n  x := (42)\n}";
            string expected = "{\n    x := (42)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_SingleReturn()
        {
            string input = "{\n  return (42)\n}";
            string expected = "{\n    return (42)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_AssignAndReturn()
        {
            string input = "{\n  x := (1)\n  return (x)\n}";
            string expected = "{\n    x := (1)\n    return (x)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }



        #endregion

        #region All 7 Operators

        [Theory]
        [InlineData("(1 + 2)")]
        [InlineData("(10 - 3)")]
        [InlineData("(4 * 5)")]
        [InlineData("(8 / 2)")]
        [InlineData("(9 // 2)")]
        [InlineData("(10 % 3)")]
        [InlineData("(2 ** 8)")]
        public void RoundTrip_EachOperator(string expr)
        {
            string input = "{\n  x := " + expr + "\n}";
            string expected = "{\n    x := " + expr + "\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_AllOperatorsInOneProgram()
        {
            string input =
                "{\n  a := (5 + 3)\n  b := (10 - 2)\n  c := (4 * 6)\n" +
                "  d := (8 / 2)\n  e := (9 // 2)\n  f := (10 % 3)\n" +
                "  g := (2 ** 3)\n  return (a)\n}";
            string expected =
                "{\n    a := (5 + 3)\n    b := (10 - 2)\n    c := (4 * 6)\n" +
                "    d := (8 / 2)\n    e := (9 // 2)\n    f := (10 % 3)\n" +
                "    g := (2 ** 3)\n    return (a)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        #endregion

        #region Nested Expressions

        [Fact]
        public void RoundTrip_NestedArithmetic()
        {
            string input = "{\n  x := ((1 + 2) * (3 - 4))\n}";
            string expected = "{\n    x := ((1 + 2) * (3 - 4))\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_DeeplyNested()
        {
            string input = "{\n  x := (((1 + 2) * 3) - (4 / 5))\n}";
            string expected = "{\n    x := (((1 + 2) * 3) - (4 / 5))\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_VariablesInExpressions()
        {
            string input = "{\n  a := (1)\n  b := (2)\n  c := (a + b)\n}";
            string expected = "{\n    a := (1)\n    b := (2)\n    c := (a + b)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        #endregion

        #region Nested Blocks

        [Fact]
        public void RoundTrip_SingleNestedBlock()
        {
            string input =
                "{\n  x := (10)\n  {\n    y := (x + 5)\n  }\n  return (x)\n}";
            string expected =
                "{\n    x := (10)\n    {\n        y := (x + 5)\n    }\n    return (x)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_DoubleNestedBlock()
        {
            string input =
                "{\n  a := (1)\n  {\n    b := (2)\n    {\n      c := (3)\n    }\n  }\n}";
            string expected =
                "{\n    a := (1)\n    {\n        b := (2)\n" +
                "        {\n            c := (3)\n        }\n    }\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_ParallelBlocks()
        {
            string input =
                "{\n  a := (1)\n  {\n    b := (2)\n  }\n  {\n    c := (3)\n  }\n  return (a)\n}";
            string expected =
                "{\n    a := (1)\n    {\n        b := (2)\n    }\n" +
                "    {\n        c := (3)\n    }\n    return (a)\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        [Fact]
        public void RoundTrip_EmptyBlock()
        {
            Assert.Equal("{\n}", ParseThenUnparse("{\n}"));
        }

        [Fact]
        public void RoundTrip_EmptyNestedBlock()
        {
            string input = "{\n  {\n  }\n}";
            string expected = "{\n    {\n    }\n}";
            Assert.Equal(expected, ParseThenUnparse(input));
        }

        #endregion

        #region Idempotency Tests

        [Theory]
        [InlineData("{\n  x := (42)\n}")]
        [InlineData("{\n  return (42)\n}")]
        [InlineData("{\n  x := (1 + 2)\n}")]
        [InlineData("{\n  x := ((1 + 2) * 3)\n}")]
        [InlineData("{\n  a := (1)\n  b := (2)\n  return (a + b)\n}")]
        public void Idempotent_SimplePrograms(string input)
        {
            string first = ParseThenUnparse(input);
            string second = ParseThenUnparse(first);
            Assert.Equal(first, second);
        }

        [Fact]
        public void Idempotent_NestedBlocks()
        {
            string input =
                "{\n  x := (10)\n  {\n    y := (x + 5)\n    {\n" +
                "      z := (y * 2)\n      return (z)\n    }\n  }\n}";
            string first = ParseThenUnparse(input);
            string second = ParseThenUnparse(first);
            Assert.Equal(first, second);
        }

        [Fact]
        public void Idempotent_AllOperators()
        {
            string input =
                "{\n  a := (5 + 3)\n  b := (10 - 2)\n  c := (4 * 6)\n" +
                "  d := (8 / 2)\n  e := (9 // 2)\n  f := (10 % 3)\n" +
                "  g := (2 ** 3)\n  return (a)\n}";
            string first = ParseThenUnparse(input);
            string second = ParseThenUnparse(first);
            Assert.Equal(first, second);
        }

        #endregion

        #region Structural Properties

        [Fact]
        public void Structure_CorrectBraceCount()
        {
            string input = "{\n  {\n    {\n      x := (1)\n    }\n  }\n}";
            string unparsed = ParseThenUnparse(input);
            Assert.Equal(3, unparsed.Count(c => c == '{'));
            Assert.Equal(3, unparsed.Count(c => c == '}'));
        }

        [Fact]
        public void Structure_CorrectLineCount()
        {
            string input = "{\n  a := (1)\n  b := (2)\n  return (a + b)\n}";
            string unparsed = ParseThenUnparse(input);
            Assert.Equal(5, unparsed.Split('\n').Length);
        }

        [Fact]
        public void Structure_IndentationConsistent()
        {
            string input =
                "{\n  a := (1)\n  {\n    b := (2)\n    {\n      c := (3)\n    }\n  }\n}";
            string unparsed = ParseThenUnparse(input);
            string[] lines = unparsed.Split('\n');
            Assert.Equal("{", lines[0]);
            Assert.Equal("}", lines[^1]);
            Assert.StartsWith("    a", lines[1]);
            Assert.StartsWith("        b", lines[3]);
            Assert.StartsWith("            c", lines[5]);
        }

        #endregion
    }
}
