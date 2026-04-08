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
    public bool RemoveEdge(T source, T destination)
    {
        if (!_adjacencyList.Contains(source) || !_adjacencyList.Contains(destination)) throw new ArgumentException("Must provide valid source/destinations");
        if (HasEdge(source, destination))
        {
            _adjacencyList[source].Remove(destination);
            _adjacencyList[destination].Remove(source);
            return true;
        }
        return false;
    }
    public bool HasEdge(T source, T destination)
    {
        throw new NotImplementedException();
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