using System.ComponentModel;
using static Pasjans.Utils;

namespace Pasjans;

public enum Mode
{
  Easy,
  Hard
}

public enum CardType
{
  [Description("♥")] Kier,
  [Description("♦")] Karo,
  [Description("♠")] Pik,
  [Description("♣")] Trefl
}

public enum CardValue
{
  [Description("A")] Ace,
  [Description("2")] Two,
  [Description("3")] Three,
  [Description("4")] Four,
  [Description("5")] Five,
  [Description("6")] Six,
  [Description("7")] Seven,
  [Description("8")] Eight,
  [Description("9")] Nine,
  [Description("10")] Ten,
  [Description("J")] Jack,
  [Description("Q")] Queen,
  [Description("K")] King
}

public enum MoveType
{
  ColumnToColumn,
  PileToColumn,
  ColumnToEnding,
  EndingToColumn,
  PileToEnding,
  Draw
}

public record Move(MoveType Type, List<Card> Cards, int FromIndex, int ToIndex, int PreviousStackIndex = 0);

public record Card(CardType Type, CardValue Value)
{
  public bool IsFaceUp { get; set; } = true;

  public override string ToString()
  {
    var description = $"{GetDescription(Value)}{GetDescription(Type)}";

    if (IsFaceUp) return description.Length < 3 ? description + " " : description;

    return "###";
  }

  public ConsoleColor ToColor()
  {
    return Type is CardType.Kier or CardType.Karo ? ConsoleColor.Black : ConsoleColor.Red;
  }

  public void Print()
  {
    Console.ForegroundColor = IsFaceUp
      ? ToColor()
      : ConsoleColor.White;
    Console.Write(ToString());
    Console.ForegroundColor = ConsoleColor.White;
  }
}

public class Pasjans
{
  public uint Moves { get; private set; }

  private const int UndoStackSize = 3;

  private readonly List<Card> _deck = new(52);
  private readonly UndoStack _undoStack = new(UndoStackSize);
  private readonly IReadOnlyList<List<Card>> _columns = CreateEmptyLists<Card>(7);
  private readonly IReadOnlyList<List<Card>> _endingStacks = CreateEmptyLists<Card>(4);
  private readonly Mode _mode;
  private int _drawPileIndex;

  public Pasjans(Mode mode)
  {
    _mode = mode;

    InitializeDeck();
    InitializeColumns();
  }

  private void InitializeDeck()
  {
    _deck.Clear();

    foreach (var type in Enum.GetValues<CardType>())
    foreach (var value in Enum.GetValues<CardValue>())
      _deck.Add(new Card(type, value));

    ShuffleDeck();
  }

  private void ShuffleDeck()
  {
    var index = _deck.Count;

    while (index > 1)
    {
      var randomIndex = Random.Shared.Next(index--);
      (_deck[index], _deck[randomIndex]) = (_deck[randomIndex], _deck[index]);
    }
  }

  private static void MoveCard(Card card, List<Card> from, List<Card> to)
  {
    if (!from.Contains(card)) return;

    to.Add(card);
    from.Remove(card);
  }

  private static void MoveCards(List<Card> from, List<Card> to, int count)
  {
    var range = from.GetRange(from.Count - count, count);
    to.AddRange(range);
    from.RemoveRange(from.Count - count, count);
  }

  private void InitializeColumns()
  {
    foreach (var (column, index) in _columns.Select((c, i) => (c, i)))
    {
      for (var i = 0; i <= index; i++)
      {
        var card = _deck.Last();
        card.IsFaceUp = false;
        MoveCard(card, _deck, column);
      }
    }
  }

  public void PrintBoard()
  {
    var max = _columns.Select(column => column.Count).Max();

    for (var i = 1; i <= 7; i++) Console.Write($"  {i} ");

    Console.WriteLine();

    for (var i = 0; i < max; i++)
    {
      foreach (var column in _columns)
      {
        Console.Write(" ");

        if (column.Count - 1 == i)
          column[i].IsFaceUp = true;

        if (column.Count <= i)
        {
          Console.Write("   ");
          continue;
        }

        var card = column[i];

        card.Print();
      }

      Console.WriteLine($" {i + 1}");
    }

    Console.Write($"Pozostało: {_deck.Count - _drawPileIndex} ");

    foreach (var card in _deck.GetRange(0, _drawPileIndex))
      card.Print();

    Console.WriteLine();

    foreach (var stack in _endingStacks)
    {
      if (stack.Count > 0)
        stack.Last().Print();
      else
        Console.Write("###");

      Console.Write(" ");
    }

    Console.WriteLine();

    Console.WriteLine(Moves);

    Console.WriteLine();
  }

  public void MoveBetweenColumns(int fromColumnIndex, int fromRowIndex, int toColumnIndex)
  {
    if (fromColumnIndex == toColumnIndex) return;

    if (fromColumnIndex < 0 || fromColumnIndex >= _columns.Count) return;
    if (toColumnIndex < 0 || toColumnIndex >= _columns.Count) return;

    var fromColumn = _columns[fromColumnIndex];
    var toColumn = _columns[toColumnIndex];

    if (fromRowIndex < 0 || fromRowIndex >= fromColumn.Count) return;

    var card = fromColumn[fromRowIndex];
    if (!card.IsFaceUp) return;

    var toCard = toColumn.LastOrDefault();

    if (!(card.Value == CardValue.King && toCard is null))
    {
      if (toCard is null) return;

      if (card.ToColor() == toCard.ToColor()) return;
      if (card.Value + 1 != toCard.Value) return;
    }

    var count = fromColumn.Count - fromRowIndex;
    var stack = fromColumn.GetRange(fromRowIndex, count);

    MoveCards(fromColumn, toColumn, count);

    _undoStack.Push(new Move(MoveType.ColumnToColumn, [..stack], fromColumnIndex, toColumnIndex));

    Moves++;
  }

  public void DrawCards()
  {
    if (_deck.Count == 0) return;

    if (_drawPileIndex >= _deck.Count)
    {
      _drawPileIndex = 0;
      ShuffleDeck();
      return;
    }

    _undoStack.Push(new Move(MoveType.Draw, [], -1, -1, _drawPileIndex));

    _drawPileIndex += _mode == Mode.Easy ? 1 : 3;

    _drawPileIndex = Math.Clamp(_drawPileIndex, 0, _deck.Count);

    Moves++;
  }

  public void MoveFromPileToColumn(int toColumnIndex)
  {
    if (toColumnIndex < 0 || toColumnIndex >= _columns.Count) return;
    if (_drawPileIndex == 0) return;

    var card = _deck[_drawPileIndex - 1];

    var toColumn = _columns[toColumnIndex];
    var toCard = toColumn.LastOrDefault();

    if (!(card.Value == CardValue.King && toCard is null))
    {
      if (toCard is null) return;

      if (card.ToColor() == toCard.ToColor()) return;
      if (card.Value + 1 != toCard.Value) return;
    }

    MoveCard(card, _deck, toColumn);

    _undoStack.Push(new Move(MoveType.PileToColumn, [card], -1, toColumnIndex));

    _drawPileIndex--;
    Moves++;
  }

  public void MoveFromColumnToEndingStack(int fromColumnIndex)
  {
    if (fromColumnIndex > 7) return;

    var fromColumn = _columns[fromColumnIndex];
    var card = fromColumn.LastOrDefault();

    if (card is null) return;

    int toStackIndex;

    switch (card.Type)
    {
      case CardType.Karo:
        toStackIndex = 0;
        break;
      case CardType.Kier:
        toStackIndex = 1;
        break;
      case CardType.Pik:
        toStackIndex = 2;
        break;
      case CardType.Trefl:
        toStackIndex = 3;
        break;
      default:
        return;
    }

    var toColumn = _endingStacks[toStackIndex];

    var toCard = toColumn.LastOrDefault();
    if (card.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (card.Value - 1 != toCard.Value) return;
    }

    MoveCard(card, fromColumn, toColumn);

    _undoStack.Push(new Move(MoveType.ColumnToEnding, [card], fromColumnIndex, toStackIndex));

    Moves++;
  }

  public void MoveFromEndingStackToColumn(int fromStackIndex, int toColumnIndex)
  {
    if (toColumnIndex > 7) return;
    if (fromStackIndex > 4) return;

    var fromColumn = _endingStacks[fromStackIndex];
    var toColumn = _columns[toColumnIndex];

    var card = fromColumn.LastOrDefault();
    var toCard = toColumn.LastOrDefault();

    if (card is null || toCard is null) return;

    if (card.ToColor() == toCard.ToColor()) return;
    if (card.Value + 1 != toCard.Value) return;

    MoveCard(card, fromColumn, toColumn);

    _undoStack.Push(new Move(MoveType.EndingToColumn, [card], fromStackIndex, toColumnIndex));

    Moves++;
  }

  public bool CheckIfWin()
  {
    return _endingStacks.All(stack => stack.Count == 13);
  }

  public void MoveFromPileToEndingStack()
  {
    var spareCards = _deck.GetRange(0, _drawPileIndex);
    var card = spareCards.LastOrDefault();

    if (card is null) return;

    int toStackIndex;

    switch (card.Type)
    {
      case CardType.Karo:
        toStackIndex = 0;
        break;
      case CardType.Kier:
        toStackIndex = 1;
        break;
      case CardType.Pik:
        toStackIndex = 2;
        break;
      case CardType.Trefl:
        toStackIndex = 3;
        break;
      default:
        return;
    }

    var toColumn = _endingStacks[toStackIndex];

    var toCard = toColumn.LastOrDefault();
    if (card.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (card.Value - 1 != toCard.Value) return;
    }

    MoveCard(card, _deck, toColumn);

    _drawPileIndex--;
    _undoStack.Push(new Move(MoveType.PileToEnding, [card], -1, toStackIndex));
    Moves++;
  }

  public void Undo()
  {
    var move = _undoStack.Pop();

    if (move is null) return;

    switch (move.Type)
    {
      case MoveType.ColumnToColumn:
        var toCol = _columns[move.ToIndex];
        var fromCol = _columns[move.FromIndex];
        for (var i = 0; i < move.Cards.Count; i++) toCol.RemoveAt(toCol.Count - 1);
        fromCol.AddRange(move.Cards);
        break;

      case MoveType.PileToColumn:
        _columns[move.ToIndex].RemoveAt(_columns[move.ToIndex].Count - 1);
        _deck.Insert(_drawPileIndex, move.Cards[0]);
        _drawPileIndex++;
        break;

      case MoveType.ColumnToEnding:
        _endingStacks[move.ToIndex].RemoveAt(_endingStacks[move.ToIndex].Count - 1);
        _columns[move.FromIndex].Add(move.Cards[0]);
        break;

      case MoveType.EndingToColumn:
        _columns[move.ToIndex].RemoveAt(_columns[move.ToIndex].Count - 1);
        _endingStacks[move.FromIndex].Add(move.Cards[0]);
        break;

      case MoveType.PileToEnding:
        _endingStacks[move.ToIndex].RemoveAt(_endingStacks[move.ToIndex].Count - 1);
        _deck.Insert(_drawPileIndex, move.Cards[0]);
        _drawPileIndex++;
        break;

      case MoveType.Draw:
        _drawPileIndex = move.PreviousStackIndex;
        break;
    }

    Moves--;
  }
}