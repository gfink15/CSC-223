

using System.Runtime.CompilerServices;


/**
 * 
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   4/8/2026
 */
namespace Utilities.Containers;

public class DiGraph<T> where T : notnull
{
    protected Dictionary<T, DLL<T>> _adjacencyList = new Dictionary<T, DLL<T>>();

    public DiGraph()
    {
    }

    public bool AddVertex(T vertex)
    {
        return _adjacencyList.TryAdd(vertex, new DLL<T>());
    }

    public bool AddEdge(T source, T destination)
    {
        if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination))
        {
            throw new ArgumentException("Both the Source and the Destination must exist.");
        }
        else if (_adjacencyList[source].Contains(destination))
        {
            return false;
        }
        else
        {
            _adjacencyList[source].Add(destination);
            return true;
        }
    }

    public bool RemoveVertex(T vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex))
        {
            throw new ArgumentException("Vertex not found!");
        }
        else
        {
            _adjacencyList.Remove(vertex);
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
            _adjacencyList[source].Remove(destination);
            return true;
        }
        return false;
    }
    
    public bool HasEdge(T source, T destination)
    {
        return _adjacencyList[source].Contains(destination);
    }

    public List<T> GetNeighbors(T vertex)
    {
        if (!_adjacencyList.ContainsKey(vertex)) throw new ArgumentException("Vertex not found!");
        return _adjacencyList[vertex].ToList();
    }
    public IEnumerable<T> GetVertices()
    {
        foreach (T vertex in _adjacencyList.Keys)
        {
            yield return vertex;
        }
    }
    public int VertexCount()
    {
        return _adjacencyList.Count();
    }
    public int EdgeCount()
    {
        int c = 0;
        foreach (DLL<T> list in _adjacencyList.Values)
        {
            c += list.Count;
        }
        return c;
    }
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