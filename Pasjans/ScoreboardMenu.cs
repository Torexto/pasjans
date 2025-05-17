using static System.Console;

namespace Pasjans;

public abstract class ScoreboardMenu
{
  public static readonly List<uint> Scores = [];

  public static void Create()
  {
    Clear();

    WriteLine("Ranking\n");

    if (Scores.Count > 0)
    {
      Scores.Sort();

      foreach (var score in Scores) WriteLine(score);
    }
    else
    {
      WriteLine("Brak wyników");
    }

    WriteLine("\nNaciśnij dowolny klawisz, aby wrócić");
    ReadKey();
  }
}