namespace Pasjans
{
  /// <summary>
  /// Reprezentuje stos do przechowywania ostatnich ruchów gry z ograniczonym rozmiarem.
  /// </summary>
  public class UndoStack
  {
    private readonly LinkedList<Move> _stack = [];

    private readonly int _size;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UndoStack"/> o podanym maksymalnym rozmiarze.
    /// </summary>
    /// <param name="size">Maksymalna liczba ruchów, które można przechować na stosie.</param>
    public UndoStack(int size)
    {
      _size = size;
    }

    /// <summary>
    /// Dodaje nowy ruch na stos.
    /// Jeśli stos jest pełny, usuwa najstarszy ruch przed dodaniem nowego.
    /// </summary>
    /// <param name="move">Ruch do dodania na stos.</param>
    public void Push(Move move)
    {
      if (_stack.Count == _size)
        _stack.RemoveFirst();
      _stack.AddLast(move);
    }

    /// <summary>
    /// Usuwa i zwraca ostatni ruch ze stosu.
    /// </summary>
    /// <returns>Ostatni ruch lub <c>null</c>, jeśli stos jest pusty.</returns>
    public Move? Pop()
    {
      var move = _stack.LastOrDefault();
      if (move is not null)
        _stack.RemoveLast();
      return move;
    }
  }
}