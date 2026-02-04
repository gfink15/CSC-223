/**
 * Doubly Linked List Class: Provide functionality for
 * a doubly linked list structure
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   1/28/2026
 */

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Text;

namespace Utilities.Containers;


public class SymbolTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private DLL<TKey> _KeyDLL = new DLL<TKey>();
    private DLL<TValue> _ValueDLL = new DLL<TValue>();
    private SymbolTable<TKey, TValue>? _parent;
    private int _sz = 0;

    public SymbolTable<TKey, TValue> Parent
    {
        get { return _parent; }
    }

    public ICollection<TKey> Keys
    {
        get { return _KeyDLL; }
    }

    public ICollection<TValue> Values
    {
        get { return _ValueDLL; }
    }

    public int Count
    {
        get { return _sz; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    public TValue this[TKey key]
    { 
        get 
        {
            if (TryGetValue(key, out TValue? value)) return value;
            else
            {
                throw new KeyNotFoundException("Key does not exist");
            }
        }
        set
        {
            if (ContainsKey(key))
            {
                int keyIndex = _KeyDLL.IndexOf(key);
                _ValueDLL[keyIndex] = value;
            }
            else
            {
                Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public SymbolTable(SymbolTable<TKey, TValue>? parent)
    {
        _parent = parent;
    }

    public bool ContainsKeyLocal(TKey key)
    {
        if (key == null) throw new ArgumentNullException("Key is null");
        return _KeyDLL.Contains(key);
    }

    public bool TryGetValueLocal(TKey key, out TValue value)
    {
        if (ContainsKeyLocal(key))
        {
            int keyIndex = _KeyDLL.IndexOf(key);
            value = _ValueDLL[keyIndex];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKeyLocal(key)) throw new ArgumentException("Key already exists");
        else
        {
            _KeyDLL.Add(key);
            _ValueDLL.Add(value);
            _sz++;
        }
    }

    public bool ContainsKey(TKey key)
    {
        if (!ContainsKeyLocal(key))
        {
            if (_parent != null)
            {
                return _parent.ContainsKey(key);
            }
        }
        return false;
    }

    public bool Remove(TKey key)
    {
        if (!ContainsKey(key)) throw new KeyNotFoundException("Key not found");
        int indexToRemove = _KeyDLL.IndexOf(key);
        _KeyDLL.RemoveAt(indexToRemove);
        _ValueDLL.RemoveAt(indexToRemove);
        return true;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {   
        value = default;
        if (!ContainsKeyLocal(key)) return false;

        int indexToRemove = _KeyDLL.IndexOf(key);

        // Come back to this, this is the global TryGetValue
        throw new NotImplementedException();


    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        _KeyDLL.Clear();
        _ValueDLL.Clear();
        _sz = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        TValue v;
        if (TryGetValue(item.Key, out v))
        {
            if (EqualityComparer<TValue>.Default.Equals(item.Value, v))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // Checking the conditions for processing and throwing exceptions if not met
        if (array == null) throw new ArgumentNullException();
        if ((array.Length - arrayIndex) < _sz ) throw new ArgumentException();
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();

        for (int tableIndex = 0; tableIndex < _sz; tableIndex++)
        {
            var newPair = new KeyValuePair<TKey, TValue>(_KeyDLL[tableIndex], _ValueDLL[tableIndex]);
            array[arrayIndex + tableIndex] = newPair;
        }

    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < _sz; i++)
        {
            yield return new KeyValuePair<TKey, TValue>(_KeyDLL[i], _ValueDLL[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}