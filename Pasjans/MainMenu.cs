using System.ComponentModel;
using static System.Console;

namespace Pasjans;

/// <summary>
/// Reprezentuje główne menu gry Pasjans.
/// </summary>
public abstract class MainMenu : Menu
{
  /// <summary>
  /// Uruchamia pętlę głównego menu, umożliwiając użytkownikowi wybór opcji.
  /// </summary>
  public static void Create()
  {
    CursorVisible = false;
    var selectedOption = MainMenuOptions.PlayEasy;

    while (true)
    {
      Clear();
      WriteLine("Pasjans\n");

      PrintMenu(selectedOption);

      var key = ReadKey().Key;

      switch (key)
      {
        case ConsoleKey.DownArrow:
          NextOption(ref selectedOption);
          break;
        case ConsoleKey.UpArrow:
          PreviousOption(ref selectedOption);
          break;
        case ConsoleKey.Q:
          Clear();
          Environment.Exit(0);
          break;
        case ConsoleKey.Enter:
          switch (selectedOption)
          {
            case MainMenuOptions.PlayEasy:
              GameMenu.Create(Pasjans.Mode.Easy);
              break;
            case MainMenuOptions.PlayHard:
              GameMenu.Create(Pasjans.Mode.Hard);
              break;
            case MainMenuOptions.Ranking:
              ScoreboardMenu.Create();
              break;
            case MainMenuOptions.Quit:
              Clear();
              Environment.Exit(0);
              break;
          }

          break;
      }
    }
  }

  /// <summary>
  /// Dostępne opcje głównego menu.
  /// </summary>
  private enum MainMenuOptions
  {
    /// <summary>
    /// Opcja rozpoczęcia gry w trybie łatwym.
    /// </summary>
    [Description("Zagraj na poziomie łatwym")]
    PlayEasy,

    /// <summary>
    /// Opcja rozpoczęcia gry w trybie trudnym.
    /// </summary>
    [Description("Zagraj na poziomie trudnym")]
    PlayHard,

    /// <summary>
    /// Opcja wyświetlenia rankingu.
    /// </summary>
    [Description("Ranking")] Ranking,

    /// <summary>
    /// Opcja zakończenia gry.
    /// </summary>
    [Description("Wyjście")] Quit
  }
}