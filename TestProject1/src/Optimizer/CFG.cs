using Utilities.Containers;
using AST;
using System.Diagnostics;
using System.Xml;

namespace Optimizer;
public class CFG : DiGraph<Statement>
{
    public Statement? Start { get; set; }

    public CFG() : base() {}

    public enum Colors {WHITE, PURPLE, BLACK}
    public (List<Statement> reachable, List<Statement> unreachable) BreadthFirstSearch()
    {
        Dictionary<Statement, Colors> colors = new Dictionary<Statement, Colors>();
        List<Statement> reachable = new List<Statement>();
        List<Statement> unreachable = new List<Statement>();
        foreach (Statement b in GetVertices())
        {
            colors[b] = Colors.WHITE;
            unreachable.Add(b);
        }
        Queue<Statement> q = new Queue<Statement>();
        if (Start == null) return default;
        q.Enqueue(Start);
        while (q.Count > 0)
        {
            Statement u = q.Dequeue();
            foreach (Statement v in GetNeighbors(u))
            {
                if (colors[v] == Colors.WHITE)
                {
                    colors[v] = Colors.PURPLE;
                    q.Enqueue(v);
                }
            }
            colors[u] = Colors.BLACK;
            reachable.Add(u);
            unreachable.Remove(u);
        }
        return (reachable, unreachable);
    }

}