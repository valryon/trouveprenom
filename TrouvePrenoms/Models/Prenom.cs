// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System.Collections.Generic;

namespace TrouvePrenoms.Models
{
  public class Prenom
  {
    public const int BOY = 1;
    public const int GIRL = 2;


    public string Value { get; set; }
    public int TotalCount { get; set; }
    public Dictionary<int, int> Counts { get; set; } = new Dictionary<int, int>();
    public int Sex { get; set; }

    public override string ToString()
    {
      return Value;
    }

    public string DataKeys
    {
      get
      {
        string s = "[";

        foreach (var c in Counts.Keys)
        {
          s += "\"" + c + "\",";
        }
        s = s.Remove(s.Length - 1); // Remove last comma
        s += "]";

        return s;
      }
    }
    public string DataValues
    {
      get
      {
        string s = "[";

        foreach (var c in Counts.Values)
        {
          s += c + ",";
        }
        s = s.Remove(s.Length - 1); // Remove last comma
        s += "]";

        return s;
      }
    }
  }
}
