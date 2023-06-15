using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlSequence;

public enum PunchStatus
{
  OK = 0,
  MP,
  WP,
  DNF
}

public class Worker
{

  public int[] Process(IList<int> course, IList<int> punched)
  {
    if (course.Count == 0)
      return Array.Empty<int>();

    if (punched.Count == 0)
      return Enumerable.Repeat(-1, course.Count).ToArray();

    if (course.SequenceEqual(punched))
      return Enumerable.Range(0, course.Count).ToArray();

    var better_matches_found = 0;
    var sequences = new List<(int, int[])>();

    for (int l = 0; l < course.Count; l++)
    {
      for (int i = 0; i < punched.Count; i++)
      {
        if (course[l] != punched[i])
          continue;

        // this double loop is required to not stop the execution when first valid sequence is found
        // e.g. when clear was not done and there are more than one race inside the punches

        count_main++;

        var (matches, sequence) = GetSequence(Enumerable.Repeat(-1, course.Count).ToArray(), course, punched, l, i);
        if (matches >= better_matches_found)
        {
          better_matches_found = matches;
          sequences.Add((matches, sequence));
        }
      }
    }

    // in case of multiple races the sequence we want is the last one
    return sequences.LastOrDefault(s => s.Item1 == better_matches_found).Item2 ?? Enumerable.Repeat(-1, course.Count).ToArray();
  }

  int count = 0;
  int count_main = 0;
  public (int, int[]) GetSequence(
    int[] sequence,
    IList<int> course,
    IList<int> punched,
    int offset_course = 0,
    int offset_punched = 0,
    bool allow_recursion = true
    )
  {
    count++;
    int punched_index = offset_punched;
    int last_saved_index = offset_punched;

    List<(int, int[])> alternatives = new() { };

    var best_match = 0;
    int correct_punches = sequence.Count(n => n >= 0);

    for (int i = offset_course; i < course.Count; i++)
    {
      bool found = false;

      while (punched_index < punched.Count)
      {
        if (punched[punched_index++] == course[i])
        {
          sequence[i] = punched_index - 1;
          last_saved_index = punched_index;
          found = true;
          correct_punches++;
          break;
        }
        else
        {
          // found an extra control, two hypothesis:
          // - extra control, to be ignored (continue this execution)
          // - later control, with current missing (should go to the next course control, done with recursion)
          if (allow_recursion && course.Count < 51)
          {
            // this part is not strictly necessary.
            // It improves the sequence selection if a butterfly central control is missed
            // with many controls it requires too much time and can be skipped
            var (matches, sequence_alternative) = GetSequence(sequence.ToArray(), course, punched, i + 1, last_saved_index, allow_recursion: false);
            if (matches >= best_match)
            {
              alternatives.Add((matches, sequence_alternative));
              best_match = matches;
            }

          }
        }
      }

      if (!found)
      {
        // if the current control is not found
        // restart to previous index in order to look for the next control
        punched_index = last_saved_index;
      }
    }


    if (correct_punches >= best_match)
    {
      alternatives.Add((correct_punches, sequence));
      best_match = correct_punches;
    }

    return alternatives.Last(s => s.Item1 == best_match);
  }


  public bool CheckCorrectness(IList<int> course, IList<int> punched)
  {
    int punched_index = 0;

    for (int i = 0; i < course.Count; i++)
    {
      bool found = false;

      while (punched_index < punched.Count)
      {
        if (punched[punched_index++] == course[i])
        {
          found = true;
          break;
        }
      }

      if (!found)
        return false;
    }

    return true;
  }


  public PunchStatus GetPunchStatus(IList<int> sequence)
  {
    if (sequence.All(n => n >= 0))
      return PunchStatus.OK;



    if (sequence.Count > 3 && sequence.SkipLast(1).TakeLast(3).All(n => n < 0))
      return PunchStatus.DNF;

    var lastGoodPunchedIndex = 0;
    var lastGoodCourseIndex = 0;
    var previousMissing = false;
    for (int i = 0; i < sequence.Count; i++)
    {
      var s = sequence[i];
      if (s != -1)
      {
        if (previousMissing)
        {
          var punchedIndexDiff = s - lastGoodPunchedIndex;
          var courseIndexDiff = i - lastGoodCourseIndex;

          if (punchedIndexDiff < courseIndexDiff)
            return PunchStatus.MP;
        }

        lastGoodPunchedIndex = s;
        lastGoodCourseIndex = i;
      }
      else
      {
        if (i == sequence.Count - 1)
          return PunchStatus.MP;
        previousMissing = true;
      }
    }

    return PunchStatus.WP;
  }

}


