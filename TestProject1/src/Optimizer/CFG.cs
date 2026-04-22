using Utilities.Containers;
using AST;
using System.Diagnostics;

namespace Optimizer;
public class CFG : DiGraph<Statement>
{
    public Statement? Start { get; set; }

    public CFG() : base() {}

    public (List<Statement> reachable, List<Statement> unreachable) BreadthFirstSearch()
    {
        List<Statement> reachable = new List<Statement>();
        List<Statement> unreachable = new List<Statement>();
        Queue<Statement> q = new Queue<Statement>();
        if (Start == null) return default;
        q.Enqueue(Start);
        while (q.Count > 0)
        {
            Statement u = q.Dequeue();
            foreach (Statement v in GetNeighbors(u))
            {
                q.Enqueue(v);
            }
            reachable.Add(u);
        }
        foreach(Statement a in GetVertices())
        {
            if (!reachable.Contains(a)) unreachable.Add(a);
        }
        return (reachable, unreachable);
    }

}