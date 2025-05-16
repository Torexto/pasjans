using System.ComponentModel;

namespace Pasjans;

public enum Mode
{
  Easy,
  Hard
}

public enum Color
{
  Red,
  Black
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
  [Description("A")] Ace = 1,
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

public record Card(CardType Type, CardValue Value)
{
  public bool IsFaceUp { get; set; }

  private static string GetDescription(Enum value)
  {
    var field = value.GetType().GetField(value.ToString());
    var attributes = (DescriptionAttribute[])field!.GetCustomAttributes(typeof(DescriptionAttribute), false);
    return attributes.Length > 0 ? attributes[0].Description : value.ToString();
  }

  public override string ToString()
  {
    var description = $"{GetDescription(Value)}{GetDescription(Type)}";

    return IsFaceUp ? (description.Length < 3 ? description += " " : description) : "###";
  }

  public ConsoleColor ToColor()
  {
    return Type is CardType.Kier or CardType.Karo ? ConsoleColor.Black : ConsoleColor.Red;
  }

  public bool IsKing()
  {
    return Value == CardValue.King;
  }

  public void Print()
  {
    Console.ForegroundColor = IsFaceUp
      ? ToColor()
      : ConsoleColor.White;
    Console.Write(ToString());
    Console.ForegroundColor = ConsoleColor.White;
  }
};

public class Pasjans
{
  private readonly List<Card> _cards = new List<Card>(52);

  private readonly List<List<Card>> _endingStacks =
  [
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
  ];

  private readonly List<List<Card>> _columns =
  [
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
    new List<Card>(),
    new List<Card>()
  ];

  private readonly Mode _mode;
  private int _cardsStackIndex;

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
    {
      foreach (var value in Enum.GetValues<CardValue>())
      {
        _cards.Add(new Card(type, value));
      }
    }

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
    foreach (var card in _cards)
    {
      card.IsFaceUp = true;
    }
  }

  public void PrintBoard()
  {
    var max = 0;

    foreach (var column in _columns)
    {
      max = Math.Max(max, column.Count);
    }

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

      Console.WriteLine();
    }

    Console.Write($"Pozostało: {_cards.Count - _cardsStackIndex} ");

    foreach (var card in _cards.GetRange(0, _cardsStackIndex))
    {
      card.Print();
    }

    Console.WriteLine();

    foreach (var stack in _endingStacks)
    {
      stack.LastOrDefault()?.Print();
    }

    Console.WriteLine();

    Console.WriteLine(CheckIfWin());

    Console.WriteLine();
  }

  public void MoveCardFromColumnToColumn(int fromColumnIndex, int fromRowIndex, int toColumnIndex)
  {
    if (fromColumnIndex == toColumnIndex) return;
    if (fromColumnIndex > 7 || toColumnIndex > 7) return;

    var fromColumn = _columns[fromColumnIndex];
    var toColumn = _columns[toColumnIndex];

    var fromCard = fromColumn[fromRowIndex];
    if (!fromCard.IsFaceUp) return;

    var toCard = toColumn.LastOrDefault();

    if (!fromCard.IsKing())
    {
      if (toCard is null) return;

      if (fromCard.ToColor() == toCard?.ToColor()) return;
      if (fromCard.Value + 1 != toCard?.Value) return;
    }

    var stack = fromColumn.GetRange(fromRowIndex, fromColumn.Count - fromRowIndex);
    toColumn.AddRange(stack);
    fromColumn.RemoveRange(fromRowIndex, stack.Count);
  }

  public void DrawCards()
  {
    if (_cardsStackIndex >= _cards.Count)
    {
      _cardsStackIndex = 0;
      ShuffleCards();
      return;
    }

    _cardsStackIndex += _mode is Mode.Easy ? 1 : 3;
  }

  public void MoveCardFromSpareToColumn(int toColumnIndex)
  {
    if (toColumnIndex > 7) return;

    var fromCard = _cards.GetRange(0, _cardsStackIndex).Last();
    var toColumn = _columns[toColumnIndex];

    var toCard = toColumn.LastOrDefault();

    if (!fromCard.IsKing())
    {
      if (toCard is null) return;

      if (fromCard.ToColor() == toCard?.ToColor()) return;
      if (fromCard.Value + 1 != toCard?.Value) return;
    }

    toColumn.Add(fromCard);
    _cards.Remove(fromCard);
    _cardsStackIndex--;
  }

  public void MoveCardFromColumnToEndingStack(int toColumnIndex)
  {
    if (toColumnIndex > 7) return;

    var fromColumn = _columns[toColumnIndex];
    var fromCard = fromColumn.LastOrDefault();

    if (fromCard is null) return;

    List<Card>? toColumn;

    switch (fromCard.Type)
    {
      case CardType.Karo:
        toColumn = _endingStacks[0];
        break;
      case CardType.Kier:
        toColumn = _endingStacks[1];
        break;
      case CardType.Pik:
        toColumn = _endingStacks[2];
        break;
      case CardType.Trefl:
        toColumn = _endingStacks[3];
        break;
      default:
        return;
    }

    var toCard = toColumn.LastOrDefault();
    if (fromCard.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (fromCard.Value - 1 != toCard?.Value) return;
    }

    toColumn.Add(fromCard);
    fromColumn.RemoveAt(fromColumn.Count - 1);
  }

  public void MoveCardFromEndingStackToColumn(int fromColumnIndex, int toColumnIndex)
  {
    if (toColumnIndex > 7) return;
    if (fromColumnIndex > 4) return;

    var fromColumn = _endingStacks[fromColumnIndex];
    var toColumn = _columns[toColumnIndex];

    var fromCard = fromColumn.LastOrDefault();
    var toCard = toColumn.LastOrDefault();

    if (fromCard is null || toCard is null) return;

    if (fromCard.ToColor() == toCard?.ToColor()) return;
    if (fromCard.Value + 1 != toCard?.Value) return;

    toColumn.Add(fromCard);
    fromColumn.RemoveAt(fromColumn.Count - 1);
  }

  private bool CheckIfWin()
  {
    var isWin = true;

    foreach (var stack in _endingStacks)
    {
      isWin = stack.Count == 13;
    }

    return isWin;
  }

  public void MoveCardFromSpareToEndingStack()
  {
    var spareCards = _cards.GetRange(0,  _cardsStackIndex);
    var fromCard = spareCards.LastOrDefault();

    if (fromCard is null) return;

    List<Card>? toColumn;

    switch (fromCard.Type)
    {
      case CardType.Karo:
        toColumn = _endingStacks[0];
        break;
      case CardType.Kier:
        toColumn = _endingStacks[1];
        break;
      case CardType.Pik:
        toColumn = _endingStacks[2];
        break;
      case CardType.Trefl:
        toColumn = _endingStacks[3];
        break;
      default:
        return;
    }

    var toCard = toColumn.LastOrDefault();
    if (fromCard.Value != CardValue.Ace)
    {
      if (toCard is null) return;
      if (fromCard.Value - 1 != toCard?.Value) return;
    }

    toColumn.Add(fromCard);
    spareCards.RemoveAt(spareCards.Count - 1);
    _cards.Remove(fromCard);
    _cardsStackIndex--;
  }
}