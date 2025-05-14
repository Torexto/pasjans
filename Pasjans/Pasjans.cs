namespace Pasjans;

public enum Mode
{
  Easy,
  Hard
}

public enum CardType
{
  Kier,
  Karo,
  Pik,
  Trefl
}

public enum CardValue
{
  Ace = 1,
  Two,
  Three,
  Four,
  Five,
  Six,
  Seven,
  Eight,
  Nine,
  Ten,
  Jack,
  Queen,
  King
}

public record Card(CardType Type, CardValue Value);

public class Pasjans
{
  private List<Card> _cards = new List<Card>(52);
  private Stack<Card>[] _columns = new Stack<Card>[7];

  public Pasjans()
  {
    GenerateCards();
  }

  private void GenerateCards()
  {
    ClearCards();

    foreach (var type in Enum.GetValues<CardType>())
    {
      foreach (var value in Enum.GetValues<CardValue>())
      {
        _cards.Add(new Card(type, value));
      }
    }

    ShuffleCards();
  }

  private void ClearCards()
  {
    _cards.Clear();
  }

  private void ShuffleCards()
  {
    var index = _cards.Count;

    while (index > 1)
    {
      var randomIndex = Random.Shared.Next(index--);
      (_cards[index], _cards[randomIndex]) = (_cards[randomIndex], _cards[index]);
    }
  }

  private void DealCards()
  {
    const int len = 1;

    foreach (var column in _columns)
    {
      for (var i = 0; i < len; i++)
      {
        column.Push(_cards.Last());
        _cards.RemoveAt(_cards.Count - 1);
      }
    }
  }
}