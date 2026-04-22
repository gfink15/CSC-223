using Optimizer;
using Xunit;
using AST;

namespace Optimizer.Tests;
public class CFGTest
{
    private static AssignmentStmt NewAssignment(string name, int value)
    {
        return new AssignmentStmt(new VariableNode(name), new LiteralNode(value));
    }

    [Fact]
    public void BreadthFirstSearch_WhenStartIsNull_ReturnsDefaultTuple()
    {
        var cfg = new CFG();

        var result = cfg.BreadthFirstSearch();

        Assert.Null(result.reachable);
        Assert.Null(result.unreachable);
    }

    [Fact]
    public void BreadthFirstSearch_WithSingleStartVertex_ReturnsOnlyStartAsReachable()
    {
        var cfg = new CFG();
        var start = NewAssignment("x", 1);

        cfg.AddVertex(start);
        cfg.Start = start;

        var (reachable, unreachable) = cfg.BreadthFirstSearch();

        Assert.Single(reachable);
        Assert.Same(start, reachable[0]);
        Assert.Empty(unreachable);
    }

    [Fact]
    public void BreadthFirstSearch_WithDisconnectedVertex_SeparatesReachableAndUnreachable()
    {
        var cfg = new CFG();
        var start = NewAssignment("x", 1);
        var next = NewAssignment("y", 2);
        var isolated = NewAssignment("z", 3);

        cfg.AddVertex(start);
        cfg.AddVertex(next);
        cfg.AddVertex(isolated);
        cfg.AddEdge(start, next);
        cfg.Start = start;

        var (reachable, unreachable) = cfg.BreadthFirstSearch();

        Assert.Equal(2, reachable.Count);
        Assert.Same(start, reachable[0]);
        Assert.Same(next, reachable[1]);
        Assert.Single(unreachable);
        Assert.Same(isolated, unreachable[0]);
    }

    [Fact]
    public void BreadthFirstSearch_WithBranchingGraph_ReturnsLevelOrderTraversal()
    {
        var cfg = new CFG();
        var start = NewAssignment("a", 1);
        var left = NewAssignment("b", 2);
        var right = NewAssignment("c", 3);
        var leftChild = NewAssignment("d", 4);
        var rightChild = NewAssignment("e", 5);

        cfg.AddVertex(start);
        cfg.AddVertex(left);
        cfg.AddVertex(right);
        cfg.AddVertex(leftChild);
        cfg.AddVertex(rightChild);

        cfg.AddEdge(start, left);
        cfg.AddEdge(start, right);
        cfg.AddEdge(left, leftChild);
        cfg.AddEdge(right, rightChild);
        cfg.Start = start;

        var (reachable, unreachable) = cfg.BreadthFirstSearch();

        Assert.Equal(5, reachable.Count);
        Assert.Same(start, reachable[0]);
        Assert.Same(left, reachable[1]);
        Assert.Same(right, reachable[2]);
        Assert.Same(leftChild, reachable[3]);
        Assert.Same(rightChild, reachable[4]);
        Assert.Empty(unreachable);
    }

    [Fact]
    public void BreadthFirstSearch_WhenStartIsNotInGraph_ThrowsArgumentException()
    {
        var cfg = new CFG();
        cfg.Start = NewAssignment("missing", 0);

        Assert.Throws<ArgumentException>(() => cfg.BreadthFirstSearch());
    }

    [Fact]
    public void BreadthFirstSearch_WithParsedProgramAndVisitor_FindsTwoUnreachableStatements()
    {
        string program = @"{
        x := (1)
        y := (2)
        return (y)
        z := (3)
        w := (4)
        }";

        var parsed = Parser.Parser.Parse(program);
        var visitor = new ControlFlowGraphGeneratorVisitor();
        parsed.Accept(visitor, null);

        Statement x = parsed.Statements[0];
        Statement y = parsed.Statements[1];
        Statement returnY = parsed.Statements[2];
        Statement z = parsed.Statements[3];
        Statement w = parsed.Statements[4];

        var (reachable, unreachable) = visitor._cfg.BreadthFirstSearch();

        Assert.Equal(3, reachable.Count);
        Assert.Same(x, reachable[0]);
        Assert.Same(y, reachable[1]);
        Assert.Same(returnY, reachable[2]);

        Assert.Equal(2, unreachable.Count);
        Assert.Same(z, unreachable[0]);
        Assert.Same(w, unreachable[1]);
    }
}