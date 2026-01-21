

using System.Diagnostics.Tracing;


/**
 * Doubly Linked List Class: Provide functionality for
 * a doubly linked list structure
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink
 * @date   1/21/2026
 */
public class DLL<T> : IEnumerable<T>, IList<T>
{
    private DNode<T> head;
    private DNode<T> tail;
    private int sz = 0;

    public DLL()
    {
        head = new DNode<T>();
        tail = new DNode<T>();
        head.next = tail;
        tail.prev = head;
    }
    private void Insert(DNode<T> node, T item)
    {
        DNode<T> temp = new DNode<T>();
        node.prev.next = temp;
        temp.prev = node.prev;
        temp.next = node;
        node.prev = temp;
        temp.data = item;
        sz++;
    }
    private void Remove(DNode<T> node)
    {
        node.prev.next = node.next;
        node.next.prev = node.prev;
        sz--;
    }
    private DNode<T> GetNode(int index)
    {
        DNode<T> current = head;
        if (index >= sz || index < 0) throw new ArgumentOutOfRangeException("Index out of range");
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        return current;
    }
    public bool Contains(T item)
    {
        DNode<T> current = head;
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            if (current.data.Equals(item)) return true;
        }
        return false;
    }
    public int Size()
    {
        return sz;
    }
    public override string ToString()
    {
        string output = "[";
        DNode<T> current = head;
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            output += current.data + ", ";
        }
        return output + "]";
    }
    public bool Remove(T item)
    {
        DNode<T> current = head;
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            if (current.data.Equals(item))
            {
                Remove(current);
                return true;
            }
        }
        return false;
    }
    public T Front()
    {
        if (sz <= 0) throw new InvalidOperationException("List is empty");
        return head.next.data;
    }
    public T Back()
    {
        if (sz <= 0) throw new InvalidOperationException("List is empty");
        return tail.prev.data;
    }
    public void PushFront(T item)
    {
        Insert(head.next, item);
    }
    public void PushBack(T item)
    {
        Insert(tail, item);
    }
    public T PopFront()
    {
        if (sz <= 0) throw new InvalidOperationException("List is empty");
        T tempData = head.next.data;
        Remove(head.next);
        return tempData;
    }
    public T PopBack()
    {
        if (sz <= 0) throw new InvalidOperationException("List is empty");
        T tempData = tail.prev.data;
        Remove(tail.prev);
        return tempData;
    }
    public void Clear()
    {
        head.next = tail;
        tail.prev = head;
    }
    public bool IsEmpty()
    {
        return sz == 0;
    }
    public int Count
    {
        get { return sz; }
    }
    public bool IsReadOnly
    { 
        get { return false; } 
    }
    public void Add(T item)
    {
        Insert(tail.prev, item);
    }
    public void Insert(int index, T item)
    {
        if (index >= sz || index < 0) throw new ArgumentOutOfRangeException("Index out of range");
        DNode<T> current = head;
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        Insert(current, item);
    }
    public int IndexOf(T item)
    {
        DNode<T> current = head;
        for (int i = 0; i < sz; i++)
        {
            current = current.next;
            if (current.data.Equals(item))
            {
                return i;
            }
        }
        return -1;
    }
    public T this[int index]
    {
        get { return this.GetNode(index).data; }
        set { this.GetNode(index).data = value; }
    }
    public void RemoveAt(int index)
    {
        if (index >= sz || index < 0) throw new ArgumentOutOfRangeException("Index out of range");
        DNode<T> current = head;
        for (int i = 0; i <= index; i++)
        {
            current = current.next;
        }
        Remove(current);
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException("Array is null");
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException("Index is negative");
        if (array.Length - (arrayIndex + 1) < sz) throw new ArgumentException("Not enough space in array");
        for (int i = arrayIndex; i < array.Length; i++)
        {
            array[i] = GetNode(i - arrayIndex).data;
        }
    }
    public IEnumerator<T> GetEnumerator()
    {
        return false;
    }
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return false;
    }
}







 public class DNode<T>
{
    public DNode<T> next;
    public DNode<T> prev;
    public T data;
    public DNode() {}
}