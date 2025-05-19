namespace Pasjans;

public class UndoStack(int size)
{
  private readonly LinkedList<Move> _stack = [];

  public void Push(Move move)
  {
    if (_stack.Count == size) _stack.RemoveFirst();
    _stack.AddLast(move);
  }

  public Move? Pop()
  {
    var move = _stack.LastOrDefault();
    if (move is not null)
      _stack.RemoveLast();
    return move;
  }
}