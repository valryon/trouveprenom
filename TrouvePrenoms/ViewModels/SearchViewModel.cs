// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Globalization;
using TrouvePrenoms.Models;

namespace TrouvePrenoms.ViewModels
{
  public class SearchViewModel
  {
    public Prenom[] Results { get; set; }
    public int MinYear { get; set; }
    public int MaxYear { get; set; }
    public int Sex { get; set; }
    public int MinOccurences { get; set; }
    public int MaxOccurences { get; set; }

  }
}
