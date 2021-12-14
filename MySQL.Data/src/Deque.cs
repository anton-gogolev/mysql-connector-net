using System;
using System.Collections;
using System.Collections.Generic;

namespace MySql.Data
{
  internal sealed class Deque<T> : ICollection, IEnumerable<T>
  {
    private const int DefaultCapacity = 8;

    private T[] buffer;
    private int offset;

    public Deque() :
      this(DefaultCapacity)
    {
    }

    public Deque(int capacity)
    {
      buffer = new T[capacity];
    }

    public int Capacity
    {
      get => buffer.Length;
      set => EnsureCapacity(value);
    }

    public int Count { get; private set; }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof(array));

      CopyToArray(array, index);
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
      EnsureCapacity();

      buffer[(Count + offset) % Capacity] = value;
      ++Count;
    }

    public void EnqueueFront(T value)
    {
      EnsureCapacity();

      offset -= 1;
      if (offset < 0)
        offset += Capacity;

      buffer[offset] = value;
      ++Count;
    }

    public T PeekBack()
    {
      AssertNotEmpty();

      var value = buffer[(Count - 1 + offset) % Capacity];

      return value;
    }

    public T PeekFront()
    {
      AssertNotEmpty();

      return buffer[offset];
    }

    public T DequeueBack()
    {
      var value = PeekBack();
      --Count;

      return value;
    }

    public T DequeueFront()
    {
      AssertNotEmpty();

      var value = PeekFront();

      offset = (offset + 1) % Capacity;
      --Count;

      return value;
    }

    public IEnumerator<T> GetEnumerator()
    {
      var count = Count;
      for (var i = 0; i != count; ++i)
      {
        var index = (i + offset) % Capacity;
        yield return buffer[index];
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private void EnsureCapacity()
    {
      if (Count < Capacity)
        return;

      EnsureCapacity(Capacity == 0 ? 1 : Capacity * 2);
    }

    private void EnsureCapacity(int value)
    {
      if (value < Count)
        throw new ArgumentOutOfRangeException(nameof(value));

      if (value == buffer.Length)
        return;

      var reallocatedBuffer = new T[value];
      CopyToArray(reallocatedBuffer);

      buffer = reallocatedBuffer;
      offset = 0;
    }

    private void CopyToArray(Array array, int index = 0)
    {
      if (array == null)
        throw new ArgumentNullException(nameof(array));

      if (offset > Capacity - Count)
      {
        var length = Capacity - offset;

        Array.Copy(buffer, offset, array, index, length);
        Array.Copy(buffer, 0, array, index + length, Count - length);
      }
      else
      {
        Array.Copy(buffer, offset, array, index, Count);
      }
    }


    private void AssertNotEmpty()
    {
      if(Count == 0)
        throw new InvalidOperationException();
    }
  }
}
