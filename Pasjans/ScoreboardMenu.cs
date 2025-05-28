using static System.Console;

namespace Pasjans
{
  /// <summary>
  /// Statyczne menu wyświetlające ranking wyników gry Pasjans.
  /// </summary>
  public static class ScoreboardMenu
  {
    /// <summary>
    /// Lista przechowująca wyniki (liczbę ruchów) graczy.
    /// </summary>
    public static readonly List<uint> Scores = [];

    /// <summary>
    /// Tworzy i wyświetla menu rankingu wyników.
    /// Sortuje listę wyników rosnąco i wyświetla je.
    /// Jeśli lista jest pusta, wyświetla komunikat o braku wyników.
    /// Po wyświetleniu wyników czeka na naciśnięcie dowolnego klawisza.
    /// </summary>
    public static void Create()
    {
      Clear();

      WriteLine("Ranking\n");

      if (Scores.Count > 0)
      {
        Scores.Sort();

        foreach (var score in Scores)
          WriteLine(score);
      }
      else
      {
        WriteLine("Brak wyników");
      }

      WriteLine("\nNaciśnij dowolny klawisz, aby wrócić");
      ReadKey();
    }
  }
}