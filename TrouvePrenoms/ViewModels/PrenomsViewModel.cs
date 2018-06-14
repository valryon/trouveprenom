// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrouvePrenoms.Models;

namespace TrouvePrenoms.ViewModels
{
  public class PrenomsViewModel
  {
    public Prenom[] Boys { get; set; }
    public Prenom[] Girls { get; set; }
    public DateTime Date { get; set; }
    public bool IsTodayOrFuture
    {
      get
      {
        return Date.Day >= DateTime.Now.Day
          && Date.Month >= DateTime.Now.Month
          && Date.Year >= DateTime.Now.Year;
      }
    }
  }
}
