using Utilities.Containers;
using AST;
using System.Diagnostics;
using System.Xml;

namespace Optimizer;

/// <summary>
/// Represents a control flow graph (CFG) over <see cref="Statement"/> nodes.
/// This graph models possible execution paths between statements so optimizer
/// passes can reason about reachability and dead code.
/// </summary>
public class CFG : DiGraph<Statement>
{
    /// <summary>
    /// Gets or sets the entry statement for traversal operations.
    /// Reachability analyses begin from this node when it is non-null.
    /// </summary>
    public Statement? Start { get; set; }

    /// <summary>
    /// Initializes an empty control flow graph with no vertices or edges.
    /// </summary>
    public CFG() : base() {}

    /// <summary>
    /// Encodes the discovery state of a node during graph traversal.
    /// </summary>
    public enum Colors {WHITE, PURPLE, BLACK}

    /// <summary>
    /// Performs a breadth-first traversal starting at <see cref="Start"/> and partitions
    /// graph vertices into reachable and unreachable sets.
    /// </summary>
    /// <returns>
    /// A tuple where:
    /// <list type="bullet">
    /// <item>
    /// <description><c>reachable</c> contains statements visited by BFS from <see cref="Start"/>.</description>
    /// </item>
    /// <item>
    /// <description><c>unreachable</c> contains statements never visited during traversal.</description>
    /// </item>
    /// </list>
    /// If <see cref="Start"/> is <c>null</c>, the method returns <c>default</c>.
    /// </returns>
    public (List<Statement> reachable, List<Statement> unreachable) BreadthFirstSearch()
    {
        // Track traversal color for each statement node.
        Dictionary<Statement, Colors> colors = new Dictionary<Statement, Colors>();
        List<Statement> reachable = new List<Statement>();
        List<Statement> unreachable = new List<Statement>();

        // Initialize each vertex as undiscovered and initially unreachable.
        foreach (Statement b in GetVertices())
        {
            colors[b] = Colors.WHITE;
            unreachable.Add(b);
        }

        // Create the BFS queue and seed it with the start vertex if it exists.
        Queue<Statement> q = new Queue<Statement>();
        if (Start == null) return default;
        q.Enqueue(Start);

        // Process nodes in FIFO order to visit by shortest edge distance.
        while (q.Count > 0)
        {
            Statement u = q.Dequeue();
            // Inspect each outgoing edge from the current statement.
            foreach (Statement v in GetNeighbors(u))
            {
                // Only enqueue nodes that have not been discovered yet.
                if (colors[v] == Colors.WHITE)
                {
                    // Mark as discovered and add to the BFS frontier.
                    colors[v] = Colors.PURPLE;
                    q.Enqueue(v);
                }
            }

            // Mark this node fully processed.
            colors[u] = Colors.BLACK;
            // Move the node from unreachable to reachable partition.
            reachable.Add(u);
            unreachable.Remove(u);
        }

        // Return both partitions for later optimization passes.
        return (reachable, unreachable);
    }

}