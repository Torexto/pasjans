using System.ComponentModel;

namespace Pasjans;

public static class Utils
{
  public static string GetDescription(Enum value)
  {
    var field = value.GetType().GetField(value.ToString());
    var attributes = (DescriptionAttribute[])field!.GetCustomAttributes(typeof(DescriptionAttribute), false);
    return attributes.Length > 0 ? attributes[0].Description : value.ToString();
  }
}