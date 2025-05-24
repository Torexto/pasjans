using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Console;

namespace Pasjans;

public abstract class GameMenu : Menu
{
  private static void PrintBoard(Pasjans game, (int, int)? highlight = null)
  {
    var max = game.Columns.Select(column => column.Count).Max();

    WriteLine();

    for (var i = 0; i < max; i++)
    {
      foreach (var (column, j) in game.Columns.Select((list, j) => (list, i: j)))
      {
        Write(" ");

        if (column.Count - 1 == i)
          column[i].IsFaceUp = true;

        if (column.Count <= i)
        {
          Write("   ");
          continue;
        }

        if (highlight is not null)
        {
          if (highlight.Value.Item2 == i && highlight.Value.Item1 == j)
            BackgroundColor = ConsoleColor.Cyan;
        }

        column[i].Print();

        ResetColor();
      }

      WriteLine($" {i + 1}");
    }
  }

  private static int? RowMenu(Pasjans game, int columnIndex)
  {
    var rowTracker = new IndexTracker(game.Columns[columnIndex].Count);

    while (true)
    {
      Clear();

      PrintBoard(game, (columnIndex, rowTracker.Index - 1));

      switch (ReadKey().Key)
      {
        case ConsoleKey.DownArrow:
          rowTracker.Next();
          break;
        case ConsoleKey.UpArrow:
          rowTracker.Previous();
          break;
        case ConsoleKey.Enter:
          return rowTracker.Index;
        case ConsoleKey.Q:
          return null;
      }
    }
  }

  private static int? ColumnMenu(Pasjans game, [CallerMemberName] string name = "")
  {
    var columnTracker = new IndexTracker(7);

    while (true)
    {
      Clear();

      if (name == nameof(MoveFromPileToColumn))
      {
        var card = game.Deck.GetRange(0, game.DrawPileIndex).LastOrDefault();
        if (card is null) return null;

        Write(" ");

        for (var i = 0; i < columnTracker.Index - 1; i++)
          Write("    ");

        card.Print();

        WriteLine();
      }

      for (var j = 1; j <= 7; j++)
      {
        Write(" ");

        if (columnTracker.Index == j)
        {
          BackgroundColor = ConsoleColor.Cyan;
          ForegroundColor = ConsoleColor.Black;
        }

        Write($" {j} ");

        ResetColor();
      }

      PrintBoard(game);

      switch (ReadKey().Key)
      {
        case ConsoleKey.RightArrow:
          columnTracker.Next();
          break;
        case ConsoleKey.LeftArrow:
          columnTracker.Previous();
          break;
        case ConsoleKey.Enter:
          return columnTracker.Index;
        case ConsoleKey.Q:
          return null;
      }
    }
  }

  private static int? EndingStackMenu(Pasjans game)
  {
    var endingStackTracker = new IndexTracker(4);

    while (true)
    {
      Clear();

      foreach (var (stack, i) in game.EndingStacks.Select((list, i) => (list, i)))
      {
        var card = stack.LastOrDefault();

        if (endingStackTracker.Index == i + 1)
          BackgroundColor = ConsoleColor.Cyan;

        if (card is null)
          Write("###");
        else
          card.Print();

        ResetColor();

        Write(" ");
      }

      switch (ReadKey().Key)
      {
        case ConsoleKey.RightArrow:
          endingStackTracker.Next();
          break;
        case ConsoleKey.LeftArrow:
          endingStackTracker.Previous();
          break;
        case ConsoleKey.Enter:
          if (game.EndingStacks[endingStackTracker.Index - 1].Count < 1) return null;
          return endingStackTracker.Index;
        case ConsoleKey.Q:
          return null;
      }
    }
  }

  public static void Create(Pasjans.Mode mode)
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

      for (var i = 1; i <= 7; i++) Write($"  {i} ");
      PrintBoard(game);

      WriteLine();

      Write($"Pozostało: {game.Deck.Count - game.DrawPileIndex} ");

      foreach (var card in game.Deck.GetRange(0, game.DrawPileIndex))
        card.Print();

      WriteLine();

      foreach (var stack in game.EndingStacks)
      {
        var card = stack.LastOrDefault();

        if (card is null)
          Write("###");
        else
          card.Print();

        Write(" ");
      }

      WriteLine();

      WriteLine($"Ilość ruchów: {game.Moves}");

      WriteLine();

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
          return;
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

  private static void MoveFromPileToColumn(Pasjans game)
  {
    var columnIndex = ColumnMenu(game);
    if (columnIndex is null) return;
    game.MoveFromPileToColumn(columnIndex.Value - 1);
  }

  private static void MoveBetweenColumns(Pasjans game)
  {
    var fromColumnIndex = ColumnMenu(game);
    if (fromColumnIndex is null) return;

    var fromRowIndex = RowMenu(game, fromColumnIndex.Value - 1);
    if (fromRowIndex is null) return;

    var toColumnIndex = ColumnMenu(game);
    if (toColumnIndex is null) return;

    game.MoveBetweenColumns(fromColumnIndex.Value - 1, fromRowIndex.Value - 1, toColumnIndex.Value - 1);
  }

  private static void MoveFromColumnToEndingStack(Pasjans game)
  {
    var columnIndex = ColumnMenu(game);
    if (columnIndex is null) return;

    game.MoveFromColumnToEndingStack(columnIndex.Value - 1);
  }

  private static void MoveFromEndingStackToColumn(Pasjans game)
  {
    var endingStackIndex = EndingStackMenu(game);
    if (endingStackIndex is null) return;

    var columnIndex = ColumnMenu(game);
    if (columnIndex is null) return;

    game.MoveFromEndingStackToColumn(endingStackIndex.Value - 1, columnIndex.Value - 1);
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