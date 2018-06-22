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
    public Criteria Criteria { get; set; }
    public int Sex { get; set; }

    public int TotalCount { get; set; }
    public int CountPerPage { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }

    public object GetPagination(int modifier)
    {
      return new
      {
        minYear = Criteria.MinYear,
        maxYear = Criteria.MinYear,
        minOccurences = Criteria.MinOccurences,
        maxOccurences = Criteria.MaxOccurences,
        sex = Sex,
        page = Page + modifier
      };
    }
  }
}
