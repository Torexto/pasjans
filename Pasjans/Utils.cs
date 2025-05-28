using System.ComponentModel;

namespace Pasjans;

/// <summary>
/// Zawiera metody pomocnicze ogólnego zastosowania.
/// </summary>
public static class Utils
{
  /// <summary>
  /// Pobiera opis atrybutu <see cref="DescriptionAttribute"/> przypisanego do wartości wyliczeniowej <paramref name="value"/>.
  /// Jeśli atrybut nie jest obecny, zwraca nazwę wartości enuma jako tekst.
  /// </summary>
  /// <param name="value">Wartość wyliczeniowa, dla której pobierany jest opis.</param>
  /// <returns>Opis lub nazwa wartości enuma.</returns>
  public static string GetDescription(Enum value)
  {
    var field = value.GetType().GetField(value.ToString());
    var attributes = (DescriptionAttribute[])field!.GetCustomAttributes(typeof(DescriptionAttribute), false);
    return attributes.Length > 0 ? attributes[0].Description : value.ToString();
  }

  /// <summary>
  /// Tworzy listę pustych list o typie <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">Typ elementów przechowywanych w listach.</typeparam>
  /// <param name="count">Liczba pustych list do utworzenia.</param>
  /// <returns>Lista zawierająca <paramref name="count"/> pustych list typu <typeparamref name="T"/>.</returns>
  public static IReadOnlyList<List<T>> CreateEmptyLists<T>(int count)
  {
    var stacks = new List<List<T>>(count);
    for (var i = 0; i < count; i++)
      stacks.Add(new List<T>());
    return stacks;
  }
}