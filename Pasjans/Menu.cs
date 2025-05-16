using System.ComponentModel;

namespace Pasjans;

using static Console;

public enum MainMenuOptions
{
  [Description("Zagraj na poziomie łatwym")]
  PlayEasy,

  [Description("Zagraj na poziomie trudnym")]
  PlayHard,
  [Description("Wyjście")] Quit
}

public class Menu
{
  public static void MainMenu()
  {
    var options = MainMenuOptions.PlayEasy;

    while (true)
    {
      Clear();
      WriteLine("Pasjans\n");

      foreach (var option in Enum.GetValues<MainMenuOptions>())
      {
        ForegroundColor = option == options ? ConsoleColor.Cyan : ConsoleColor.White;
        WriteLine(Utils.GetDescription(option));
        ForegroundColor = ConsoleColor.White;
      }

      var key = ReadKey().Key;

      switch (key)
      {
        case ConsoleKey.DownArrow:
          if ((int)options < Enum.GetValues<MainMenuOptions>().Length - 1)
            options++;
          break;
        case ConsoleKey.UpArrow:
          if ((int)options > 0)
            options--;
          break;
        case ConsoleKey.Enter:
          if (options == MainMenuOptions.Quit)
            Environment.Exit(0);

          GameMenu(options == MainMenuOptions.PlayEasy ? Mode.Easy : Mode.Hard);
          break;
      }
    }
  }

  private static void GameMenu(Mode mode)
  {
    var game = new Pasjans(mode);
    var isRunning = true;

    while (isRunning)
    {
      Clear();


      game.PrintBoard();

      Console.WriteLine(
        "1 - Dobierz karty, 2 - Przenieś kartę ze stosu do kolumny, 3 - Przenieś karty z kolumny do kolumny, 4 - Przenieś kartę do stosu końcowego, 5 - Przenieś kartę ze stosu końcowego do kolumny, 6 - Prenieś kartę ze stosu zapasowego do stosu końcowego, 7 - Wyjście");
      Console.Write("> ");
      var choice = int.Parse(Console.ReadLine() ?? "");

      switch (choice)
      {
        case 1:
          game.DrawCards();
          break;
        case 2:
          Console.WriteLine("Wybierz kolumnę do której chcesz przenieść kartę");
          var column = int.Parse(Console.ReadLine() ?? "") - 1;

          game.MoveCardFromSpareToColumn(column);
          break;
        case 3:
        {
          Console.WriteLine("Wybierz kolumnę z której chcesz przenieść kartę");
          var fromColumn = int.Parse(Console.ReadLine() ?? "") - 1;

          Console.WriteLine("Wybierz kartę z kolumny którą chcesz przenieść");
          var fromRow = int.Parse(Console.ReadLine() ?? "") - 1;

          Console.WriteLine("Wybierz kolumnę do której chcesz przenieść kartę");
          var toColumn = int.Parse(Console.ReadLine() ?? "") - 1;

          game.MoveCardFromColumnToColumn(fromColumn, fromRow, toColumn);
        }
          break;
        case 4:
        {
          Console.WriteLine("Wybierz kolumnę z której chcesz przenieść kartę");
          var fromColumnIndex = int.Parse(Console.ReadLine() ?? "") - 1;
          game.MoveCardFromColumnToEndingStack(fromColumnIndex);
        }
          break;
        case 5:
        {
          Console.WriteLine("Wybierz kolumnę z której chcesz przenieść kartę");
          var fromColumn = int.Parse(Console.ReadLine() ?? "") - 1;

          Console.WriteLine("Wybierz kolumnę do której chcesz przenieść kartę");
          var toColumn = int.Parse(Console.ReadLine() ?? "") - 1;

          game.MoveCardFromEndingStackToColumn(fromColumn, toColumn);
        }
          break;
        case 6:
          game.MoveCardFromSpareToEndingStack();
          break;
        case 7:
          isRunning = false;
          break;
        default:
          continue;
      }
    }
  }
}