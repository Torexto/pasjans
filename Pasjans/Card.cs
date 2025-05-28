using System.ComponentModel;

namespace Pasjans;

/// <summary>
/// Typ karty reprezentujący kolor w talii (np. Kier, Karo, Pik, Trefl).
/// Każdy element posiada przypisaną ikonę Unicode.
/// </summary>
public enum CardType
{
  [Description("\u2665")] Kier,
  [Description("\u2666")] Karo,
  [Description("\u2660")] Pik,
  [Description("\u2663")] Trefl
}

/// <summary>
/// Wartość karty od Asa do Króla.
/// Każda wartość posiada przypisaną skróconą reprezentację tekstową.
/// </summary>
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

/// <summary>
/// Reprezentuje pojedynczą kartę w grze Pasjans.
/// </summary>
/// <param name="Type">Typ karty (np. Kier, Karo, Pik, Trefl).</param>
/// <param name="Value">Wartość karty (np. As, Dama, Dziesięć).</param>
public record Card(CardType Type, CardValue Value)
{
  /// <summary>
  /// Określa, czy karta jest odkryta (widoczna dla gracza).
  /// Wartość domyślna to <c>true</c>.
  /// </summary>
  public bool IsFaceUp { get; set; } = true;

  /// <summary>
  /// Zwraca reprezentację tekstową karty opartą na atrybutach <see cref="DescriptionAttribute"/>.
  /// Jeśli karta jest zakryta, zwracany jest placeholder <c>"###"</c>.
  /// </summary>
  /// <returns>Tekstowy opis karty (np. "A♥", "10♠", lub "###").</returns>
  public override string ToString()
  {
    var description = $"{Utils.GetDescription(Value)}{Utils.GetDescription(Type)}";

    if (IsFaceUp) return description.Length < 3 ? description + " " : description;

    return "###";
  }

  /// <summary>
  /// Zwraca kolor konsolowy odpowiadający kolorowi karty:
  /// <list type="bullet">
  /// <item><description><see cref="CardType.Kier"/> i <see cref="CardType.Karo"/> = <c>ConsoleColor.Black</c></description></item>
  /// <item><description><see cref="CardType.Pik"/> i <see cref="CardType.Trefl"/> = <c>ConsoleColor.Red</c></description></item>
  /// </list>
  /// </summary>
  /// <returns>Kolor do użycia w konsoli.</returns>
  public ConsoleColor ToColor()
  {
    return Type is CardType.Kier or CardType.Karo ? ConsoleColor.Black : ConsoleColor.Red;
  }

  /// <summary>
  /// Wyświetla kartę w konsoli z odpowiednim kolorem, w zależności od jej typu i stanu (odkryta/zakryta).
  /// </summary>
  public void Print()
  {
    Console.ForegroundColor = IsFaceUp
      ? ToColor()
      : ConsoleColor.White;
    Console.Write(ToString());
    Console.ResetColor();
  }
}