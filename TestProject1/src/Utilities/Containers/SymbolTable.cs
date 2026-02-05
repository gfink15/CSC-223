/**
 * Symbol Table Class: Provide functionality for
 * a symbol table data structure with parent scope support.
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/4/2026
 */

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Text;

namespace Utilities.Containers;

/// <summary>
/// A generic symbol table implementation that supports hierarchical scoping through parent references.
/// Implements the <see cref="IDictionary{TKey, TValue}"/> interface using two parallel doubly linked lists
/// for storing keys and values.
/// </summary>
/// <typeparam name="TKey">The type of keys in the symbol table.</typeparam>
/// <typeparam name="TValue">The type of values in the symbol table.</typeparam>
public class SymbolTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private DLL<TKey> _KeyDLL = new DLL<TKey>();
    private DLL<TValue> _ValueDLL = new DLL<TValue>();
    private SymbolTable<TKey, TValue>? _parent;
    private int _sz = 0;

    /// <summary>
    /// Gets the parent symbol table for hierarchical scope lookup.
    /// </summary>
    /// <value>The parent <see cref="SymbolTable{TKey, TValue}"/> or null if this is the root scope.</value>
    public SymbolTable<TKey, TValue> Parent
    {
        get { return _parent; }
    }

    /// <summary>
    /// Gets a collection containing all keys in the local symbol table.
    /// </summary>
    /// <value>An <see cref="ICollection{TKey}"/> containing the keys in this symbol table.</value>
    public ICollection<TKey> Keys
    {
        get { return _KeyDLL; }
    }

    /// <summary>
    /// Gets a collection containing all values in the local symbol table.
    /// </summary>
    /// <value>An <see cref="ICollection{TValue}"/> containing the values in this symbol table.</value>
    public ICollection<TValue> Values
    {
        get { return _ValueDLL; }
    }

    /// <summary>
    /// Gets the number of key-value pairs contained in the local symbol table.
    /// </summary>
    /// <value>The number of elements in this symbol table (excludes parent entries).</value>
    public int Count
    {
        get { return _sz; }
    }

    /// <summary>
    /// Gets a value indicating whether the symbol table is read-only.
    /// </summary>
    /// <value>Always returns <c>false</c> as this symbol table supports modifications.</value>
    public bool IsReadOnly
    {
        get { return false; }
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <value>The value associated with the specified key.</value>
    /// <exception cref="KeyNotFoundException">Thrown when getting a value and the key does not exist in this table or any parent.</exception>
    /// <remarks>
    /// When getting, searches the local table first, then recursively searches parent tables.
    /// When setting, updates the local table if the key exists locally, otherwise adds a new entry.
    /// </remarks>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolTable{TKey, TValue}"/> class with an optional parent scope.
    /// </summary>
    /// <param name="parent">The parent symbol table for hierarchical scope lookup, or null for a root-level table.</param>
    public SymbolTable(SymbolTable<TKey, TValue>? parent)
    {
        _parent = parent;
    }

    /// <summary>
    /// Determines whether the local symbol table contains the specified key (does not check parent).
    /// </summary>
    /// <param name="key">The key to locate in the local symbol table.</param>
    /// <returns><c>true</c> if the key exists in the local table; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public bool ContainsKeyLocal(TKey key)
    {
        if (key == null) throw new ArgumentNullException("Key is null");
        return _KeyDLL.Contains(key);
    }

    /// <summary>
    /// Attempts to get the value associated with the specified key from the local table only.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the key if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if the key was found in the local table and the value is not null; otherwise, <c>false</c>.</returns>
    public bool TryGetValueLocal(TKey key, out TValue? value)
    {
        if (ContainsKeyLocal(key))
        {
            int keyIndex = _KeyDLL.IndexOf(key);
            value = _ValueDLL[keyIndex];

            if (value == null && this.Parent != null) return false;

            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Adds a key-value pair to the local symbol table.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. Can be null.</param>
    /// <exception cref="ArgumentException">Thrown when the key already exists in the local table.</exception>
    public void Add(TKey key, TValue? value)
    {
        if (ContainsKeyLocal(key)) throw new ArgumentException("Key already exists");
        else
        {
            _KeyDLL.Add(key);
            _ValueDLL.Add(value);
            _sz++;
        }
    }

    /// <summary>
    /// Determines whether the symbol table or any of its parent tables contain the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns><c>true</c> if the key exists in this table or any parent table; otherwise, <c>false</c>.</returns>
    public bool ContainsKey(TKey key)
    {
        if (!ContainsKeyLocal(key))
        {
            if (_parent != null)
            {
                return _parent.ContainsKey(key);
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Removes the element with the specified key from the local symbol table.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns><c>true</c> if the element was successfully removed; otherwise, <c>false</c> if the key was not found.</returns>
    public bool Remove(TKey key)
    {
        if (!ContainsKey(key)) return false;//throw new KeyNotFoundException("Key not found");
        int indexToRemove = _KeyDLL.IndexOf(key);
        _KeyDLL.RemoveAt(indexToRemove);
        _ValueDLL.RemoveAt(indexToRemove);
        _sz--;
        return true;
    }

    /// <summary>
    /// Attempts to get the value associated with the specified key, searching parent scopes if necessary.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the key if found; otherwise, the default value.</param>
    /// <returns><c>true</c> if the key was found in this table or any parent; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {   

        if (key == null) throw new ArgumentNullException();


        bool valueExists = this.TryGetValueLocal(key, out value);
        if (valueExists == false)
        {
            if (this.Parent == null) return false;
            else return this.Parent.TryGetValue(key, out value);
        }
        else
        {
            return true;
        }


    }

    /// <summary>
    /// Adds a key-value pair to the local symbol table.
    /// </summary>
    /// <param name="item">The key-value pair to add.</param>
    /// <exception cref="ArgumentException">Thrown when the key already exists in the local table.</exception>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all key-value pairs from the local symbol table.
    /// </summary>
    /// <remarks>This does not affect parent tables.</remarks>
    public void Clear()
    {
        _KeyDLL.Clear();
        _ValueDLL.Clear();
        _sz = 0;
    }

    /// <summary>
    /// Determines whether the symbol table contains a specific key-value pair.
    /// </summary>
    /// <param name="item">The key-value pair to locate.</param>
    /// <returns><c>true</c> if the exact key-value pair is found; otherwise, <c>false</c>.</returns>
    /// <remarks>Searches parent tables if the key is not found locally.</remarks>
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

    /// <summary>
    /// Copies the elements of the local symbol table to an array, starting at the specified index.
    /// </summary>
    /// <param name="array">The destination array for the key-value pairs.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="arrayIndex"/> is less than zero.</exception>
    /// <exception cref="ArgumentException">Thrown when the array does not have enough space to hold all elements.</exception>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // Checking the conditions for processing and throwing exceptions if not met
        if (array == null) throw new ArgumentNullException();
        if ((array.Length - arrayIndex) < _sz ) throw new ArgumentException();
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();

        // Go through the SymbolTable starting at index 0 to the end.
        for (int tableIndex = 0; tableIndex < _sz; tableIndex++)
        {
            var newPair = new KeyValuePair<TKey, TValue>(_KeyDLL[tableIndex], _ValueDLL[tableIndex]);

            // Just add arrayIndex and tableIndex to get the next index to put pair at
            array[arrayIndex + tableIndex] = newPair;
        }

    }

    /// <summary>
    /// Removes a specific key-value pair from the local symbol table.
    /// </summary>
    /// <param name="item">The key-value pair to remove.</param>
    /// <returns><c>true</c> if the exact key-value pair was found and removed; otherwise, <c>false</c>.</returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (EqualityComparer<TValue>.Default.Equals(this[item.Key], item.Value)) return Remove(item.Key);
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the local symbol table's key-value pairs.
    /// </summary>
    /// <returns>An enumerator for the key-value pairs in the local table.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < _sz; i++)
        {
            yield return new KeyValuePair<TKey, TValue>(_KeyDLL[i], _ValueDLL[i]);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the local symbol table.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for the local table.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}