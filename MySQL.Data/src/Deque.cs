using System;
using System.Collections;
using System.Collections.Generic;

namespace MySql.Data
{
  internal sealed class Deque<T> : ICollection, IEnumerable<T>
  {
    private readonly LinkedList<T> linkedList = new LinkedList<T>();

    public Deque()
    {
    }

    public Deque(int capacity)
    {
    }

    public int Count => linkedList.Count;

    void ICollection.CopyTo(Array array, int index)
    {
      ((ICollection)linkedList).CopyTo(array, index);
    }

    object ICollection.SyncRoot => this;

    bool ICollection.IsSynchronized => false;

    public bool Contains(T value)
    {
      var comparer = EqualityComparer<T>.Default;
      foreach (var entry in this)
      {
        if (comparer.Equals(value, entry))
          return true;
      }

      return false;
    }

    public void EnqueueBack(T value)
    {
      linkedList.AddLast(value);
    }

    public void EnqueueFront(T value)
    {
      linkedList.AddFirst(value);
    }

    public T PeekBack()
    {
      AssertNotEmpty();

      return linkedList.Last.Value;
    }

    public T PeekFront()
    {
      AssertNotEmpty();

      return linkedList.First.Value;
    }

    public T DequeueBack()
    {
      var value = PeekBack();
      linkedList.RemoveLast();

      return value;
    }

    public T DequeueFront()
    {
      var value = PeekFront();
      linkedList.RemoveFirst();

      return value;
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (var item in linkedList)
        yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private void AssertNotEmpty()
    {
      if (Count == 0)
        throw new InvalidOperationException();
    }
  }
}
