using Pasjans;

var game = new Pasjans.Pasjans(Mode.Easy);
var isRunning = true;

try
{
  while (isRunning)
  {
    Console.Clear();

    game.PrintBoard();

    Console.WriteLine(
      "1 - Dobierz karty, 2 - Przenieś kartę ze stosu do kolumny, 3 - Przenieś karty z kolumny do kolumny, 4 - Przenieś kartę do stosu końcowego, 5 - Przenieś kartę ze stosu końcowego do kolumny, 6 - Wyjście");
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
        Console.WriteLine("Wybierz kolumnę z której chcesz przenieść kartę");
        var fromColumn = int.Parse(Console.ReadLine() ?? "") - 1;

        Console.WriteLine("Wybierz kartę z kolumny którą chcesz przenieść");
        var fromRow = int.Parse(Console.ReadLine() ?? "") - 1;

        Console.WriteLine("Wybierz kolumnę do której chcesz przenieść kartę");
        var toColumn = int.Parse(Console.ReadLine() ?? "") - 1;

        game.MoveCardFromColumnToColumn(fromColumn, fromRow, toColumn);
        break;
      case 4:
        break;
      case 5:
        break;
      case 6:
        isRunning = false;
        break;
    }
  }
}
catch (Exception e)
{
  Console.WriteLine(e.Message);
}