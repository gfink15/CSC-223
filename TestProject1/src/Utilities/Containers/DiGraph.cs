/**
 * The DiGraph Class. 
 * Comments and Docstrings with the help of GPT 5.3 Codex.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   4/8/2026
 */


using System.Runtime.CompilerServices;

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
    public string ToString()
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
}