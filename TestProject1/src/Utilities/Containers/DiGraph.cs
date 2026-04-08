

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
            foreach(KeyValuePair<T, DLL<T>> item in _adjacencyList)
            {
                return true;
            }
        }
        return true;
    }

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
        return _adjacencyList[vertex];
    }
    public IEnumerable<T> GetVertices()
    {
        foreach (T vertex in _adjacencyList.Keys())
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
        foreach (DLL list in _adjacencyList.Values())
        {
            c += list.Count;
        }
        return c;
    }
    public string ToString()
    {
        string r = "";
        foreach (Tuple<T, DLL<T>> vals in _adjacencyList)
        {
            r += "Node "+vals.Item1+" connects to: ";
            foreach (T item in vals.Item2)
            {
                r += item.ToString() + ", ";
            }
        }
        return r;
    }
}