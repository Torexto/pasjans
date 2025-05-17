using System.ComponentModel;
using static System.Console;

namespace Pasjans;

public abstract class GameMenu : Menu
{
  public static void Create(Mode mode)
  {
    var game = new Pasjans(mode);
    var selectedOption = GameMenuOption.DrawCards;

    while (true)
    {
      Clear();

      if (game.CheckIfWin())
      {
        WriteLine("Gratulacje!!!");
        WriteLine($"Ilość ruchów: {game.Moves}\n");

        WriteLine("Naciśnij dowolny klawisz, aby wrócić");
        ReadKey();

        ScoreboardMenu.Scores.Add(game.Moves);
        ScoreboardMenu.Create();
        return;
      }

      game.PrintBoard();

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
            case GameMenuOption.DrawCards:
              game.DrawCards();
              break;
            case GameMenuOption.MoveFromPileToColumn:
              MoveFromPileToColumn(game);
              break;
            case GameMenuOption.MoveBetweenColumns:
              MoveBetweenColumns(game);
              break;
            case GameMenuOption.MoveFromColumnToEndingStack:
              MoveFromColumnToEndingStack(game);
              break;
            case GameMenuOption.MoveFromEndingStackToColumn:
              MoveFromEndingStackToColumn(game);
              break;
            case GameMenuOption.MoveFromPileToEndingStack:
              game.MoveFromPileToEndingStack();
              break;
            case GameMenuOption.Undo:
              game.Undo();
              break;
            case GameMenuOption.Exit:
              return;
          }

          break;
      }
    }
  }

  private static bool GetNumber(string text, out int number)
  {
    WriteLine(text);
    return int.TryParse(ReadLine(), out number);
  }

  private static void MoveFromPileToColumn(Pasjans game)
  {
    if (!GetNumber("Wybierz kolumnę do której chcesz przenieść kartę", out var toColumnIndex)) return;

    game.MoveFromPileToColumn(toColumnIndex - 1);
  }

  private static void MoveBetweenColumns(Pasjans game)
  {
    if (!GetNumber("Wybierz kolumnę z której chcesz przenieść kartę", out var fromColumnIndex)) return;
    if (!GetNumber("Wybierz kartę z kolumny którą chcesz przenieść", out var fromRowIndex)) return;
    if (!GetNumber("Wybierz kolumnę do której chcesz przenieść kartę", out var toColumnIndex)) return;

    game.MoveBetweenColumns(fromColumnIndex - 1, fromRowIndex - 1, toColumnIndex - 1);
  }

  private static void MoveFromColumnToEndingStack(Pasjans game)
  {
    if (!GetNumber("Wybierz kolumnę z której chcesz przenieść kartę", out var fromColumnIndex)) return;

    game.MoveFromColumnToEndingStack(fromColumnIndex - 1);
  }

  private static void MoveFromEndingStackToColumn(Pasjans game)
  {
    if (!GetNumber("Wybierz kolumnę z której chcesz przenieść kartę", out var fromColumnIndex)) return;
    if (!GetNumber("Wybierz kolumnę do której chcesz przenieść kartę", out var toColumnIndex)) return;

    game.MoveFromEndingStackToColumn(fromColumnIndex - 1, toColumnIndex - 1);
  }

  private enum GameMenuOption
  {
    [Description("Dobierz karty")] DrawCards,

    [Description("Przenieś kartę ze stosu rezerwowego do kolumny")]
    MoveFromPileToColumn,

    [Description("Przenieś karty z kolumny do kolumny")]
    MoveBetweenColumns,

    [Description("Przenieś kartę z kolumny do stosu końcowego")]
    MoveFromColumnToEndingStack,

    [Description("Przenieś kartę ze stosu końcowego do kolumny")]
    MoveFromEndingStackToColumn,

    [Description("Przenieś kartę ze stosu rezerwowego do stosu końcowego")]
    MoveFromPileToEndingStack,

    [Description("Cofnij")] Undo,

    [Description("Zakończ grę")] Exit
  }
}