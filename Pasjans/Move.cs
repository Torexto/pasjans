namespace Pasjans;

public enum MoveType
{
  ColumnToColumn,
  PileToColumn,
  ColumnToEnding,
  EndingToColumn,
  PileToEnding,
  Draw
}

public record Move(MoveType Type, List<Card> Cards, int FromIndex, int ToIndex, int PreviousStackIndex = 0, bool LastCardFaceUp = true);