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
        throw new NotImplementedException();
    }
    public IEnumerable<T> GetVertices()
    {
        throw new NotImplementedException();
    }
    public int VertexCount()
    {
        throw new NotImplementedException();
    }
    public int EdgeCount()
    {
        throw new NotImplementedException();
    }
    public string ToString()
    {
        throw new NotImplementedException();
    }
}