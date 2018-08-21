// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using ProtoBuf;

namespace TrouvePrenoms.Models
{
  [ProtoContract]
  public class Prenom
  {
    public const int BOY = 1;
    public const int GIRL = 2;

    [ProtoMember(1)]
    public string Value { get; set; }
    [ProtoMember(2)]
    public int TotalCount { get; set; }
    [ProtoMember(3)]
    public Dictionary<int, int> Counts { get; set; } = new Dictionary<int, int>();
    [ProtoMember(4)]
    public int Sex { get; set; }
    [ProtoMember(5)]
    public int MinYear { get; set; }
    [ProtoMember(6)]
    public int MaxYear { get; set; }

    public int GetCount(int yearMin, int yearMax)
    {
      int count = 0;

      for (int i = yearMin; i <= yearMax; i++)
      {
        count += Counts[i];
      }

      return count;
    }

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
