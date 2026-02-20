/**
 * Doubly Linked List Class: Provide functionality for
 * a doubly linked list structure
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   1/21/2026
 */

using System.Collections;
using System.Diagnostics.Tracing;
using System.Text;

namespace Utilities.Containers;

/// <summary>
/// A generic doubly linked list implementation that provides efficient insertion
/// and deletion operations at both ends and anywhere in the list.
/// Uses sentinel head and tail nodes to simplify edge case handling.
/// </summary>
/// <typeparam name="T">The type of elements stored in the list.</typeparam>
public class DLL<T> : IEnumerable<T>, IList<T>
{
    /// <summary>
    /// Represents a node in the doubly linked list.
    /// Each node contains data and pointers to the previous and next nodes.
    /// </summary>
    public class DNode
    {
        /// <summary>Reference to the next node in the list.</summary>
        public DNode? next;
        
        /// <summary>Reference to the previous node in the list.</summary>
        public DNode? prev;
        
        /// <summary>The data stored in this node.</summary>
        public T? data;
        
        /// <summary>
        /// Default constructor that initializes an empty node.
        /// </summary>
        public DNode() {}
    }
    
    // Sentinel head node - marks the beginning of the list
    private DNode head;
    
    // Sentinel tail node - marks the end of the list
    private DNode tail;
    
    // Tracks the current number of elements in the list
    private int sz = 0;

    /// <summary>
    /// Initializes a new instance of the doubly linked list.
    /// Creates sentinel head and tail nodes that serve as boundaries.
    /// </summary>
    public DLL()
    {
        // Create sentinel nodes
        head = new DNode();
        tail = new DNode();
        
        // Link sentinels together to form an empty list
        head.next = tail;
        tail.prev = head;
    }
    
    /// <summary>
    /// Private helper method that inserts a new node containing the specified item
    /// immediately before the given node.
    /// </summary>
    /// <param name="node">The node before which to insert the new element.</param>
    /// <param name="item">The item to store in the new node.</param>
    private void Insert(DNode node, T? item)
    {
        // Create a new node to hold the item
        DNode temp = new DNode();
        
        // Link the new node into the list before the specified node
        node.prev.next = temp;  // Previous node points forward to new node
        temp.prev = node.prev;   // New node points back to previous node
        temp.next = node;        // New node points forward to specified node
        node.prev = temp;        // Specified node points back to new node
        
        // Store the data in the new node
        temp.data = item;
        
        // Increment the size counter
        sz++;
    }
    
    /// <summary>
    /// Private helper method that removes the specified node from the list.
    /// </summary>
    /// <param name="node">The node to remove from the list.</param>
    private void Remove(DNode node)
    {
        // Bypass the node by linking its neighbors to each other
        node.prev.next = node.next;
        node.next.prev = node.prev;
        
        // Decrement the size counter
        sz--;
    }
    
    /// <summary>
    /// Private helper method that retrieves the node at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the node to retrieve.</param>
    /// <returns>The node at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the index is negative or greater than or equal to the list size.
    /// </exception>
    private DNode GetNode(int index)
    {
        // Start at the sentinel head node
        DNode current = head;
        
        // Validate the index is within bounds
        if (index >= sz || index < 0) 
            throw new ArgumentOutOfRangeException("Index out of range");
        
        // Traverse to the node at the specified index
        // (i <= index because we start at head and need to advance past it)
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        
        return current;
    }
    
    /// <summary>
    /// Determines whether the list contains the specified item.
    /// </summary>
    /// <param name="item">The item to locate in the list.</param>
    /// <returns>True if the item is found; otherwise, false.</returns>
    public bool Contains(T? item)
    {
        // Start at the sentinel head node
        DNode current = head;
        
        // Traverse through all valid nodes in the list
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            
            // Safety check: if we reach tail, item is not in list
            if (current == tail) return false;
            
            // Use EqualityComparer for type-safe comparison
            if (EqualityComparer<T>.Default.Equals(current.data, item)) 
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Returns the number of elements in the list.
    /// </summary>
    /// <returns>The count of elements currently stored in the list.</returns>
    public int Size()
    {
        return sz;
    }
    
    /// <summary>
    /// Returns a string representation of the list for debugging purposes.
    /// Concatenates all element values in order from front to back.
    /// </summary>
    /// <returns>A string containing all elements in the list.</returns>
    public override string ToString()
    {
        // Use StringBuilder for efficient string concatenation
        StringBuilder output = new StringBuilder();
        
        // Start traversal at the sentinel head
        DNode current = head;
        
        // Append each element's data to the output string
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            output.Append(current.data);
        }
        
        return output.ToString();
    }
    
    /// <summary>
    /// Removes the first occurrence of the specified item from the list.
    /// </summary>
    /// <param name="item">The item to remove from the list.</param>
    /// <returns>True if the item was found and removed; otherwise, false.</returns>
    public bool Remove(T? item)
    {
        // Start at the sentinel head node
        DNode current = head;
        
        // Search through the list for the item
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            
            // Check if current node contains the target item and is not the tail
            if (EqualityComparer<T>.Default.Equals(current.data, item) && current != tail)
            {
                // Remove the node and report success
                Remove(current);
                return true;
            }
        }
        
        // Item not found
        return false;
    }
    
    /// <summary>
    /// Returns the element at the front of the list without removing it.
    /// </summary>
    /// <returns>The first element in the list.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access the front of an empty list.
    /// </exception>
    public T Front()
    {
        // Verify the list is not empty
        if (sz <= 0) 
            throw new InvalidOperationException("List is empty");
        
        // Return the data from the first valid node (after head sentinel)
        return head.next.data;
    }
    
    /// <summary>
    /// Returns the element at the back of the list without removing it.
    /// </summary>
    /// <returns>The last element in the list.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access the back of an empty list.
    /// </exception>
    public T Back()
    {
        // Verify the list is not empty
        if (sz <= 0) 
            throw new InvalidOperationException("List is empty");
        
        // Return the data from the last valid node (before tail sentinel)
        return tail.prev.data;
    }
    
    /// <summary>
    /// Adds the specified item to the front of the list.
    /// </summary>
    /// <param name="item">The item to add to the front of the list.</param>
    public void PushFront(T item)
    {
        // Insert before the first valid node (which is head.next)
        Insert(head.next, item);
    }
    
    /// <summary>
    /// Adds the specified item to the back of the list.
    /// </summary>
    /// <param name="item">The item to add to the back of the list.</param>
    public void PushBack(T item)
    {
        // Insert before the tail sentinel (making it the last valid node)
        Insert(tail, item);
    }
    
    /// <summary>
    /// Removes and returns the element at the front of the list.
    /// </summary>
    /// <returns>The element that was removed from the front.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove from an empty list.
    /// </exception>
    public T PopFront()
    {
        // Verify the list is not empty
        if (sz <= 0) 
            throw new InvalidOperationException("List is empty");
        
        // Save the data before removing the node
        T tempData = head.next.data;
        
        // Remove the first valid node
        Remove(head.next);
        
        return tempData;
    }
    
    /// <summary>
    /// Removes and returns the element at the back of the list.
    /// </summary>
    /// <returns>The element that was removed from the back.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to remove from an empty list.
    /// </exception>
    public T PopBack()
    {
        // Verify the list is not empty
        if (sz <= 0) 
            throw new InvalidOperationException("List is empty");
        
        // Save the data before removing the node
        T tempData = tail.prev.data;
        
        // Remove the last valid node
        Remove(tail.prev);
        
        return tempData;
    }
    
    /// <summary>
    /// Removes all elements from the list, leaving only the sentinel nodes.
    /// </summary>
    public void Clear()
    {
        // Re-link sentinels to create an empty list
        head.next = tail;
        tail.prev = head;
        
        // Reset the size counter
        sz = 0;
    }
    
    /// <summary>
    /// Determines whether the list contains any elements.
    /// </summary>
    /// <returns>True if the list is empty; otherwise, false.</returns>
    public bool IsEmpty()
    {
        return sz == 0;
    }
    
    /// <summary>
    /// Gets the number of elements contained in the list.
    /// This property is part of the ICollection interface.
    /// </summary>
    public int Count
    {
        get { return sz; }
    }
    
    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// Always returns false as this implementation supports modification.
    /// </summary>
    public bool IsReadOnly
    { 
        get { return false; } 
    }
    
    /// <summary>
    /// Appends an item to the end of the list.
    /// This method is part of the ICollection interface.
    /// </summary>
    /// <param name="item">The item to add to the list.</param>
    public void Add(T? item)
    {
        // Insert before the tail sentinel (equivalent to PushBack)
        Insert(tail, item);
    }
    
    /// <summary>
    /// Inserts an item at the specified index, shifting subsequent elements down.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the item.</param>
    /// <param name="item">The item to insert into the list.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the index is negative or greater than the list size.
    /// Note that inserting at index equal to Count is allowed (appends to end).
    /// </exception>
    public void Insert(int index, T? item)
    {
        // Validate the index (can insert at Count to append)
        if (index > sz || index < 0) 
            throw new ArgumentOutOfRangeException("Index out of range");
        
        // Start at the sentinel head
        DNode current = head;
        
        // Traverse to the node that will follow the new node
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        
        // Insert the new item before the current node
        Insert(current, item);
    }
    
    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified item.
    /// </summary>
    /// <param name="item">The item to locate in the list.</param>
    /// <returns>
    /// The index of the first occurrence if found; otherwise, -1.
    /// </returns>
    public int IndexOf(T item)
    {
        // Start at the sentinel head
        DNode current = head;
        
        // Search through all valid nodes
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            
            // Check if current node contains the target item
            if (EqualityComparer<T>.Default.Equals(current.data, item))
            {
                return i;
            }
        }
        
        // Item not found
        return -1;
    }
    
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// Provides array-like indexer access to list elements.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the index is negative or greater than or equal to Count.
    /// </exception>
    public T this[int index]
    {
        // Getter retrieves the data from the node at the specified index
        get { return this.GetNode(index).data; }
        
        // Setter updates the data in the node at the specified index
        set { this.GetNode(index).data = value; }
    }
    
    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the index is negative or greater than or equal to Count.
    /// </exception>
    public void RemoveAt(int index)
    {
        // Validate the index is within bounds
        if (index >= sz || index < 0) 
            throw new ArgumentOutOfRangeException("Index out of range");
        
        // Start at the sentinel head
        DNode current = head;
        
        // Traverse to the node at the specified index
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        
        // Remove the node
        Remove(current);
    }
    
    /// <summary>
    /// Copies all elements from the list to a compatible array, starting at
    /// the specified array index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional array that is the destination of the elements.
    /// </param>
    /// <param name="arrayIndex">
    /// The zero-based index in the array at which copying begins.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the array is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the array index is negative.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when there is insufficient space in the array to hold all elements
    /// from the list starting at the specified index.
    /// </exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        // Validate parameters
        if (array == null) 
            throw new ArgumentNullException("Array is null");
        if (arrayIndex < 0) 
            throw new ArgumentOutOfRangeException("Index is negative");
        if (array.Length - arrayIndex < sz) 
            throw new ArgumentException("Not enough space in array");
        
        // Copy each element from the list to the array
        for (int i = arrayIndex; i < array.Length; i++)
        {
            // Calculate the corresponding list index
            if ((i - arrayIndex) < sz) 
                array[i] = GetNode(i - arrayIndex).data;
            else 
                break;  // Stop when all list elements have been copied
        }
    }
    
    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// Enables foreach loops and LINQ queries on the list.
    /// </summary>
    /// <returns>An IEnumerator that can be used to iterate through the list.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        // Start at the sentinel head
        DNode current = head;
        
        // Yield each element in sequence
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            yield return current.data;
        }
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the list.
    /// Required for IEnumerable interface implementation.
    /// </summary>
    /// <returns>An IEnumerator that can be used to iterate through the list.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        // Delegate to the generic version
        return GetEnumerator();
    }
}