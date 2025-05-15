using System.ComponentModel;

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
};

public class Pasjans
{
  private readonly List<Card> _cards = new List<Card>(52);

  private readonly List<List<Card>> _result =
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

  public Pasjans()
  {
    GenerateCards();
    DealCards();
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

  public void PrintBoard()
  {
    for (var i = 0; i < 7; i++)
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

        Console.Write(column[i].ToString());
      }

      Console.WriteLine();
    }
  }
}