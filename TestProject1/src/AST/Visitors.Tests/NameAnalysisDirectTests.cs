// =============================================================================
// NameAnalysisVisitor — Direct Unit Tests
// Manually constructs AST nodes and calls Accept to verify each Visit method
// correctly identifies declared/undeclared variables in isolation.
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
    public class NameAnalysisDirectTests
    {
        private Tuple<SymbolTable<string, object>, Statement> MakeTuple(SymbolTable<string, object> scope)
        {
            var dummy = new ReturnStmt(new LiteralNode(0));
            return new Tuple<SymbolTable<string, object>, Statement>(scope, dummy);
        }

        private Tuple<SymbolTable<string, object>, Statement> MakeTuple(
            SymbolTable<string, object> scope, Statement stmt)
        {
            return new Tuple<SymbolTable<string, object>, Statement>(scope, stmt);
        }

        // =====================================================================
        //  Literal Node Tests
        // =====================================================================

        #region Literal Node Tests

        [Fact]
        public void Literal_Int_AlwaysPasses()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new LiteralNode(42).Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Literal_Double_AlwaysPasses()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new LiteralNode(3.14).Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Literal_String_AlwaysPasses()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new LiteralNode("hello").Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Literal_Zero_AlwaysPasses()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new LiteralNode(0).Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Literal_Negative_AlwaysPasses()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new LiteralNode(-99).Accept(visitor, MakeTuple(scope)));
        }

        #endregion

        // =====================================================================
        //  Variable Node Tests
        // =====================================================================

        #region Variable Node Tests

        [Fact]
        public void Variable_DeclaredInScope_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["x"] = 10;
            Assert.True(new VariableNode("x").Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Variable_DeclaredInParentScope_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var parent = new SymbolTable<string, object>();
            parent["x"] = 5;
            var child = new SymbolTable<string, object>(parent);
            Assert.True(new VariableNode("x").Accept(visitor, MakeTuple(child)));
        }

        [Fact]
        public void Variable_DeclaredInGrandparentScope_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var gp = new SymbolTable<string, object>();
            gp["x"] = 1;
            var parent = new SymbolTable<string, object>(gp);
            var child = new SymbolTable<string, object>(parent);
            Assert.True(new VariableNode("x").Accept(visitor, MakeTuple(child)));
        }

        [Fact]
        public void Variable_Undeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.False(new VariableNode("y").Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Variable_NotInAnyScope_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var parent = new SymbolTable<string, object>();
            var child = new SymbolTable<string, object>(parent);
            Assert.False(new VariableNode("missing").Accept(visitor, MakeTuple(child)));
        }

        [Fact]
        public void Variable_MultipleVarsDeclared_PassForEach()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0;
            scope["b"] = 0;
            scope["c"] = 0;
            Assert.True(new VariableNode("a").Accept(visitor, MakeTuple(scope)));
            Assert.True(new VariableNode("b").Accept(visitor, MakeTuple(scope)));
            Assert.True(new VariableNode("c").Accept(visitor, MakeTuple(scope)));
        }

        #endregion

        // =====================================================================
        //  Binary Operator Tests — All 7 Operators
        // =====================================================================

        #region Binary Operator Tests — Both Declared

        [Fact]
        public void Plus_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0; scope["b"] = 0;
            var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            Assert.True(node.Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Minus_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["x"] = 0; scope["y"] = 0;
            Assert.True(new MinusNode(new VariableNode("x"), new VariableNode("y"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Times_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["m"] = 0; scope["n"] = 0;
            Assert.True(new TimesNode(new VariableNode("m"), new VariableNode("n"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void FloatDiv_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["p"] = 0; scope["q"] = 0;
            Assert.True(new FloatDivNode(new VariableNode("p"), new VariableNode("q"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void IntDiv_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["p"] = 0; scope["q"] = 0;
            Assert.True(new IntDivNode(new VariableNode("p"), new VariableNode("q"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Modulus_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0; scope["b"] = 0;
            Assert.True(new ModulusNode(new VariableNode("a"), new VariableNode("b"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Exponentiation_BothDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["base"] = 0; scope["exp"] = 0;
            Assert.True(new ExponentiationNode(new VariableNode("base"), new VariableNode("exp"))
                .Accept(visitor, MakeTuple(scope)));
        }

        #endregion

        #region Binary Operator Tests — Undeclared

        [Fact]
        public void Plus_LeftUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["b"] = 0;
            Assert.False(new PlusNode(new VariableNode("a"), new VariableNode("b"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Plus_RightUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0;
            Assert.False(new PlusNode(new VariableNode("a"), new VariableNode("b"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Plus_BothUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.False(new PlusNode(new VariableNode("x"), new VariableNode("y"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Times_LeftUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["b"] = 0;
            Assert.False(new TimesNode(new VariableNode("a"), new VariableNode("b"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void Modulus_RightUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0;
            Assert.False(new ModulusNode(new VariableNode("a"), new VariableNode("b"))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void NestedExpr_DeepUndeclared_Fails()
        {
            // ((a + b) * c) where c is undeclared
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0; scope["b"] = 0;
            var plus = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var times = new TimesNode(plus, new VariableNode("c"));
            Assert.False(times.Accept(visitor, MakeTuple(scope)));
        }

        #endregion

        #region Binary Operators with Literals (always pass)

        [Fact]
        public void BinaryExpr_AllLiterals_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            Assert.True(new PlusNode(new LiteralNode(1), new LiteralNode(2))
                .Accept(visitor, MakeTuple(scope)));
        }

        [Fact]
        public void BinaryExpr_MixedLiteralAndDeclaredVar_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["x"] = 0;
            Assert.True(new TimesNode(new VariableNode("x"), new LiteralNode(5))
                .Accept(visitor, MakeTuple(scope)));
        }

        #endregion

        // =====================================================================
        //  AssignmentStmt Tests
        // =====================================================================

        #region AssignmentStmt Tests

        [Fact]
        public void Assignment_LiteralRHS_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(5));
            Assert.True(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Assignment_RegistersVariable()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(5));
            stmt.Accept(visitor, MakeTuple(scope, stmt));
            Assert.True(scope.ContainsKey("x"));
        }

        [Fact]
        public void Assignment_DeclaredVarInRHS_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0; scope["b"] = 0;
            var expr = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var stmt = new AssignmentStmt(new VariableNode("c"), expr);
            Assert.True(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Assignment_UndeclaredVarInRHS_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new AssignmentStmt(new VariableNode("y"), new VariableNode("x"));
            Assert.False(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Assignment_UndeclaredVarInRHS_DoesNotRegisterLHS()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new AssignmentStmt(new VariableNode("y"), new VariableNode("x"));
            stmt.Accept(visitor, MakeTuple(scope, stmt));
            Assert.False(scope.ContainsKey("y"));
        }

        [Fact]
        public void Assignment_ChainedDeclarations_Pass()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var s1 = new AssignmentStmt(new VariableNode("x"), new LiteralNode(1));
            Assert.True(s1.Accept(visitor, MakeTuple(scope, s1)));
            var s2 = new AssignmentStmt(new VariableNode("y"), new VariableNode("x"));
            Assert.True(s2.Accept(visitor, MakeTuple(scope, s2)));
        }

        [Fact]
        public void Assignment_UndeclaredInNestedExpr_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0;
            var expr = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            var stmt = new AssignmentStmt(new VariableNode("z"), expr);
            Assert.False(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        #endregion

        // =====================================================================
        //  ReturnStmt Tests
        // =====================================================================

        #region ReturnStmt Tests

        [Fact]
        public void Return_LiteralExpr_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new ReturnStmt(new LiteralNode(42));
            Assert.True(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Return_DeclaredVariable_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["x"] = 10;
            var stmt = new ReturnStmt(new VariableNode("x"));
            Assert.True(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Return_UndeclaredVariable_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            var stmt = new ReturnStmt(new VariableNode("z"));
            Assert.False(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Return_ExprWithDeclaredVars_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0; scope["b"] = 0;
            var stmt = new ReturnStmt(new PlusNode(new VariableNode("a"), new VariableNode("b")));
            Assert.True(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        [Fact]
        public void Return_ExprWithUndeclaredVar_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var scope = new SymbolTable<string, object>();
            scope["a"] = 0;
            var stmt = new ReturnStmt(
                new PlusNode(new VariableNode("a"), new VariableNode("missing")));
            Assert.False(stmt.Accept(visitor, MakeTuple(scope, stmt)));
        }

        #endregion

        // =====================================================================
        //  BlockStmt Tests
        // =====================================================================

        #region BlockStmt Tests — Valid

        [Fact]
        public void Block_Empty_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            Assert.True(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_SingleLiteralAssignment_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(5)));
            Assert.True(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_AssignThenReturn_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(10)));
            block.Add(new ReturnStmt(new VariableNode("x")));
            Assert.True(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_MultipleAssignmentsThenReturn_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(1)));
            block.Add(new AssignmentStmt(new VariableNode("b"), new LiteralNode(2)));
            block.Add(new AssignmentStmt(new VariableNode("c"),
                new PlusNode(new VariableNode("a"), new VariableNode("b"))));
            block.Add(new ReturnStmt(new VariableNode("c")));
            Assert.True(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_UsesParentScopeVariable_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            ps["x"] = 5;
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new ReturnStmt(new VariableNode("x")));
            Assert.True(block.Accept(visitor, MakeTuple(ps, block)));
        }

        #endregion

        #region BlockStmt Tests — Invalid

        [Fact]
        public void Block_UseBeforeDeclaration_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new AssignmentStmt(new VariableNode("y"), new VariableNode("x")));
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            Assert.False(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_ReturnUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new ReturnStmt(new VariableNode("z")));
            Assert.False(block.Accept(visitor, MakeTuple(ps, block)));
        }

        [Fact]
        public void Block_UndeclaredInExpression_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var block = new BlockStmt(new SymbolTable<string, object>(ps));
            block.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            block.Add(new AssignmentStmt(new VariableNode("y"),
                new PlusNode(new VariableNode("x"), new VariableNode("missing"))));
            Assert.False(block.Accept(visitor, MakeTuple(ps, block)));
        }

        #endregion

        // =====================================================================
        //  Nested Block / Scoping Tests
        // =====================================================================

        #region Nested Block Tests

        [Fact]
        public void NestedBlock_InnerUsesOuterVar_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var outerST = new SymbolTable<string, object>(ps);

            var inner = new BlockStmt(new SymbolTable<string, object>(outerST));
            inner.Add(new AssignmentStmt(new VariableNode("y"), new VariableNode("x")));

            var outer = new BlockStmt(outerST);
            outer.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            outer.Add(inner);

            Assert.True(outer.Accept(visitor, MakeTuple(ps, outer)));
        }

        [Fact]
        public void NestedBlock_InnerDeclaresThenReturns_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var outerST = new SymbolTable<string, object>(ps);

            var inner = new BlockStmt(new SymbolTable<string, object>(outerST));
            inner.Add(new AssignmentStmt(new VariableNode("y"),
                new PlusNode(new VariableNode("x"), new LiteralNode(1))));
            inner.Add(new ReturnStmt(new VariableNode("y")));

            var outer = new BlockStmt(outerST);
            outer.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            outer.Add(inner);

            Assert.True(outer.Accept(visitor, MakeTuple(ps, outer)));
        }

        [Fact]
        public void NestedBlock_InnerUsesUndeclaredVar_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var outerST = new SymbolTable<string, object>(ps);

            var inner = new BlockStmt(new SymbolTable<string, object>(outerST));
            inner.Add(new AssignmentStmt(new VariableNode("y"), new VariableNode("z")));

            var outer = new BlockStmt(outerST);
            outer.Add(new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)));
            outer.Add(inner);

            Assert.False(outer.Accept(visitor, MakeTuple(ps, outer)));
        }

        [Fact]
        public void TripleNestedBlock_AllDeclared_Passes()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var outerST = new SymbolTable<string, object>(ps);
            var middleST = new SymbolTable<string, object>(outerST);
            var innST = new SymbolTable<string, object>(middleST);

            var innermost = new BlockStmt(innST);
            innermost.Add(new AssignmentStmt(new VariableNode("c"),
                new PlusNode(new VariableNode("a"), new VariableNode("b"))));
            innermost.Add(new ReturnStmt(new VariableNode("c")));

            var middle = new BlockStmt(middleST);
            middle.Add(new AssignmentStmt(new VariableNode("b"), new VariableNode("a")));
            middle.Add(innermost);

            var outer = new BlockStmt(outerST);
            outer.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(1)));
            outer.Add(middle);

            Assert.True(outer.Accept(visitor, MakeTuple(ps, outer)));
        }

        [Fact]
        public void TripleNestedBlock_DeepUndeclared_Fails()
        {
            var visitor = new NameAnalysisVisitor();
            var ps = new SymbolTable<string, object>();
            var outerST = new SymbolTable<string, object>(ps);
            var middleST = new SymbolTable<string, object>(outerST);
            var innST = new SymbolTable<string, object>(middleST);

            var innermost = new BlockStmt(innST);
            innermost.Add(new AssignmentStmt(new VariableNode("c"),
                new PlusNode(new VariableNode("a"), new VariableNode("missing"))));

            var middle = new BlockStmt(middleST);
            middle.Add(new AssignmentStmt(new VariableNode("b"), new VariableNode("a")));
            middle.Add(innermost);

            var outer = new BlockStmt(outerST);
            outer.Add(new AssignmentStmt(new VariableNode("a"), new LiteralNode(1)));
            outer.Add(middle);

            Assert.False(outer.Accept(visitor, MakeTuple(ps, outer)));
        }

        #endregion
    }
}
