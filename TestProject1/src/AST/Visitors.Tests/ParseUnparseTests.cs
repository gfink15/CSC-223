// =============================================================================
// Parse → Unparse Round-Trip Test Suite
// Verifies that parsing source code into an AST then unparsing it via the
// UnparseVisitor produces the expected canonical output, and that the
// canonical output is idempotent (unparse ∘ parse ∘ unparse ∘ parse == unparse ∘ parse).
//
// Bugs: None known.
//
// @author Graham Fink, Mridul Agrawal
// @date   4/1/2026
// =============================================================================

using Xunit;
using AST;
using Parser;
using Utilities.Containers;

namespace AST.Visitors.Tests
{
    public class ParseUnparseTests
    {
        private readonly UnparseVisitor _visitor = new UnparseVisitor();

        // =========================================================================
        //  Helper — Parse a program string and unparse using the visitor
        // =========================================================================

        /// <summary>
        /// Parses the input program string into an AST and then unparses it
        /// using the UnparseVisitor to produce a canonical string.
        /// </summary>
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
            string unparsed = ParseThenUnparse(input);

            // The parser strips the redundant parens around a single literal
            string expected =
                "{\n" +
                "    x := (42)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_SingleIntReturn()
        {
            string input = "{\n  return (42)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    return (42)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_SingleVariableReturn()
        {
            string input = "{\n  x := (1)\n  return (x)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (1)\n" +
                "    return (x)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Binary Expression Programs — All Operators

        [Fact]
        public void RoundTrip_PlusExpression()
        {
            string input = "{\n  x := (1 + 2)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (1 + 2)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_MinusExpression()
        {
            string input = "{\n  x := (10 - 3)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (10 - 3)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_TimesExpression()
        {
            string input = "{\n  x := (4 * 5)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (4 * 5)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_FloatDivExpression()
        {
            string input = "{\n  x := (8 / 2)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (8 / 2)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_IntDivExpression()
        {
            string input = "{\n  x := (9 // 2)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (9 // 2)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_ModulusExpression()
        {
            string input = "{\n  x := (10 % 3)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (10 % 3)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_ExponentiationExpression()
        {
            string input = "{\n  x := (2 ** 8)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (2 ** 8)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Nested Expressions

        [Fact]
        public void RoundTrip_NestedArithmetic()
        {
            string input = "{\n  x := ((1 + 2) * (3 - 4))\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := ((1 + 2) * (3 - 4))\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_DeeplyNestedExpression()
        {
            string input = "{\n  x := (((1 + 2) * 3) - (4 / 5))\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (((1 + 2) * 3) - (4 / 5))\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_VariableExpression()
        {
            string input = "{\n  a := (1)\n  b := (2)\n  c := (a + b)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (1)\n" +
                "    b := (2)\n" +
                "    c := (a + b)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Multiple Statements

        [Fact]
        public void RoundTrip_MultipleAssignmentsAndReturn()
        {
            string input = "{\n  a := (5)\n  b := (10)\n  c := (a + b)\n  return (c)\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (5)\n" +
                "    b := (10)\n" +
                "    c := (a + b)\n" +
                "    return (c)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_AllOperatorsInOneProgram()
        {
            string input =
                "{\n" +
                "  a := (5 + 3)\n" +
                "  b := (10 - 2)\n" +
                "  c := (4 * 6)\n" +
                "  d := (8 / 2)\n" +
                "  e := (9 // 2)\n" +
                "  f := (10 % 3)\n" +
                "  g := (2 ** 3)\n" +
                "  return (a)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (5 + 3)\n" +
                "    b := (10 - 2)\n" +
                "    c := (4 * 6)\n" +
                "    d := (8 / 2)\n" +
                "    e := (9 // 2)\n" +
                "    f := (10 % 3)\n" +
                "    g := (2 ** 3)\n" +
                "    return (a)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Nested Blocks

        [Fact]
        public void RoundTrip_SingleNestedBlock()
        {
            string input =
                "{\n" +
                "  x := (10)\n" +
                "  {\n" +
                "    y := (x + 5)\n" +
                "  }\n" +
                "  return (x)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (10)\n" +
                "    {\n" +
                "        y := (x + 5)\n" +
                "    }\n" +
                "    return (x)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_DoubleNestedBlock()
        {
            string input =
                "{\n" +
                "  a := (1)\n" +
                "  {\n" +
                "    b := (2)\n" +
                "    {\n" +
                "      c := (3)\n" +
                "    }\n" +
                "  }\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (1)\n" +
                "    {\n" +
                "        b := (2)\n" +
                "        {\n" +
                "            c := (3)\n" +
                "        }\n" +
                "    }\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_ParallelBlocks()
        {
            string input =
                "{\n" +
                "  a := (1)\n" +
                "  {\n" +
                "    b := (2)\n" +
                "  }\n" +
                "  {\n" +
                "    c := (3)\n" +
                "  }\n" +
                "  return (a)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (1)\n" +
                "    {\n" +
                "        b := (2)\n" +
                "    }\n" +
                "    {\n" +
                "        c := (3)\n" +
                "    }\n" +
                "    return (a)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_TripleNestedBlocks()
        {
            string input =
                "{\n" +
                "  a := (1)\n" +
                "  {\n" +
                "    b := (2)\n" +
                "    {\n" +
                "      c := (3)\n" +
                "      {\n" +
                "        d := (4)\n" +
                "      }\n" +
                "    }\n" +
                "  }\n" +
                "  return (a)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (1)\n" +
                "    {\n" +
                "        b := (2)\n" +
                "        {\n" +
                "            c := (3)\n" +
                "            {\n" +
                "                d := (4)\n" +
                "            }\n" +
                "        }\n" +
                "    }\n" +
                "    return (a)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Block with Complex Expressions

        [Fact]
        public void RoundTrip_NestedBlockWithComplexExpressions()
        {
            string input =
                "{\n" +
                "  x := (10)\n" +
                "  {\n" +
                "    y := ((x + 1) * (x - 1))\n" +
                "    return (y)\n" +
                "  }\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (10)\n" +
                "    {\n" +
                "        y := ((x + 1) * (x - 1))\n" +
                "        return (y)\n" +
                "    }\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_ComplexNestedExpressionProgram()
        {
            string input =
                "{\n" +
                "  a := (5)\n" +
                "  b := (10)\n" +
                "  c := ((a + b) * (b - a))\n" +
                "  d := (((c + 1) ** 2) // 10)\n" +
                "  return (d)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    a := (5)\n" +
                "    b := (10)\n" +
                "    c := ((a + b) * (b - a))\n" +
                "    d := (((c + 1) ** 2) // 10)\n" +
                "    return (d)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Idempotency Tests

        [Theory]
        [InlineData("{\n  x := (42)\n}")]
        [InlineData("{\n  return (42)\n}")]
        [InlineData("{\n  x := (1 + 2)\n}")]
        [InlineData("{\n  x := ((1 + 2) * 3)\n}")]
        [InlineData("{\n  a := (1)\n  b := (2)\n  return (a + b)\n}")]
        public void RoundTrip_Idempotent_SimplePrograms(string input)
        {
            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        [Fact]
        public void RoundTrip_Idempotent_NestedBlocks()
        {
            string input =
                "{\n" +
                "  x := (10)\n" +
                "  {\n" +
                "    y := (x + 5)\n" +
                "    {\n" +
                "      z := (y * 2)\n" +
                "      return (z)\n" +
                "    }\n" +
                "  }\n" +
                "}";

            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        [Fact]
        public void RoundTrip_Idempotent_AllOperators()
        {
            string input =
                "{\n" +
                "  a := (5 + 3)\n" +
                "  b := (10 - 2)\n" +
                "  c := (4 * 6)\n" +
                "  d := (8 / 2)\n" +
                "  e := (9 // 2)\n" +
                "  f := (10 % 3)\n" +
                "  g := (2 ** 3)\n" +
                "  return (a)\n" +
                "}";

            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        [Fact]
        public void RoundTrip_Idempotent_ParallelBlocks()
        {
            string input =
                "{\n" +
                "  a := (1)\n" +
                "  {\n" +
                "    b := (2)\n" +
                "  }\n" +
                "  {\n" +
                "    c := (3)\n" +
                "  }\n" +
                "  return (a)\n" +
                "}";

            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        [Fact]
        public void RoundTrip_Idempotent_ComplexNestedExpressions()
        {
            string input =
                "{\n" +
                "  a := (5)\n" +
                "  b := (10)\n" +
                "  c := ((a + b) * (b - a))\n" +
                "  d := (((c + 1) ** 2) // 10)\n" +
                "  return (d)\n" +
                "}";

            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        #endregion

        #region Empty / Minimal Programs

        [Fact]
        public void RoundTrip_EmptyBlock()
        {
            string input = "{\n}";
            string unparsed = ParseThenUnparse(input);

            Assert.Equal("{\n}", unparsed);
        }

        [Fact]
        public void RoundTrip_EmptyBlock_Idempotent()
        {
            string input = "{\n}";
            string firstPass = ParseThenUnparse(input);
            string secondPass = ParseThenUnparse(firstPass);

            Assert.Equal(firstPass, secondPass);
        }

        [Fact]
        public void RoundTrip_EmptyNestedBlock()
        {
            string input = "{\n  {\n  }\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    {\n" +
                "    }\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Mixed Statements and Nested Blocks

        [Fact]
        public void RoundTrip_StatementsBeforeAndAfterBlock()
        {
            string input =
                "{\n" +
                "  x := (1)\n" +
                "  {\n" +
                "    y := (x + 1)\n" +
                "  }\n" +
                "  z := (x + 2)\n" +
                "  return (z)\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    x := (1)\n" +
                "    {\n" +
                "        y := (x + 1)\n" +
                "    }\n" +
                "    z := (x + 2)\n" +
                "    return (z)\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        [Fact]
        public void RoundTrip_ReturnWithNestedExpression()
        {
            string input = "{\n  return ((2 ** 3) + (4 * 5))\n}";
            string unparsed = ParseThenUnparse(input);

            string expected =
                "{\n" +
                "    return ((2 ** 3) + (4 * 5))\n" +
                "}";
            Assert.Equal(expected, unparsed);
        }

        #endregion

        #region Structural Properties of Unparsed Output

        [Fact]
        public void RoundTrip_CorrectBraceCount()
        {
            string input =
                "{\n" +
                "  {\n" +
                "    {\n" +
                "      x := (1)\n" +
                "    }\n" +
                "  }\n" +
                "}";
            string unparsed = ParseThenUnparse(input);

            int openBraces = unparsed.Count(c => c == '{');
            int closeBraces = unparsed.Count(c => c == '}');
            Assert.Equal(3, openBraces);
            Assert.Equal(3, closeBraces);
        }

        [Fact]
        public void RoundTrip_CorrectLineCount()
        {
            // A program with 3 statements in one block should have 5 lines:
            // { + stmt + stmt + stmt + }
            string input = "{\n  a := (1)\n  b := (2)\n  return (a + b)\n}";
            string unparsed = ParseThenUnparse(input);

            int lineCount = unparsed.Split('\n').Length;
            Assert.Equal(5, lineCount);
        }

        [Fact]
        public void RoundTrip_IndentationIsConsistent()
        {
            string input =
                "{\n" +
                "  a := (1)\n" +
                "  {\n" +
                "    b := (2)\n" +
                "    {\n" +
                "      c := (3)\n" +
                "    }\n" +
                "  }\n" +
                "}";
            string unparsed = ParseThenUnparse(input);
            string[] lines = unparsed.Split('\n');

            // Level 0: { and }
            Assert.Equal("{", lines[0]);
            Assert.Equal("}", lines[^1]);

            // Level 1 statements start with 4 spaces
            Assert.StartsWith("    a", lines[1]);

            // Level 2 statements start with 8 spaces
            Assert.StartsWith("        b", lines[3]);

            // Level 3 statements start with 12 spaces
            Assert.StartsWith("            c", lines[5]);
        }

        #endregion
    }
}