// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Globalization;
using TrouvePrenoms.Models;

namespace TrouvePrenoms.ViewModels
{
  public class Criteria
  {
    public const int MIN_COUNT_THRESHOLD = 500;

    public int MinYear { get; set; } = PrenomsData.MinYearGlobal;
    public int MaxYear { get; set; } = PrenomsData.MaxYearGlobal;
    public int MinOccurences { get; set; } = MIN_COUNT_THRESHOLD;
    public int MaxOccurences { get; set; } = -1;
  }
}
