using System.ComponentModel;

namespace Pasjans;

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

public record Card(CardType Type, CardValue Value)
{
  public bool IsFaceUp { get; set; } = true;

  public override string ToString()
  {
    var description = $"{Utils.GetDescription(Value)}{Utils.GetDescription(Type)}";

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