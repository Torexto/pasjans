using static System.Console;

namespace Pasjans;

/// <summary>
/// Klasa bazowa dla wszystkich menu w grze Pasjans.
/// Zawiera wspólne metody do obsługi i wyświetlania opcji menu.
/// </summary>
public abstract class Menu
{
  /// <summary>
  /// Wyświetla wszystkie opcje menu, wyróżniając aktualnie wybraną opcję.
  /// </summary>
  /// <typeparam name="T">Typ reprezentujący opcje menu.</typeparam>
  /// <param name="selectedOption">Aktualnie wybrana opcja menu.</param>
  protected static void PrintMenu<T>(T selectedOption) where T : struct, Enum
  {
    var values = Enum.GetValues<T>();

    foreach (var option in values)
    {
      ForegroundColor = option.Equals(selectedOption) ? ConsoleColor.Cyan : ConsoleColor.White;
      WriteLine(Utils.GetDescription(option));
      ForegroundColor = ConsoleColor.White;
    }
  }

  /// <summary>
  /// Przechodzi do następnej opcji menu.
  /// </summary>
  /// <typeparam name="T">Typ reprezentujący opcje menu.</typeparam>
  /// <param name="selectedOption">Referencja do aktualnie wybranej opcji, która zostanie zaktualizowana.</param>
  protected static void NextOption<T>(ref T selectedOption) where T : struct, Enum
  {
    var options = Enum.GetValues<T>();
    var index = Array.IndexOf(options, selectedOption);

    if (index < options.Length - 1)
      selectedOption = options[index + 1];
  }

  /// <summary>
  /// Przechodzi do poprzedniej opcji menu.
  /// </summary>
  /// <typeparam name="T">Typ reprezentujący opcje menu.</typeparam>
  /// <param name="selectedOption">Referencja do aktualnie wybranej opcji, która zostanie zaktualizowana.</param>
  protected static void PreviousOption<T>(ref T selectedOption) where T : struct, Enum
  {
    var options = Enum.GetValues<T>();
    var index = Array.IndexOf(options, selectedOption);

    if (index > 0)
      selectedOption = options[index - 1];
  }
}