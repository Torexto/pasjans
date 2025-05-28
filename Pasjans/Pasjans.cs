using static Pasjans.Utils;

namespace Pasjans;

/// <summary>
/// Reprezentuje logikę gry Pasjans.
/// Obejmuje operacje inicjalizacji, ruchów kart, cofania i warunków zwycięstwa.
/// </summary>
public class Pasjans
{
    /// <summary>
    /// Określa poziom trudności gry.
    /// </summary>
    public enum Mode
    {
        Easy,
        Hard
    }

    private const int UndoStackSize = 3;

    /// <summary>
    /// Liczba wykonanych ruchów.
    /// </summary>
    public uint Moves { get; private set; }

    /// <summary>
    /// Aktualny indeks stosu do dobierania kart.
    /// </summary>
    public int DrawPileIndex { get; private set; }

    /// <summary>
    /// Lista kart w talii, w tym karty do dobierania.
    /// </summary>
    public List<Card> Deck { get; }

    /// <summary>
    /// Kolumny gry (7 kolumn).
    /// </summary>
    public IReadOnlyList<List<Card>> Columns { get; }

    /// <summary>
    /// Końcowe stosy kart (4 stosy, po jednym na każdy kolor).
    /// </summary>
    public IReadOnlyList<List<Card>> EndingStacks { get; }

    private readonly UndoStack _undoStack = new(UndoStackSize);
    private readonly Mode _mode;

    /// <summary>
    /// Tworzy nową instancję gry Pasjans.
    /// </summary>
    /// <param name="mode">Wybrany tryb gry: łatwy lub trudny.</param>
    public Pasjans(Mode mode)
    {
        _mode = mode;
        Deck = new List<Card>(52);
        Columns = CreateEmptyLists<Card>(7);
        EndingStacks = CreateEmptyLists<Card>(4);
        InitializeDeck();
        InitializeColumns();
    }

    /// <summary>
    /// Inicjalizuje i tasuje talię kart.
    /// </summary>
    private void InitializeDeck()
    {
        Deck.Clear();
        foreach (var type in Enum.GetValues<CardType>())
        foreach (var value in Enum.GetValues<CardValue>())
            Deck.Add(new Card(type, value));
        ShuffleDeck();
    }

    /// <summary>
    /// Tasuje talie kart losowo.
    /// </summary>
    private void ShuffleDeck()
    {
        var index = Deck.Count;
        while (index > 1)
        {
            var randomIndex = Random.Shared.Next(index--);
            (Deck[index], Deck[randomIndex]) = (Deck[randomIndex], Deck[index]);
        }
    }

    /// <summary>
    /// Przenosi pojedynczą kartę między dwoma listami.
    /// </summary>
    private static void MoveCard(Card card, List<Card> from, List<Card> to)
    {
        if (!from.Contains(card)) return;
        to.Add(card);
        from.Remove(card);
    }

    /// <summary>
    /// Przenosi wiele kart między listami.
    /// </summary>
    private static void MoveCards(List<Card> from, List<Card> to, int count)
    {
        var range = from.GetRange(from.Count - count, count);
        to.AddRange(range);
        from.RemoveRange(from.Count - count, count);
    }

    /// <summary>
    /// Rozkłada karty do 7 kolumn zgodnie z zasadami Pasjansa.
    /// </summary>
    private void InitializeColumns()
    {
        foreach (var (column, index) in Columns.Select((c, i) => (c, i)))
        {
            for (var i = 0; i <= index; i++)
            {
                var card = Deck.Last();
                card.IsFaceUp = false;
                MoveCard(card, Deck, column);
            }
            column.Last().IsFaceUp = true;
        }
    }

    /// <summary>
    /// Wykonuje ruch kart między kolumnami.
    /// </summary>
    public void MoveBetweenColumns(int fromColumnIndex, int fromRowIndex, int toColumnIndex)
    {
        if (fromColumnIndex == toColumnIndex) return;
        if (fromColumnIndex < 0 || fromColumnIndex >= Columns.Count) return;
        if (toColumnIndex < 0 || toColumnIndex >= Columns.Count) return;

        var fromColumn = Columns[fromColumnIndex];
        var toColumn = Columns[toColumnIndex];
        if (fromRowIndex < 0 || fromRowIndex >= fromColumn.Count) return;

        var card = fromColumn[fromRowIndex];
        if (!card.IsFaceUp) return;

        var toCard = toColumn.LastOrDefault();
        if (!(card.Value == CardValue.King && toCard is null))
        {
            if (toCard is null || card.ToColor() == toCard.ToColor() || card.Value + 1 != toCard.Value)
                return;
        }

        var count = fromColumn.Count - fromRowIndex;
        var stack = fromColumn.GetRange(fromRowIndex, count);

        MoveCards(fromColumn, toColumn, count);
        var lastCard = fromColumn.LastOrDefault();
        _undoStack.Push(new Move(MoveType.ColumnToColumn, [..stack], fromColumnIndex, toColumnIndex, 0, lastCard?.IsFaceUp ?? false));

        if (lastCard is not null) lastCard.IsFaceUp = true;
        Moves++;
    }

    /// <summary>
    /// Dobiera karty ze stosu: 1 (Easy) lub 3 (Hard).
    /// </summary>
    public void DrawCards()
    {
        if (Deck.Count == 0) return;
        if (DrawPileIndex >= Deck.Count)
        {
            DrawPileIndex = 0;
            ShuffleDeck();
            return;
        }

        _undoStack.Push(new Move(MoveType.Draw, [], -1, -1, DrawPileIndex));
        DrawPileIndex += _mode == Mode.Easy ? 1 : 3;
        DrawPileIndex = Math.Clamp(DrawPileIndex, 0, Deck.Count);
        Moves++;
    }

    /// <summary>
    /// Przenosi kartę z talii do wskazanej kolumny.
    /// </summary>
    public void MoveFromPileToColumn(int toColumnIndex)
    {
        if (toColumnIndex < 0 || toColumnIndex >= Columns.Count) return;
        if (DrawPileIndex == 0) return;

        var card = Deck[DrawPileIndex - 1];
        var toColumn = Columns[toColumnIndex];
        var toCard = toColumn.LastOrDefault();

        if (!(card.Value == CardValue.King && toCard is null))
        {
            if (toCard is null || card.ToColor() == toCard.ToColor() || card.Value + 1 != toCard.Value)
                return;
        }

        MoveCard(card, Deck, toColumn);
        _undoStack.Push(new Move(MoveType.PileToColumn, [card], -1, toColumnIndex));
        DrawPileIndex--;
        Moves++;
    }

    /// <summary>
    /// Przenosi kartę z kolumny do odpowiedniego stosu końcowego.
    /// </summary>
    public void MoveFromColumnToEndingStack(int fromColumnIndex)
    {
        if (fromColumnIndex > 7) return;

        var fromColumn = Columns[fromColumnIndex];
        var card = fromColumn.LastOrDefault();
        if (card is null) return;

        var toStackIndex = card.Type switch
        {
            CardType.Karo => 0,
            CardType.Kier => 1,
            CardType.Pik => 2,
            CardType.Trefl => 3,
            _ => -1
        };

        if (toStackIndex == -1) return;

        var toColumn = EndingStacks[toStackIndex];
        var toCard = toColumn.LastOrDefault();

        if (card.Value != CardValue.Ace)
        {
            if (toCard is null || card.Value - 1 != toCard.Value) return;
        }

        MoveCard(card, fromColumn, toColumn);
        var lastCard = fromColumn.LastOrDefault();
        _undoStack.Push(new Move(MoveType.ColumnToEnding, [card], fromColumnIndex, toStackIndex, 0, lastCard?.IsFaceUp ?? false));

        if (lastCard is not null) lastCard.IsFaceUp = true;
        Moves++;
    }

    /// <summary>
    /// Przenosi kartę ze stosu końcowego do wskazanej kolumny.
    /// </summary>
    public void MoveFromEndingStackToColumn(int fromStackIndex, int toColumnIndex)
    {
        if (toColumnIndex > 7 || fromStackIndex > 4) return;

        var fromColumn = EndingStacks[fromStackIndex];
        var toColumn = Columns[toColumnIndex];
        var card = fromColumn.LastOrDefault();
        var toCard = toColumn.LastOrDefault();

        if (card is null || toCard is null) return;
        if (card.ToColor() == toCard.ToColor() || card.Value + 1 != toCard.Value) return;

        MoveCard(card, fromColumn, toColumn);
        _undoStack.Push(new Move(MoveType.EndingToColumn, [card], fromStackIndex, toColumnIndex));
        Moves++;
    }

    /// <summary>
    /// Sprawdza, czy gra została wygrana (wszystkie stosy mają po 13 kart).
    /// </summary>
    public bool CheckIfWin()
    {
        return EndingStacks.All(stack => stack.Count == 13);
    }

    /// <summary>
    /// Przenosi kartę ze stosu dobierania do stosu końcowego.
    /// </summary>
    public void MoveFromPileToEndingStack()
    {
        var spareCards = Deck.GetRange(0, DrawPileIndex);
        var card = spareCards.LastOrDefault();
        if (card is null) return;

        int toStackIndex = card.Type switch
        {
            CardType.Karo => 0,
            CardType.Kier => 1,
            CardType.Pik => 2,
            CardType.Trefl => 3,
            _ => -1
        };

        if (toStackIndex == -1) return;

        var toColumn = EndingStacks[toStackIndex];
        var toCard = toColumn.LastOrDefault();

        if (card.Value != CardValue.Ace)
        {
            if (toCard is null || card.Value - 1 != toCard.Value) return;
        }

        MoveCard(card, Deck, toColumn);
        DrawPileIndex--;
        _undoStack.Push(new Move(MoveType.PileToEnding, [card], -1, toStackIndex));
        Moves++;
    }

    /// <summary>
    /// Cofa ostatni wykonany ruch, o ile jest to możliwe.
    /// </summary>
    public void Undo()
    {
        var move = _undoStack.Pop();
        if (move is null) return;

        switch (move.Type)
        {
            case MoveType.ColumnToColumn:
                var toCol = Columns[move.ToIndex];
                var fromCol = Columns[move.FromIndex];
                if (!move.LastCardFaceUp) fromCol.Last().IsFaceUp = false;
                for (var i = 0; i < move.Cards.Count; i++) toCol.RemoveAt(toCol.Count - 1);
                fromCol.AddRange(move.Cards);
                break;

            case MoveType.PileToColumn:
                Columns[move.ToIndex].RemoveAt(Columns[move.ToIndex].Count - 1);
                Deck.Insert(DrawPileIndex, move.Cards[0]);
                DrawPileIndex++;
                break;

            case MoveType.ColumnToEnding:
                if (!move.LastCardFaceUp) Columns[move.FromIndex].Last().IsFaceUp = false;
                EndingStacks[move.ToIndex].RemoveAt(EndingStacks[move.ToIndex].Count - 1);
                Columns[move.FromIndex].Add(move.Cards[0]);
                break;

            case MoveType.EndingToColumn:
                Columns[move.ToIndex].RemoveAt(Columns[move.ToIndex].Count - 1);
                EndingStacks[move.FromIndex].Add(move.Cards[0]);
                break;

            case MoveType.PileToEnding:
                EndingStacks[move.ToIndex].RemoveAt(EndingStacks[move.ToIndex].Count - 1);
                Deck.Insert(DrawPileIndex, move.Cards[0]);
                DrawPileIndex++;
                break;

            case MoveType.Draw:
                DrawPileIndex = move.PreviousStackIndex;
                break;
        }

        Moves--;
    }
}
