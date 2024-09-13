using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

class Utils {
  public static string ToString(object? what) {
    if (what == null) {
      return "null";
    }

    if (what is List<dynamic> l) {
      return "[" + string.Join(", ", l.Select(i => Utils.ToString(i)).ToList()) + "]";
    }
    else if (what is IEnumerable<dynamic> e) {
      return "[" + string.Join(", ", e.Select(i => Utils.ToString(i)).ToList()) + "]";
    }
    else if (what is bool[] ba) {
      return "[" + string.Join(", ", ba.Select(i => Utils.ToString(i)).ToList()) + "]";
    }
    else if (what.GetType().IsArray) {
      return "[" + string.Join(", ", ((dynamic[])what).Select(i => Utils.ToString(i)).ToList()) + "]";
    }
    else if (what is Dictionary<string, dynamic> dictionary) {
      return "{" + string.Join(", ", dictionary.Select(i => i.Key + ": " + Utils.ToString(i.Value)).ToList()) + "}";
    }
    else if (what is string s) {
      return "\"" + s + "\"";
    }

    return what.ToString() ?? "null";
  }

  public static void print(
    object? what,
    [CallerArgumentExpression(nameof(what))] string? whatExpression = null
  ) {
    GD.PrintS(whatExpression + "=" + ToString(what));
  }

  public static void print(
    object? what,
    object? what2,
    [CallerArgumentExpression(nameof(what))] string? whatExpression = null,
    [CallerArgumentExpression(nameof(what2))] string? what2Expression = null
  ) {
    GD.PrintS(
      whatExpression + "=" + ToString(what) + ", " +
      what2Expression + "=" + ToString(what2)
    );
  }

  public static void print(
    object? what,
    object? what2,
    object? what3,
    [CallerArgumentExpression(nameof(what))] string? whatExpression = null,
    [CallerArgumentExpression(nameof(what2))] string? what2Expression = null,
    [CallerArgumentExpression(nameof(what3))] string? what3Expression = null
  ) {
    GD.PrintS(
      whatExpression + "=" + ToString(what) + ", " +
      what2Expression + "=" + ToString(what2) + ", " +
      what3Expression + "=" + ToString(what3)
    );
  }
}