namespace Pasjans;

internal class IndexTracker(int max)
{
  private int _index = 1;

  public void Next()
  {
    if (_index < max) _index++;
  }

  public void Previous()
  {
    if (_index > 1) _index--;
  }

  public override string ToString()
  {
    return _index.ToString();
  }

  public int Index => _index;
}