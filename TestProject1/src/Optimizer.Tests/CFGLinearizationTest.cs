using System.Collections.Generic;
using System.Linq;
using AST;
using Optimizer;
using Xunit;

namespace Optimizer.Tests;

/// <summary>
/// Integration tests for DiGraph.FindStronglyConnectedComponents exercised
/// through a full Parser → ControlFlowGraphGeneratorVisitor → CFG pipeline.
///
/// The current CFG builder emits only forward, acyclic flow (no loops or
/// conditionals are modeled yet), so every CFG produced from a parsed program
/// is a DAG. In a DAG each vertex is its own strongly connected component.
/// The base invariant these tests verify is therefore:
///
///     number of SCCs == number of AssignmentStmt + ReturnStmt in the program
///
/// and each SCC contains exactly one statement, with reference-equal
/// membership. Additional tests inject cycles manually to prove the SCC
/// algorithm collapses them correctly even when the visitor never produces
/// cycles on its own.
/// </summary>
public class CFGLinearizationTests
{
    #region Test Helpers

    /// <summary>
    /// Recursively collects every AssignmentStmt and ReturnStmt reachable by
    /// walking into any nested BlockStmt children. These are exactly the
    /// statement kinds the CFG visitor emits as vertices.
    /// </summary>
    private static List<Statement> CollectCfgStatements(BlockStmt block)
    {
        var results = new List<Statement>();
        foreach (var stmt in block.Statements)
        {
            if (stmt is AssignmentStmt || stmt is ReturnStmt) results.Add(stmt);
            else if (stmt is BlockStmt inner) results.AddRange(CollectCfgStatements(inner));
        }
        return results;
    }

    /// <summary>
    /// Parses <paramref name="program"/>, runs the CFG visitor, and returns
    /// the built CFG along with the parsed block for inspection.
    /// </summary>
    private static CFG BuildCfg(string program, out BlockStmt parsed)
    {
        parsed = Parser.Parser.Parse(program);
        var visitor = new ControlFlowGraphGeneratorVisitor();
        parsed.Accept(visitor, null);
        return visitor._cfg;
    }

    /// <summary>
    /// Strict validation that the SCC result is structurally sound for a DAG:
    ///   1. Count matches the number of AssignmentStmt/ReturnStmt in source.
    ///   2. Every SCC is a singleton.
    ///   3. Every expected statement appears in exactly one SCC by reference.
    ///   4. No alien vertex leaked into the SCC list.
    /// </summary>
    private static void AssertEveryStatementIsItsOwnSCC(
        List<Statement> expectedStatements,
        List<List<Statement>> sccs)
    {
        Assert.Equal(expectedStatements.Count, sccs.Count);
        Assert.All(sccs, scc => Assert.Single(scc));

        var flattened = sccs.SelectMany(s => s).ToList();
        Assert.Equal(expectedStatements.Count, flattened.Count);

        foreach (var stmt in expectedStatements)
        {
            Assert.Contains(flattened, v => ReferenceEquals(v, stmt));
        }

        foreach (var vertex in flattened)
        {
            Assert.Contains(expectedStatements, s => ReferenceEquals(s, vertex));
        }
    }

    #endregion

    #region Trivial Programs

    [Fact]
    public void FindSCC_SingleAssignment_ProducesOneSingletonComponent()
    {
        string program = @"{
        x := (1)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_SingleReturn_ProducesOneSingletonComponent()
    {
        string program = @"{
        return (42)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    #endregion

    #region Sequential Statements

    [Fact]
    public void FindSCC_SequentialAssignments_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        d := (4)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_AssignmentsThenReturn_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        return (c)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_LongLinearProgram_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        d := (4)
        e := (5)
        f := (6)
        g := (7)
        return (g)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_MultipleSequentialReturns_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        return (1)
        return (2)
        return (3)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    #endregion

    #region Nested Blocks

    [Fact]
    public void FindSCC_NestedBlocks_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        a := (1)
        {
            b := (2)
            c := (3)
        }
        d := (4)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_DeeplyNestedBlocks_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        a := (1)
        {
            b := (2)
            {
                c := (3)
                {
                    d := (4)
                    return (d)
                }
            }
        }
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    #endregion

    #region Dead Code After Return

    [Fact]
    public void FindSCC_DeadCodeAfterReturn_StillCountsEveryStatement()
    {
        string program = @"{
        x := (1)
        y := (2)
        return (y)
        z := (3)
        w := (4)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        // Statements after the return are unreachable but still become CFG
        // vertices, so each still forms its own SCC.
        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_DeadCodeSpreadAcrossNestedBlocks_StillCountsEveryStatement()
    {
        string program = @"{
        x := (1)
        {
            y := (2)
            return (y)
            z := (3)
        }
        w := (4)
        return (w)
        v := (5)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    #endregion

    #region End-to-End Programs

    [Fact]
    public void FindSCC_SimpleProgram_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        x := (1)
        y := (2)
        {
            b := (x + y)
            return (b)
        }
        c := (8)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    [Fact]
    public void FindSCC_ComplexProgram_EachStatementIsItsOwnSCC()
    {
        string program = @"{
        x := (1)
        {
            y := (2)
            {
                z := (3)
                return (z)
                w := (4)
            }
            return (y)
            q := (5)
        }
        m := (6)
        return (m)
        n := (7)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var expected = CollectCfgStatements(parsed);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(expected, sccs);
    }

    #endregion

    #region Manually Induced Cycles

    [Fact]
    public void FindSCC_WithBackEdgeToStart_CollapsesAllReachableIntoOneComponent()
    {
        // Build an acyclic CFG, then manually close a loop by linking the last
        // statement back to the first. Every statement in the chain is now
        // mutually reachable, so they must collapse into a single SCC.
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        return (c)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var statements = CollectCfgStatements(parsed);
        cfg.AddEdge(statements[^1], statements[0]);

        var sccs = cfg.FindStronglyConnectedComponents();

        Assert.Single(sccs);
        Assert.Equal(statements.Count, sccs[0].Count);
        foreach (var stmt in statements)
        {
            Assert.Contains(sccs[0], v => ReferenceEquals(v, stmt));
        }
    }

    [Fact]
    public void FindSCC_WithPartialBackEdge_OnlyTheLoopedSegmentCollapses()
    {
        // a -> b -> c -> d, plus a back-edge c -> b. Only {b, c} are mutually
        // reachable; a and d remain singletons. Total SCC count must drop by
        // exactly one compared to the acyclic case.
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        d := (4)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var statements = CollectCfgStatements(parsed);
        cfg.AddEdge(statements[2], statements[1]);

        var sccs = cfg.FindStronglyConnectedComponents();

        Assert.Equal(statements.Count - 1, sccs.Count);

        var loopScc = sccs.Single(s => s.Count == 2);
        Assert.Contains(loopScc, v => ReferenceEquals(v, statements[1]));
        Assert.Contains(loopScc, v => ReferenceEquals(v, statements[2]));

        var singletons = sccs.Where(s => s.Count == 1).SelectMany(s => s).ToList();
        Assert.Contains(singletons, v => ReferenceEquals(v, statements[0]));
        Assert.Contains(singletons, v => ReferenceEquals(v, statements[3]));
    }

    [Fact]
    public void FindSCC_WithSelfLoopOnStatement_DoesNotInflateComponentSize()
    {
        // A self-loop only makes a vertex mutually reachable with itself,
        // which every vertex already is. The SCC structure must be unchanged.
        string program = @"{
        a := (1)
        b := (2)
        c := (3)
        }";
        var cfg = BuildCfg(program, out var parsed);
        var statements = CollectCfgStatements(parsed);
        cfg.AddEdge(statements[1], statements[1]);

        var sccs = cfg.FindStronglyConnectedComponents();

        AssertEveryStatementIsItsOwnSCC(statements, sccs);
    }

    #endregion

    #region Determinism and Isolation

    [Fact]
    public void FindSCC_CalledTwiceOnSameCfg_ProducesEquivalentPartition()
    {
        string program = @"{
        x := (1)
        {
            y := (2)
            {
                z := (3)
                return (z)
                w := (4)
            }
            return (y)
            q := (5)
        }
        m := (6)
        return (m)
        n := (7)
        }";
        var cfg = BuildCfg(program, out _);

        var first = cfg.FindStronglyConnectedComponents();
        var second = cfg.FindStronglyConnectedComponents();

        Assert.Equal(first.Count, second.Count);

        // Same set of singleton contents across both runs, regardless of the
        // outer ordering the algorithm chose.
        var firstMembers = first.SelectMany(s => s).ToHashSet();
        var secondMembers = second.SelectMany(s => s).ToHashSet();
        Assert.Equal(firstMembers, secondMembers);
    }

    [Fact]
    public void FindSCC_DoesNotMutateCfg()
    {
        string program = @"{
        x := (1)
        y := (2)
        return (y)
        z := (3)
        }";
        var cfg = BuildCfg(program, out _);

        int verticesBefore = cfg.VertexCount();
        int edgesBefore = cfg.EdgeCount();
        var startBefore = cfg.Start;

        cfg.FindStronglyConnectedComponents();

        Assert.Equal(verticesBefore, cfg.VertexCount());
        Assert.Equal(edgesBefore, cfg.EdgeCount());
        Assert.Same(startBefore, cfg.Start);
    }

    #endregion
}
