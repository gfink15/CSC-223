// =============================================================================
// NameAnalysisVisitor — Integration Tests
// Parses DEC source strings into ASTs via the Parser, then runs the
// NameAnalysisVisitor on the resulting BlockStmt to verify correct
// declaration checking across nested scopes.
// Also verifies that the visitor's ErrorStatements and ErrorMessages
// lists are populated correctly for invalid programs.
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
        /// Parses the program, runs name analysis, and returns the visitor
        /// so tests can inspect both the pass/fail result and the error lists.
        /// </summary>
        private (bool result, NameAnalysisVisitor visitor) ParseAndAnalyze(string program)
        {
            var visitor = new NameAnalysisVisitor();
            BlockStmt ast = Parser.Parser.Parse(program);
            var scope = new SymbolTable<string, object>();
            var tuple = new Tuple<SymbolTable<string, object>, Statement>(scope, ast);
            bool result = ast.Accept(visitor, tuple);
            return (result, visitor);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
        }

        [Fact]
        public void Valid_LiteralOnly()
        {
            string program = @"{
                return (42)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
        }

        [Fact]
        public void Valid_EmptyBlock()
        {
            string program = "{\n}";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
        }

        #endregion

        // =====================================================================
        //  Invalid Programs — Should Fail with Error Details
        // =====================================================================

        #region Invalid Programs

        [Fact]
        public void Invalid_UndeclaredInReturn()
        {
            string program = @"{
                return (x)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Single(visitor.ErrorStatements);
            Assert.Single(visitor.ErrorMessages);
            Assert.IsType<ReturnStmt>(visitor.ErrorStatements[0]);
            Assert.Contains("x", visitor.ErrorMessages[0]);
        }

        [Fact]
        public void Invalid_UndeclaredInExpression()
        {
            string program = @"{
                a := (1)
                b := (a + missing)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Single(visitor.ErrorStatements);
            Assert.Single(visitor.ErrorMessages);
            Assert.IsType<AssignmentStmt>(visitor.ErrorStatements[0]);
            Assert.Contains("missing", visitor.ErrorMessages[0]);
        }

        [Fact]
        public void Invalid_ComplexExprWithUndeclared()
        {
            string program = @"{
                a := (1)
                b := (((a + c) * 2) - 1)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Single(visitor.ErrorStatements);
            Assert.Single(visitor.ErrorMessages);
            Assert.IsType<AssignmentStmt>(visitor.ErrorStatements[0]);
            Assert.Contains("c", visitor.ErrorMessages[0]);
        }

        [Fact]
        public void Invalid_AllUndeclared()
        {
            // x and y are both undeclared on the RHS of z := (x + y)
            string program = @"{
                z := (x + y)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            // Both x and y should produce an error
            Assert.Equal(2, visitor.ErrorStatements.Count);
            Assert.Equal(2, visitor.ErrorMessages.Count);
            Assert.Contains("x", visitor.ErrorMessages[0]);
            Assert.Contains("y", visitor.ErrorMessages[1]);
        }

        [Fact]
        public void Invalid_MultipleStatementsWithErrors()
        {
            // Two separate statements each use an undeclared variable
            string program = @"{
                a := (foo)
                b := (bar)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            // Each statement has one undeclared variable
            Assert.Equal(2, visitor.ErrorStatements.Count);
            Assert.Equal(2, visitor.ErrorMessages.Count);
            Assert.Contains("foo", visitor.ErrorMessages[0]);
            Assert.Contains("bar", visitor.ErrorMessages[1]);
        }

        [Fact]
        public void Invalid_ErrorMessageContainsVariableName()
        {
            string program = @"{
                return (nope)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            // Verify the error message mentions the undeclared variable name
            Assert.Contains("nope", visitor.ErrorMessages[0]);
            // Verify it mentions "not found" context
            Assert.Contains("not found", visitor.ErrorMessages[0]);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Single(visitor.ErrorStatements);
            Assert.Single(visitor.ErrorMessages);
            Assert.Contains("z", visitor.ErrorMessages[0]);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Single(visitor.ErrorStatements);
            Assert.Single(visitor.ErrorMessages);
            Assert.Contains("missing", visitor.ErrorMessages[0]);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
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
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
        }

        [Fact]
        public void Complex_ReassignedVariable()
        {
            string program = @"{
                a := (1)
                a := (2)
                return (a)
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.True(result);
            Assert.Empty(visitor.ErrorStatements);
            Assert.Empty(visitor.ErrorMessages);
        }

        [Fact]
        public void Complex_ErrorsInNestedAndOuterScope()
        {
            // Both scopes have undeclared variables
            string program = @"{
                a := (oops)
                {
                    b := (nah)
                }
            }";
            var (result, visitor) = ParseAndAnalyze(program);
            Assert.False(result);
            Assert.Equal(2, visitor.ErrorStatements.Count);
            Assert.Equal(2, visitor.ErrorMessages.Count);
            Assert.Contains("oops", visitor.ErrorMessages[0]);
            Assert.Contains("nah", visitor.ErrorMessages[1]);
        }

        #endregion
    }
}
