using System.ComponentModel;
using static System.Console;

namespace Pasjans;

public abstract class MainMenu : Menu
{
  public static void Create()
  {
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
        case ConsoleKey.Enter:
          switch (selectedOption)
          {
            case MainMenuOptions.PlayEasy:
              GameMenu.Create(Mode.Easy);
              break;
            case MainMenuOptions.PlayHard:
              GameMenu.Create(Mode.Hard);
              break;
            case MainMenuOptions.Ranking:
              ScoreboardMenu.Create();
              break;
            case MainMenuOptions.Quit:
              Environment.Exit(0);
              break;
          }

          break;
      }
    }
  }

  private enum MainMenuOptions
  {
    [Description("Zagraj na poziomie łatwym")]
    PlayEasy,

    [Description("Zagraj na poziomie trudnym")]
    PlayHard,

    [Description("Ranking")] Ranking,

    [Description("Wyjście")] Quit
  }
}