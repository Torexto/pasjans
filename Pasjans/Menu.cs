using static System.Console;

namespace Pasjans;

public abstract class Menu
{
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

  protected static void NextOption<T>(ref T selectedOption) where T : struct, Enum
  {
    var options = Enum.GetValues<T>();
    var index = Array.IndexOf(options, selectedOption);

    if (index < options.Length - 1)
      selectedOption = options[index + 1];
  }

  protected static void PreviousOption<T>(ref T selectedOption) where T : struct, Enum
  {
    var options = Enum.GetValues<T>();
    var index = Array.IndexOf(options, selectedOption);

    if (index > 0)
      selectedOption = options[index - 1];
  }
}