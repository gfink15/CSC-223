// =============================================================================
// NameAnalysisVisitor — Integration Tests
// Parses DEC source strings into ASTs via the Parser, then runs the
// NameAnalysisVisitor on the resulting BlockStmt to verify correct
// declaration checking across nested scopes.
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
    public class NameAnalysisIntegrationTests
    {
        /// <summary>
        /// Parses the program and runs name analysis.
        /// Returns true if no undeclared variables are found.
        /// </summary>
        private bool ParseAndAnalyze(string program)
        {
            var visitor = new NameAnalysisVisitor();
            BlockStmt ast = Parser.Parser.Parse(program);
            var scope = new SymbolTable<string, object>();
            var tuple = new Tuple<SymbolTable<string, object>, Statement>(scope, ast);
            return ast.Accept(visitor, tuple);
        }

        // =====================================================================
        //  Valid Programs — Should Pass
        // =====================================================================

        #region Valid Programs

        [Fact]
        public void Valid_SimpleAssignAndReturn()
        {
            string program = @"{
                x := (10)
                return (x)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_MultipleAssignments()
        {
            string program = @"{
                a := (1)
                b := (2)
                c := (a + b)
                return (c)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_LiteralOnly()
        {
            string program = @"{
                return (42)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_AllOperators()
        {
            string program = @"{
                a := (1)
                b := (2)
                c := (a + b)
                d := (a - b)
                e := (a * b)
                f := (a / b)
                g := (a // b)
                h := (a % b)
                i := (a ** b)
                return (i)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_ChainedComputations()
        {
            string program = @"{
                x := (5)
                y := (x + 1)
                z := (y * 2)
                return (z)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_NestedExprAllDeclared()
        {
            string program = @"{
                a := (1)
                b := (2)
                c := (3)
                d := (((a + b) * c) - (a ** b))
                return (d)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Valid_EmptyBlock()
        {
            string program = "{\n}";
            Assert.True(ParseAndAnalyze(program));
        }

        #endregion

        // =====================================================================
        //  Invalid Programs — Should Fail
        // =====================================================================

        #region Invalid Programs

        [Fact]
        public void Invalid_UndeclaredInReturn()
        {
            string program = @"{
                return (x)
            }";
            Assert.False(ParseAndAnalyze(program));
        }



        [Fact]
        public void Invalid_UndeclaredInExpression()
        {
            string program = @"{
                a := (1)
                b := (a + missing)
            }";
            Assert.False(ParseAndAnalyze(program));
        }

        [Fact]
        public void Invalid_ComplexExprWithUndeclared()
        {
            string program = @"{
                a := (1)
                b := (((a + c) * 2) - 1)
            }";
            Assert.False(ParseAndAnalyze(program));
        }

        [Fact]
        public void Invalid_AllUndeclared()
        {
            string program = @"{
                z := (x + y)
            }";
            Assert.False(ParseAndAnalyze(program));
        }

        #endregion

        // =====================================================================
        //  Nested Scope Tests — Parser builds properly-parented symbol tables
        // =====================================================================

        #region Nested Scope Tests

        [Fact]
        public void Scope_InnerUsesOuterVariable_Passes()
        {
            string program = @"{
                x := (7)
                {
                    return (x)
                }
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_InnerDeclaresThenReturns_Passes()
        {
            string program = @"{
                x := (1)
                {
                    y := (x + 1)
                    return (y)
                }
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_InnerUsesUndeclaredVar_Fails()
        {
            string program = @"{
                x := (1)
                {
                    y := (z)
                }
            }";
            Assert.False(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_TripleNested_AllDeclared_Passes()
        {
            string program = @"{
                a := (1)
                {
                    b := (a + 1)
                    {
                        c := (a + b)
                        return (c)
                    }
                }
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_TripleNested_DeepUndeclared_Fails()
        {
            string program = @"{
                a := (1)
                {
                    b := (a)
                    {
                        c := (a + missing)
                    }
                }
            }";
            Assert.False(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_ParallelBlocks_BothValid()
        {
            string program = @"{
                x := (1)
                {
                    y := (x)
                }
                {
                    z := (x)
                }
                return (x)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Scope_ShadowedVariable()
        {
            string program = @"{
                x := (1)
                {
                    x := (2)
                }
                return (x)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        #endregion

        // =====================================================================
        //  Complex Integration Programs
        // =====================================================================

        #region Complex Programs

        [Fact]
        public void Complex_MultiScopeComputation()
        {
            string program = @"{
                a := (5)
                b := (10)
                c := ((a + b) * (b - a))
                {
                    d := (c + 1)
                    return (d)
                }
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Complex_StatementsAfterBlock()
        {
            string program = @"{
                x := (1)
                {
                    y := (x + 1)
                }
                z := (x + 2)
                return (z)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        [Fact]
        public void Complex_ReassignedVariable()
        {
            string program = @"{
                a := (1)
                a := (2)
                return (a)
            }";
            Assert.True(ParseAndAnalyze(program));
        }

        #endregion
    }
}
