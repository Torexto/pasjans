namespace Pasjans;

/// <summary>
/// Reprezentuje typ ruchu wykonanego przez gracza.
/// </summary>
public enum MoveType
{
  ColumnToColumn,
  PileToColumn,
  ColumnToEnding,
  EndingToColumn,
  PileToEnding,
  Draw
}

/// <summary>
/// Reprezentuje wykonany ruch w grze.
/// Używane do systemu cofania (undo).
/// </summary>
/// <param name="Type">Typ ruchu.</param>
/// <param name="Cards">Lista kart objętych ruchem.</param>
/// <param name="FromIndex">Indeks źródłowy (kolumna lub stos).</param>
/// <param name="ToIndex">Indeks docelowy (kolumna lub stos).</param>
/// <param name="PreviousStackIndex">Indeks stosu przed ruchem (dotyczy dobierania kart).</param>
/// <param name="LastCardFaceUp">Stan odkrycia ostatniej karty przed ruchem.</param>
public record Move(
  MoveType Type,
  List<Card> Cards,
  int FromIndex,
  int ToIndex,
  int PreviousStackIndex = 0,
  bool LastCardFaceUp = true
);