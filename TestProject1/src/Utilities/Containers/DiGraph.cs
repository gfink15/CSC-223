/**
 * The DiGraph Class. 
 * Comments and Docstrings with the help of GPT 5.3 Codex.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   4/8/2026
 */


using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace Utilities.Containers;

/// <summary>
/// Represents a directed graph using an adjacency-list backing store.
/// </summary>
/// <typeparam name="T">The vertex type. Vertices must be non-null.</typeparam>
public class DiGraph<T> where T : notnull
{
    /// <summary>
    /// Maps each vertex to the list of vertices it has outgoing edges to.
    /// </summary>
    protected Dictionary<T, DLL<T>> _adjacencyList = new Dictionary<T, DLL<T>>();

    /// <summary>
    /// Initializes an empty directed graph.
    /// </summary>
    public DiGraph()
    {
    }

    /// <summary>
    /// Adds a vertex to the graph.
    /// </summary>
    /// <param name="vertex">The vertex to add.</param>
    /// <returns><c>true</c> if the vertex was added; otherwise, <c>false</c> if it already exists.</returns>
    public bool AddVertex(T vertex)
    {
        return _adjacencyList.TryAdd(vertex, new DLL<T>());
    }

    /// <summary>
    /// Adds a directed edge from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    /// <param name="source">The source vertex.</param>
    /// <param name="destination">The destination vertex.</param>
    /// <returns>
    /// <c>true</c> if the edge was added; <c>false</c> if that exact edge already exists.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when either vertex is not present in the graph.
    /// </exception>
    public bool AddEdge(T source, T destination)
    {
        // Both endpoints must already be present in the graph.
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination))
        {
            throw new ArgumentException("Both the Source and the Destination must exist.");
        }
        else if (_adjacencyList[source].Contains(destination))
        {
            // Do not store duplicate directed edges.
            return false;
        }
        else
        {
            _adjacencyList[source].Add(destination);
            return true;
        }
    }

    /// <summary>
    /// Removes a vertex and all edges that point to it.
    /// </summary>
    /// <param name="vertex">The vertex to remove.</param>
    /// <returns><c>true</c> when the vertex is removed.</returns>
    /// <exception cref="ArgumentException">Thrown when the vertex is not found.</exception>
    public bool RemoveVertex(T vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex))
        {
            throw new ArgumentException("Vertex not found!");
        }
        else
        {
            // Remove the vertex and its outgoing edges.
            _adjacencyList.Remove(vertex);

            // Remove incoming edges from all remaining vertices.
            foreach(KeyValuePair<T, DLL<T>> item in _adjacencyList)
            {
                item.Value.Remove(vertex);
            }
            return true;
        }
    }

    /// <summary>
    /// Removes a directed edge from source to destination.
    /// </summary>
    /// <param name="source">The vertex where the edge starts.</param>
    /// <param name="destination">The vertex where the edge ends.</param>
    /// <returns><c>true</c> if the edge existed and was removed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when either source or destination is not a valid vertex in the graph.
    /// </exception>
    public bool RemoveEdge(T source, T destination)
    {
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination)) throw new ArgumentException("Must provide valid source/destinations");
        if (HasEdge(source, destination))
        {
            // DLL.Remove removes the first matching edge.
            _adjacencyList[source].Remove(destination);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Determines whether a directed edge exists from <paramref name="source"/> to <paramref name="destination"/>.
    /// </summary>
    /// <param name="source">The source vertex.</param>
    /// <param name="destination">The destination vertex.</param>
    /// <returns><c>true</c> if the edge exists; otherwise, <c>false</c>.</returns>
    public bool HasEdge(T source, T destination)
    {
        return _adjacencyList[source].Contains(destination);
    }

    /// <summary>
    /// Returns all outgoing neighbors for a vertex.
    /// </summary>
    /// <param name="vertex">The vertex whose neighbors are requested.</param>
    /// <returns>A new list containing the vertex's outgoing neighbors.</returns>
    /// <exception cref="ArgumentException">Thrown when the vertex is not found.</exception>
    public List<T> GetNeighbors(T vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex)) throw new ArgumentException("Vertex not found!");

        // Return a list copy so callers cannot mutate internal adjacency storage.
        return _adjacencyList[vertex].ToList();
    }

    /// <summary>
    /// Enumerates all vertices currently in the graph.
    /// </summary>
    /// <returns>An enumerable view of all vertices.</returns>
    public IEnumerable<T> GetVertices()
    {
        foreach (T vertex in _adjacencyList.Keys)
        {
            yield return vertex;
        }
    }

    /// <summary>
    /// Returns the number of vertices in the graph.
    /// </summary>
    /// <returns>The vertex count.</returns>
    public int VertexCount()
    {
        return _adjacencyList.Count();
    }

    /// <summary>
    /// Returns the number of directed edges in the graph.
    /// </summary>
    /// <returns>The total edge count.</returns>
    public int EdgeCount()
    {
        int c = 0;
        foreach (DLL<T> list in _adjacencyList.Values)
        {
            c += list.Count;
        }
        return c;
    }

    /// <summary>
    /// Builds a readable string of each vertex and its outgoing neighbors.
    /// </summary>
    /// <returns>A concatenated graph description.</returns>
    public override string ToString()
    {
        string r = "";
        foreach (KeyValuePair<T, DLL<T>> vals in _adjacencyList)
        {
            r += "Node "+vals.Key+" connects to: ";
            foreach (T item in vals.Value)
            {
                r += item.ToString() + ", ";
            }
        }
        return r;
    }

    /// <summary>
    /// Vertex coloring used during DFS traversal.
    /// <list type="bullet">
    ///   <item><description><c>WHITE</c> — not yet discovered.</description></item>
    ///   <item><description><c>PURPLE</c> — discovered, but its DFS call has not finished (still on the recursion stack).</description></item>
    ///   <item><description><c>BLACK</c> — fully processed; all descendants visited.</description></item>
    /// </list>
    /// </summary>
    public enum VertexColor { WHITE, PURPLE, BLACK }

    /// <summary>
    /// Runs a depth-first search over the entire graph and returns vertices
    /// in reverse postorder (the order they finish, with the last-finished
    /// vertex on top of the stack).
    ///
    /// Popping the returned stack yields a valid topological ordering when the
    /// graph is a DAG. It is also the ordering used as input to Kosaraju's
    /// SCC algorithm in <see cref="FindStronglyConnectedComponents"/>.
    /// </summary>
    /// <returns>A stack of every vertex, ordered by DFS finish time (latest on top).</returns>
    public Stack<T> DepthFirstSearch()
    {
        var colors = new Dictionary<T, VertexColor>();
        IEnumerable<T> vertices = GetVertices();
        var dfs_stack = new Stack<T>();

        // Start every vertex as undiscovered.
        foreach (T vertex in vertices) colors.TryAdd(vertex, VertexColor.WHITE);

        // Launch a DFS from each undiscovered vertex so disconnected
        // components are all covered.
        foreach (T vertex in colors.Keys)
        {
            if (colors[vertex] == VertexColor.WHITE) DFS_Visit(vertex, colors, dfs_stack);
        }

        return dfs_stack;
    }

    /// <summary>
    /// Recursive DFS helper. Marks <paramref name="vertex"/> as in-progress,
    /// recurses into each unvisited neighbor, then marks it finished and
    /// pushes it onto <paramref name="dfs_stack"/> so the stack ends up in
    /// reverse postorder.
    /// </summary>
    /// <param name="vertex">The vertex to visit.</param>
    /// <param name="colors">Shared color map tracking each vertex's DFS state.</param>
    /// <param name="dfs_stack">Stack that collects vertices in finish order.</param>
    private void DFS_Visit(T vertex, Dictionary<T, VertexColor> colors, Stack<T> dfs_stack)
    {
        // Mark as "in progress" — stops cycles from recursing forever.
        colors[vertex] = VertexColor.PURPLE;

        foreach (T adj_vertex in GetNeighbors(vertex))
        {
            if (colors[adj_vertex] == VertexColor.WHITE) DFS_Visit(adj_vertex, colors, dfs_stack);
        }

        // All descendants done — record this vertex's finish time by pushing it.
        colors[vertex] = VertexColor.BLACK;
        dfs_stack.Push(vertex);
    }

    /// <summary>
    /// Returns a new graph with the same vertices but every edge reversed:
    /// for every edge <c>u -> v</c> in this graph, the returned graph contains
    /// <c>v -> u</c>. The original graph is not modified.
    /// </summary>
    /// <returns>A new <see cref="DiGraph{T}"/> representing the transpose.</returns>
    public DiGraph<T> Transpose()
    {
        var reversed_graph = new DiGraph<T>();

        // Copy the vertex set first so AddEdge below always has both endpoints.
        foreach (T vertex in GetVertices()) reversed_graph.AddVertex(vertex);

        // For each original edge u -> v, add the reversed edge v -> u.
        foreach (T vertex in _adjacencyList.Keys)
        {
            foreach (T adj_vertex in GetNeighbors(vertex)) reversed_graph.AddEdge(adj_vertex, vertex);
        }

        return reversed_graph;
    }

    /// <summary>
    /// Finds every strongly connected component (SCC) of the graph using
    /// Kosaraju's algorithm. An SCC is a maximal set of vertices in which
    /// every vertex can reach every other via directed edges; the SCCs
    /// partition the vertex set (each vertex appears in exactly one).
    ///
    /// Algorithm:
    ///   1. Run DFS on this graph and collect finish-order in a stack.
    ///   2. Transpose the graph (reverse every edge).
    ///   3. Pop vertices from the stack; each DFS on the transpose that starts
    ///      from an unvisited vertex discovers exactly one SCC.
    /// </summary>
    /// <returns>
    /// A list of SCCs. Each inner list contains the vertices belonging to one
    /// SCC. The order of SCCs and the order of vertices within them is an
    /// implementation detail and should not be relied on.
    /// </returns>
    public List<List<T>> FindStronglyConnectedComponents()
    {
        // Step 1: finish-order of a DFS on the original graph.
        Stack<T> dfs_stack = DepthFirstSearch();
        List<List<T>> scc = new List<List<T>>();

        // Step 2: edges reversed so DFS on this graph explores SCCs inward.
        DiGraph<T> reversed_graph = Transpose();
        var colors = new Dictionary<T, VertexColor>();

        // Mark every vertex undiscovered for the second-pass DFS.
        foreach (T vertex in dfs_stack) colors.TryAdd(vertex, VertexColor.WHITE);

        // Step 3: walk vertices in reverse finish order (top of the stack
        // first). Each DFS_Visit on the transpose that starts from a WHITE
        // vertex visits exactly the vertices of one SCC.
        foreach (T vertex in dfs_stack)
        {
            // BLACK means this vertex was already included in a previously
            // discovered SCC — skip it to avoid double-counting.
            if (colors[vertex] != VertexColor.BLACK)
            {
                Stack<T> local_dfs_stack = new Stack<T>();
                reversed_graph.DFS_Visit(vertex, colors, local_dfs_stack);
                scc.Add(local_dfs_stack.ToList());
            }
        }

        return scc;
    }
}