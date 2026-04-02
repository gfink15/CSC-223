using Xunit;
using AST;
using Utilities.Containers;

namespace AST.Visitors.Tests;

// =============================================================================
// NameAnalysisVisitorTest
//
// Direct unit tests — construct AST nodes manually and call Accept with the
// Tuple<SymbolTable<string, object>, Statement> parameter that the visitor expects.
//
// BlockStmt construction (from AST.cs):
//   var block = new BlockStmt(new SymbolTable<string, object>());
//   block.Add(someStatement);
//
// AssignmentStmt construction (from AST.cs):
//   new AssignmentStmt(new VariableNode("x"), someExpression)
//
// Structural constraints observed from NameAnalysisVisitor.cs:
//   - No public Errors list or Analyze() entry point; correctness is verified
//     solely through the bool return value of Accept.
//   - Binary expression nodes use short-circuit && evaluation: if the left
//     operand returns false, the right operand is never visited. Tests
//     therefore only assert the aggregate bool, not per-operand error counts.
//   - AssignmentStmt adds the LHS variable to the symbol table ONLY when the
//     RHS expression is valid (returns true).
//   - BlockStmt.Visit creates a fresh child SymbolTable linked to the parent,
//     so variables declared in an inner block are not visible in the outer block.
//   - The second item of the Tuple is the "containing statement" used in
//     error messages; tests supply a real statement for this context.
// =============================================================================

public class NameAnalysisVisitorTest
{
    private readonly NameAnalysisVisitor _visitor;

    public NameAnalysisVisitorTest()
    {
        _visitor = new NameAnalysisVisitor();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>Build a fresh empty SymbolTable.</summary>
    private static SymbolTable<string, object> EmptyTable()
        => new SymbolTable<string, object>();

    /// <summary>Build a SymbolTable pre-populated with the given names.</summary>
    private static SymbolTable<string, object> TableWith(params string[] names)
    {
        var t = new SymbolTable<string, object>();
        foreach (var n in names)
            t[n] = null;
        return t;
    }

    /// <summary>
    /// Build the Tuple parameter the visitor expects.
    /// </summary>
    private static Tuple<SymbolTable<string, object>, Statement> Param(
        SymbolTable<string, object> table, Statement ctx)
        => Tuple.Create(table, ctx);

    /// <summary>A lightweight dummy ReturnStmt used when the context statement is irrelevant.</summary>
    private static Statement DummyStmt()
        => new ReturnStmt(new LiteralNode(0));

    /// <summary>
    /// Builds a BlockStmt with a fresh SymbolTable and adds the given statements.
    /// This is the correct construction per AST.cs.
    /// </summary>
    private static BlockStmt MakeBlock(params Statement[] stmts)
    {
        var block = new BlockStmt(new SymbolTable<string, object>());
        foreach (var s in stmts)
            block.Add(s);
        return block;
    }

    /// <summary>
    /// Builds a BlockStmt whose SymbolTable is a child of the given parent table.
    /// </summary>
    private static BlockStmt MakeInnerBlock(
        SymbolTable<string, object> parentTable, params Statement[] stmts)
    {
        var block = new BlockStmt(new SymbolTable<string, object>(parentTable));
        foreach (var s in stmts)
            block.Add(s);
        return block;
    }

    // -----------------------------------------------------------------------
    // LiteralNode — always valid regardless of symbol table contents
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-1)]
    public void Visit_LiteralInt_AlwaysReturnsTrue(int value)
    {
        var node = new LiteralNode(value);
        Assert.True(node.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_LiteralDouble_AlwaysReturnsTrue()
    {
        var node = new LiteralNode(3.14);
        Assert.True(node.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    // -----------------------------------------------------------------------
    // VariableNode — defined vs. undefined
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_VariableNode_DefinedInTable_ReturnsTrue()
    {
        var node = new VariableNode("x");
        Assert.True(node.Accept(_visitor, Param(TableWith("x"), DummyStmt())));
    }

    [Fact]
    public void Visit_VariableNode_NotInTable_ReturnsFalse()
    {
        var node = new VariableNode("undeclared");
        Assert.False(node.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_VariableNode_DefinedAmongSeveralVariables_ReturnsTrue()
    {
        var node = new VariableNode("c");
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b", "c"), DummyStmt())));
    }

    // -----------------------------------------------------------------------
    // PlusNode — representative of all binary expression nodes
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_PlusNode_BothOperandsDefined_ReturnsTrue()
    {
        var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_PlusNode_LeftOperandUndefined_ReturnsFalse()
    {
        var node = new PlusNode(new VariableNode("noLeft"), new VariableNode("b"));
        Assert.False(node.Accept(_visitor, Param(TableWith("b"), DummyStmt())));
    }

    [Fact]
    public void Visit_PlusNode_RightOperandUndefined_ReturnsFalse()
    {
        var node = new PlusNode(new VariableNode("a"), new VariableNode("noRight"));
        Assert.False(node.Accept(_visitor, Param(TableWith("a"), DummyStmt())));
    }

    [Fact]
    public void Visit_PlusNode_BothOperandsUndefined_ReturnsFalse()
    {
        // Short-circuit &&: left fails so right is never visited; result is still false
        var node = new PlusNode(new VariableNode("noLeft"), new VariableNode("noRight"));
        Assert.False(node.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_PlusNode_LiteralOperands_ReturnsTrue()
    {
        var node = new PlusNode(new LiteralNode(1), new LiteralNode(2));
        Assert.True(node.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    // -----------------------------------------------------------------------
    // Spot-check every other binary node type with both operands defined
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_MinusNode_BothDefined_ReturnsTrue()
    {
        var node = new MinusNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_TimesNode_BothDefined_ReturnsTrue()
    {
        var node = new TimesNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_FloatDivNode_BothDefined_ReturnsTrue()
    {
        var node = new FloatDivNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_IntDivNode_BothDefined_ReturnsTrue()
    {
        var node = new IntDivNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_ModulusNode_BothDefined_ReturnsTrue()
    {
        var node = new ModulusNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    [Fact]
    public void Visit_ExponentiationNode_BothDefined_ReturnsTrue()
    {
        var node = new ExponentiationNode(new VariableNode("a"), new VariableNode("b"));
        Assert.True(node.Accept(_visitor, Param(TableWith("a", "b"), DummyStmt())));
    }

    // -----------------------------------------------------------------------
    // AssignmentStmt
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_AssignmentStmt_LiteralRhs_ReturnsTrue()
    {
        var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(1));
        Assert.True(stmt.Accept(_visitor, Param(EmptyTable(), stmt)));
    }

    [Fact]
    public void Visit_AssignmentStmt_LiteralRhs_AddsVariableToTable()
    {
        var table = EmptyTable();
        var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(1));
        stmt.Accept(_visitor, Param(table, stmt));
        Assert.True(table.ContainsKey("x"));
    }

    [Fact]
    public void Visit_AssignmentStmt_DefinedVariableOnRhs_ReturnsTrue()
    {
        var table = TableWith("y");
        var stmt = new AssignmentStmt(new VariableNode("x"), new VariableNode("y"));
        Assert.True(stmt.Accept(_visitor, Param(table, stmt)));
    }

    [Fact]
    public void Visit_AssignmentStmt_DefinedVariableOnRhs_AddsLhsToTable()
    {
        var table = TableWith("y");
        var stmt = new AssignmentStmt(new VariableNode("x"), new VariableNode("y"));
        stmt.Accept(_visitor, Param(table, stmt));
        Assert.True(table.ContainsKey("x"));
    }

    [Fact]
    public void Visit_AssignmentStmt_UndefinedVariableOnRhs_ReturnsFalse()
    {
        var stmt = new AssignmentStmt(new VariableNode("x"), new VariableNode("notDeclared"));
        Assert.False(stmt.Accept(_visitor, Param(EmptyTable(), stmt)));
    }

    [Fact]
    public void Visit_AssignmentStmt_UndefinedVariableOnRhs_DoesNotAddLhsToTable()
    {
        // Because the RHS fails, the LHS variable must NOT be entered into the table,
        // so downstream references to x do not get a false "declared" result.
        var table = EmptyTable();
        var stmt = new AssignmentStmt(new VariableNode("x"), new VariableNode("notDeclared"));
        stmt.Accept(_visitor, Param(table, stmt));
        Assert.False(table.ContainsKey("x"));
    }

    [Fact]
    public void Visit_AssignmentStmt_Reassignment_ReturnsTrue()
    {
        var table = TableWith("x");
        var stmt = new AssignmentStmt(new VariableNode("x"), new LiteralNode(42));
        Assert.True(stmt.Accept(_visitor, Param(table, stmt)));
    }

    // -----------------------------------------------------------------------
    // ReturnStmt
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_ReturnStmt_LiteralExpression_ReturnsTrue()
    {
        var stmt = new ReturnStmt(new LiteralNode(0));
        Assert.True(stmt.Accept(_visitor, Param(EmptyTable(), stmt)));
    }

    [Fact]
    public void Visit_ReturnStmt_DefinedVariable_ReturnsTrue()
    {
        var stmt = new ReturnStmt(new VariableNode("x"));
        Assert.True(stmt.Accept(_visitor, Param(TableWith("x"), stmt)));
    }

    [Fact]
    public void Visit_ReturnStmt_UndefinedVariable_ReturnsFalse()
    {
        var stmt = new ReturnStmt(new VariableNode("missing"));
        Assert.False(stmt.Accept(_visitor, Param(EmptyTable(), stmt)));
    }

    [Fact]
    public void Visit_ReturnStmt_ValidBinaryExpression_ReturnsTrue()
    {
        var stmt = new ReturnStmt(
            new PlusNode(new VariableNode("a"), new VariableNode("b")));
        Assert.True(stmt.Accept(_visitor, Param(TableWith("a", "b"), stmt)));
    }

    // -----------------------------------------------------------------------
    // BlockStmt — scope creation and variable visibility
    // -----------------------------------------------------------------------

    [Fact]
    public void Visit_BlockStmt_AllStatementsValid_ReturnsTrue()
    {
        var block = MakeBlock(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(1)),
            new ReturnStmt(new VariableNode("x")));
        Assert.True(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_UndeclaredVariableUsed_ReturnsFalse()
    {
        var block = MakeBlock(
            new ReturnStmt(new VariableNode("x"))); // x never declared
        Assert.False(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_UseBeforeDeclaration_ReturnsFalse()
    {
        // y is referenced before the statement that defines it
        var block = MakeBlock(
            new AssignmentStmt(new VariableNode("x"), new VariableNode("y")),
            new AssignmentStmt(new VariableNode("y"), new LiteralNode(5)));
        Assert.False(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_EmptyBlock_ReturnsTrue()
    {
        var block = new BlockStmt(new SymbolTable<string, object>());
        Assert.True(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_InnerScopeCanReadOuterVariable_ReturnsTrue()
    {
        // x defined in the outer table passed to the outer block;
        // NameAnalysisVisitor.Visit(BlockStmt) creates a child table from it,
        // so x is visible inside the inner block as well.
        var outerTable = TableWith("x");
        var inner = MakeBlock(new ReturnStmt(new VariableNode("x")));
        var outer = MakeBlock(inner);
        Assert.True(outer.Accept(_visitor, Param(outerTable, DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_VariableDeclaredInInnerScope_NotVisibleOutside_ReturnsFalse()
    {
        // y is declared only inside the inner block; the outer return y must fail
        var inner = MakeBlock(
            new AssignmentStmt(new VariableNode("y"), new LiteralNode(10)));
        var outer = MakeBlock(
            inner,
            new ReturnStmt(new VariableNode("y")));
        Assert.False(outer.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_ShadowedVariableInInnerScope_OuterUnaffected_ReturnsTrue()
    {
        // x defined in outer scope; inner block redefines x locally;
        // outer return x should still succeed because outer x is untouched
        var outerTable = TableWith("x");
        var inner = MakeBlock(
            new AssignmentStmt(new VariableNode("x"), new LiteralNode(99)));
        var outer = MakeBlock(
            inner,
            new ReturnStmt(new VariableNode("x")));
        Assert.True(outer.Accept(_visitor, Param(outerTable, DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_ContinuesAnalysisAfterError_ReturnsFalse()
    {
        // First statement has an error; second statement is valid.
        // The visitor must continue past the error; overall result must be false.
        var block = MakeBlock(
            new AssignmentStmt(new VariableNode("a"), new VariableNode("notDeclared")),
            new ReturnStmt(new LiteralNode(0)));
        Assert.False(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_MultipleUndeclaredVariables_ReturnsFalse()
    {
        var block = MakeBlock(
            new AssignmentStmt(new VariableNode("a"), new VariableNode("missing1")),
            new AssignmentStmt(new VariableNode("b"), new VariableNode("missing2")));
        Assert.False(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_ThreeLevelScopeChain_AllVariablesVisible_ReturnsTrue()
    {
        // x provided in the outerTable; middle block defines y; inner block defines z
        // and returns (y + z) — both must be visible.
        var outerTable = TableWith("x");

        var inner = MakeBlock(
            new AssignmentStmt(new VariableNode("z"), new LiteralNode(3)),
            new ReturnStmt(new PlusNode(
                new VariableNode("y"),
                new VariableNode("z"))));

        var middle = MakeBlock(
            new AssignmentStmt(new VariableNode("y"), new LiteralNode(2)),
            inner);

        var outer = MakeBlock(middle);
        Assert.True(outer.Accept(_visitor, Param(outerTable, DummyStmt())));
    }

    [Fact]
    public void Visit_BlockStmt_ValidComplexExpression_ReturnsTrue()
    {
        // a := 3; b := 4; return ((a * a) + (b * b))
        var block = MakeBlock(
            new AssignmentStmt(new VariableNode("a"), new LiteralNode(3)),
            new AssignmentStmt(new VariableNode("b"), new LiteralNode(4)),
            new ReturnStmt(new PlusNode(
                new TimesNode(new VariableNode("a"), new VariableNode("a")),
                new TimesNode(new VariableNode("b"), new VariableNode("b")))));
        Assert.True(block.Accept(_visitor, Param(EmptyTable(), DummyStmt())));
    }
}