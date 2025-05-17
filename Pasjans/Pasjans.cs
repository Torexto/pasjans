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
  public bool IsFaceUp { get; set; }

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
  private readonly List<Card> _cards = new(52);

  private readonly List<List<Card>> _columns =
  [
    new(),
    new(),
    new(),
    new(),
    new(),
    new(),
    new()
  ];

  private readonly List<List<Card>> _endingStacks =
  [
    new(),
    new(),
    new(),
    new()
  ];

  private readonly Mode _mode;
  private readonly Stack<Move> _undoStack = new();
  private int _cardsStackIndex;
  public uint Moves;


  public Pasjans(Mode mode)
  {
    _mode = mode;

    GenerateCards();
    DealCards();
    FlipCards();
  }

  private void GenerateCards()
  {
    ClearCards();

    foreach (var type in Enum.GetValues<CardType>())
    foreach (var value in Enum.GetValues<CardValue>())
      _cards.Add(new Card(type, value));

    ShuffleCards();
  }

  private void ClearCards()
  {
    _cards.Clear();
  }

  private void ShuffleCards()
  {
    var index = _cards.Count;

    while (index > 1)
    {
      var randomIndex = Random.Shared.Next(index--);
      (_cards[index], _cards[randomIndex]) = (_cards[randomIndex], _cards[index]);
    }
  }

  private void DealCards()
  {
    var len = 1;

    foreach (var column in _columns)
    {
      for (var i = 0; i < len; i++)
      {
        column.Add(_cards.Last());
        _cards.RemoveAt(_cards.Count - 1);
      }

      len++;
    }
  }

  private void FlipCards()
  {
    foreach (var card in _cards) card.IsFaceUp = true;
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

    if (_cardsStackIndex <= _cards.Count)
    {
      Console.Write($"Pozostało: {_cards.Count - _cardsStackIndex} ");

      foreach (var card in _cards.GetRange(0, _cardsStackIndex))
        card.Print();
    }
    else
    {
      Console.Write($"Pozostało: 0 ");

      foreach (var card in _cards)
        card.Print();
    }

    Console.WriteLine();

    foreach (var stack in _endingStacks)
    {
      if (stack.Count > 0)
        stack.LastOrDefault()?.Print();
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
    if (fromColumnIndex > 7 || toColumnIndex > 7) return;

    var fromColumn = _columns[fromColumnIndex];
    var toColumn = _columns[toColumnIndex];

    if (fromColumn.Count <= fromRowIndex) return;

    var fromCard = fromColumn[fromRowIndex];
    if (!fromCard.IsFaceUp) return;

    var toCard = toColumn.LastOrDefault();

    if (fromCard.Value != CardValue.King)
    {
      if (toCard is null) return;

      if (fromCard.ToColor() == toCard.ToColor()) return;
      if (fromCard.Value + 1 != toCard.Value) return;
    }

    var stack = fromColumn.GetRange(fromRowIndex, fromColumn.Count - fromRowIndex);
    toColumn.AddRange(stack);
    fromColumn.RemoveRange(fromRowIndex, stack.Count);

    _undoStack.Push(new Move(MoveType.ColumnToColumn, [..stack], fromColumnIndex, toColumnIndex));

    Moves++;
  }

  public void DrawCards()
  {
    if (_cardsStackIndex >= _cards.Count)
    {
      _cardsStackIndex = 0;
      ShuffleCards();
      return;
    }

    _undoStack.Push(new Move(MoveType.Draw, new List<Card>(), -1, -1, _cardsStackIndex));
    _cardsStackIndex += _mode is Mode.Easy ? 1 : 3;

    Moves++;
  }

  public void MoveFromPileToColumn(int toColumnIndex)
  {
    if (_cardsStackIndex == 0) return;
    if (toColumnIndex > 7) return;

    var fromCard = _cards.GetRange(0, _cardsStackIndex).Last();
    var toColumn = _columns[toColumnIndex];

    var toCard = toColumn.LastOrDefault();

    if (fromCard.Value != CardValue.King)
    {
      if (toCard is null) return;

      if (fromCard.ToColor() == toCard.ToColor()) return;
      if (fromCard.Value + 1 != toCard.Value) return;
    }

    toColumn.Add(fromCard);
    _cards.Remove(fromCard);
    _cardsStackIndex--;

    _undoStack.Push(new Move(MoveType.PileToColumn, [fromCard], -1, toColumnIndex));

    Moves++;
  }

  public void MoveFromColumnToEndingStack(int fromColumnIndex)
  {
    if (fromColumnIndex > 7) return;

    var fromColumn = _columns[fromColumnIndex];
    var fromCard = fromColumn.LastOrDefault();

    if (fromCard is null) return;

    int toStackIndex;

    switch (fromCard.Type)
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
    if (fromCard.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (fromCard.Value - 1 != toCard.Value) return;
    }

    toColumn.Add(fromCard);
    fromColumn.RemoveAt(fromColumn.Count - 1);

    _undoStack.Push(new Move(MoveType.ColumnToEnding, [fromCard], fromColumnIndex, toStackIndex));

    Moves++;
  }

  public void MoveFromEndingStackToColumn(int fromStackIndex, int toColumnIndex)
  {
    if (toColumnIndex > 7) return;
    if (fromStackIndex > 4) return;

    var fromColumn = _endingStacks[fromStackIndex];
    var toColumn = _columns[toColumnIndex];

    var fromCard = fromColumn.LastOrDefault();
    var toCard = toColumn.LastOrDefault();

    if (fromCard is null || toCard is null) return;

    if (fromCard.ToColor() == toCard.ToColor()) return;
    if (fromCard.Value + 1 != toCard.Value) return;

    toColumn.Add(fromCard);
    fromColumn.RemoveAt(fromColumn.Count - 1);

    _undoStack.Push(new Move(MoveType.EndingToColumn, [fromCard], fromStackIndex, toColumnIndex));

    Moves++;
  }

  public bool CheckIfWin()
  {
    return _endingStacks.All(stack => stack.Count == 13);
  }

  public void MoveFromPileToEndingStack()
  {
    var spareCards = _cards.GetRange(0, _cardsStackIndex);
    var fromCard = spareCards.LastOrDefault();

    if (fromCard is null) return;

    int toStackIndex;

    switch (fromCard.Type)
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
    if (fromCard.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (fromCard.Value - 1 != toCard.Value) return;
    }

    toColumn.Add(fromCard);
    spareCards.RemoveAt(spareCards.Count - 1);
    _cards.Remove(fromCard);
    _cardsStackIndex--;
    _undoStack.Push(new Move(MoveType.PileToEnding, [fromCard], -1, toStackIndex));
    Moves++;
  }

  public void Undo()
  {
    if (_undoStack.Count == 0) return;

    var move = _undoStack.Pop();

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
        _cards.Insert(_cardsStackIndex, move.Cards[0]);
        _cardsStackIndex++;
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
        _cards.Insert(_cardsStackIndex, move.Cards[0]);
        _cardsStackIndex++;
        break;

      case MoveType.Draw:
        _cardsStackIndex = move.PreviousStackIndex;
        break;
    }

    Moves--;
  }
}