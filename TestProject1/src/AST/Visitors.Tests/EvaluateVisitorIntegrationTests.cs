// =============================================================================
// EvaluateVisitor — Integration Tests
// Parses DEC source strings into ASTs via the Parser, then evaluates them
// with the EvaluateVisitor and verifies correct computation results.
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
    public class EvaluateVisitorIntegrationTests
    {
        private readonly EvaluateVisitor _evaluator = new EvaluateVisitor();

        private object Eval(string program)
        {
            BlockStmt ast = Parser.Parser.Parse(program);
            return _evaluator.Evaluate(ast);
        }

        // =====================================================================
        //  Basic Arithmetic — Each Operator
        // =====================================================================

        #region Basic Arithmetic

        [Fact]
        public void Eval_SimpleReturn()
        {
            Assert.Equal(5, Eval("{ return (2 + 3) }"));
        }

        [Theory]
        [InlineData("(10 + 5)", 15)]
        [InlineData("(10 - 5)", 5)]
        [InlineData("(10 * 5)", 50)]
        [InlineData("(10 // 3)", 3)]
        [InlineData("(10 % 3)", 1)]
        public void Eval_BasicArithmeticOps(string expr, int expected)
        {
            Assert.Equal(expected, Eval($"{{ return {expr} }}"));
        }

        [Fact]
        public void Eval_FloatDivision()
        {
            Assert.Equal(3.5, Eval("{ return (7 / 2) }"));
        }

        [Fact]
        public void Eval_Exponentiation()
        {
            Assert.Equal(256, Eval("{ return (2 ** 8) }"));
        }

        [Fact]
        public void Eval_NestedArithmetic()
        {
            // ((2 + 3) * (4 - 1)) = 15
            Assert.Equal(15, Eval("{ return ((2 + 3) * (4 - 1)) }"));
        }

        [Fact]
        public void Eval_DeepNesting()
        {
            // (((1 + 2) * 3) - 4) = 5
            Assert.Equal(5, Eval("{ return (((1 + 2) * 3) - 4) }"));
        }

        [Fact]
        public void Eval_ZeroResult()
        {
            Assert.Equal(0, Eval("{ return (5 - 5) }"));
        }

        [Fact]
        public void Eval_NegativeResult()
        {
            Assert.Equal(-3, Eval("{ return (2 - 5) }"));
        }

        #endregion

        // =====================================================================
        //  Variables
        // =====================================================================

        #region Variables

        [Fact]
        public void Eval_AssignAndReturn()
        {
            string program = @"{
                x := (10)
                return (x)
            }";
            Assert.Equal(10, Eval(program));
        }

        [Fact]
        public void Eval_MultipleAssignments()
        {
            string program = @"{
                x := (3)
                y := (4)
                return (x + y)
            }";
            Assert.Equal(7, Eval(program));
        }

        [Fact]
        public void Eval_ReassignVariable()
        {
            string program = @"{
                a := (1)
                a := (2)
                return (a)
            }";
            Assert.Equal(2, Eval(program));
        }

        [Fact]
        public void Eval_ChainedComputation()
        {
            string program = @"{
                x := (5)
                y := (x + 1)
                z := (y * 2)
                return (z)
            }";
            Assert.Equal(12, Eval(program));
        }

        [Fact]
        public void Eval_ComplexComputation()
        {
            string program = @"{
                a := (10)
                b := (3)
                c := (a - b)
                d := (c ** 2)
                return (d)
            }";
            Assert.Equal(49, Eval(program));
        }

        [Fact]
        public void Eval_SelfReferentialReassignment()
        {
            // x = x + 1: declare x, then reassign using its own value
            string program = @"{
                x := (5)
                x := (x + 1)
                return (x)
            }";
            Assert.Equal(6, Eval(program));
        }

        #endregion

        // =====================================================================
        //  Scope
        // =====================================================================

        #region Scope

        [Fact]
        public void Eval_InnerScopeDoesNotLeak()
        {
            string program = @"{
                x := (5)
                {
                    y := (10)
                }
                return (x)
            }";
            Assert.Equal(5, Eval(program));
        }

        [Fact]
        public void Eval_InnerScopeCanReadOuterVariable()
        {
            string program = @"{
                x := (7)
                {
                    return (x)
                }
            }";
            Assert.Equal(7, Eval(program));
        }

        [Fact]
        public void Eval_ShadowedVariable()
        {
            string program = @"{
                x := (1)
                {
                    x := (99)
                }
                return (x)
            }";
            Assert.Equal(1, Eval(program));
        }

        [Fact]
        public void Eval_InnerScopeComputesFromOuter()
        {
            string program = @"{
                a := (10)
                {
                    b := (a + 5)
                    return (b)
                }
            }";
            Assert.Equal(15, Eval(program));
        }

        #endregion

        // =====================================================================
        //  Return Semantics
        // =====================================================================

        #region Return Semantics

        [Fact]
        public void Eval_ReturnStopsExecution()
        {
            string program = @"{
                return (42)
                x := (999)
            }";
            Assert.Equal(42, Eval(program));
        }

        [Fact]
        public void Eval_NoReturnYieldsLastValue()
        {
            string program = @"{
                x := (3)
                y := (7)
            }";
            Assert.Equal(7, Eval(program));
        }

        [Fact]
        public void Eval_ReturnFromNestedBlockPropagates()
        {
            string program = @"{
                x := (1)
                {
                    return (55)
                }
                x := (2)
            }";
            Assert.Equal(55, Eval(program));
        }

        [Fact]
        public void Eval_ReturnExpressionEvaluated()
        {
            string program = @"{
                x := (10)
                return (x + 5)
            }";
            Assert.Equal(15, Eval(program));
        }

        #endregion

        // =====================================================================
        //  Division by Zero
        // =====================================================================

        #region Division by Zero

        [Fact]
        public void Eval_IntDivByZero_Throws()
        {
            string program = "{ return (10 // 0) }";
            BlockStmt ast = Parser.Parser.Parse(program);
            Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
        }

        [Fact]
        public void Eval_FloatDivByZero_Throws()
        {
            string program = "{ return (10 / 0) }";
            BlockStmt ast = Parser.Parser.Parse(program);
            Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
        }

        [Fact]
        public void Eval_ModByZero_Throws()
        {
            string program = "{ return (5 % 0) }";
            BlockStmt ast = Parser.Parser.Parse(program);
            Assert.Throws<EvaluationException>(() => _evaluator.Evaluate(ast));
        }

        #endregion

        // =====================================================================
        //  Mixed int / double Promotion
        // =====================================================================

        #region Mixed int/double

        [Fact]
        public void Eval_IntPlusDouble()
        {
            Assert.Equal(2.5, Eval("{ return (1 + 1.5) }"));
        }

        [Fact]
        public void Eval_ComplexMixed()
        {
            // (3 * 2.0) - (10 // 4) = 6.0 - 2 = 4.0
            Assert.Equal(4, Eval("{ return ((3 * 2.0) - (10 // 4)) }"));
        }

        #endregion

        // =====================================================================
        //  Complex Integration Programs
        // =====================================================================

        #region Complex Programs

        [Fact]
        public void Eval_MultiStepWithAllOperators()
        {
            string program = @"{
                a := (10)
                b := (3)
                c := (a + b)
                d := (a - b)
                e := (c * d)
                return (e)
            }";
            // c=13, d=7, e=91
            Assert.Equal(91, Eval(program));
        }

        [Fact]
        public void Eval_NestedScopeComputation()
        {
            string program = @"{
                x := (1)
                {
                    y := (x + 1)
                    {
                        z := (y * 2)
                        return (z)
                    }
                }
            }";
            Assert.Equal(4, Eval(program));
        }

        [Fact]
        public void Eval_ModulusAndIntDiv()
        {
            string program = @"{
                a := (17)
                b := (5)
                q := (a // b)
                r := (a % b)
                return (q + r)
            }";
            // q=3, r=2, q+r=5
            Assert.Equal(5, Eval(program));
        }

        #endregion
    }
}
