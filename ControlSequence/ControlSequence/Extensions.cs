using System.Collections.Generic;

namespace System.Linq;

public static class Extensions
{
  public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
  {
    var buffer = new List<T>();
    int pos = 0;

    foreach (var item in source)
    {
      if (buffer.Count < count)
      {
        // phase 1
        buffer.Add(item);
      }
      else
      {
        // phase 2
        buffer[pos] = item;
        pos = (pos + 1) % count;
      }
    }

    for (int i = 0; i < buffer.Count; i++)
    {
      yield return buffer[pos];
      pos = (pos + 1) % count;
    }
  }

  public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
  {
    var buffer = new List<T>();
    int pos = 0;

    foreach (var item in source)
    {
      if (buffer.Count < count)
      {
        // phase 1
        buffer.Add(item);
      }
      else
      {
        // phase 2
        yield return buffer[pos];
        buffer[pos] = item;
        pos = (pos + 1) % count;
      }
    }
  }
}

