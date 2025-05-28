namespace Pasjans;

/// <summary>
/// Pomocnicza klasa służąca do śledzenia aktualnego indeksu w ograniczonym zakresie.
/// </summary>
internal class IndexTracker(int max)
{
  private int _index = 1;

  /// <summary>
  /// Zwiększa indeks o 1, jeśli nie osiągnięto wartości maksymalnej.
  /// </summary>
  public void Next()
  {
    if (_index < max) _index++;
  }

  /// <summary>
  /// Zmniejsza indeks o 1, jeśli nie osiągnięto minimalnej wartości 1.
  /// </summary>
  public void Previous()
  {
    if (_index > 1) _index--;
  }

  /// <summary>
  /// Zwraca indeks jako ciąg znaków.
  /// </summary>
  /// <returns>Aktualny indeks w postaci tekstowej.</returns>
  public override string ToString()
  {
    return _index.ToString();
  }

  /// <summary>
  /// Aktualna wartość indeksu.
  /// </summary>
  public int Index => _index;
}