// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Globalization;
using TrouvePrenoms.Models;

namespace TrouvePrenoms.ViewModels
{
  public class MyNameViewModel
  {
    public string Search { get; set; }
    public Prenom[] Results { get; set; }   
    //public string[] AllNames { get; set; }
  }
}
