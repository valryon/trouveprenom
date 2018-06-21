// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TrouvePrenoms.Models;
using TrouvePrenoms.ViewModels;

namespace TrouvePrenoms.Controllers
{
  public class HomeController : Controller
  {

    public IActionResult Index()
    {
      return View(GetData(DateTime.Now));
    }

    public IActionResult CustomDate(string dateString)
    {
      try
      {
        DateTime date = DateTime.ParseExact(dateString, PrenomsViewModel.DATE_FORMAT, PrenomsViewModel.DATE_CULTURE);
        return View("Index", GetData(date));
      }
      catch (Exception)
      {
        return RedirectToAction("Index");
      }
    }

    public IActionResult Search(int minYear = -1, int maxYear = -1, int sex = -1,  int minOccurences = -1, int maxOccurences = -1, int count = 100)
    {
      if (minYear < PrenomsData.MinYearGlobal) minYear = PrenomsData.MinYearGlobal;
      if (maxYear <= 0 || maxYear > PrenomsData.MaxYearGlobal) maxYear = PrenomsData.MaxYearGlobal;

      var vm = new SearchViewModel()
      {
        MinYear = minYear,
        MaxYear = maxYear,
        Sex = sex,
        MinOccurences = minOccurences,
        MaxOccurences = maxOccurences
      };

      // Search with LINQ
      Predicate<Prenom> predicate = (p) =>
      {
        int c = p.GetCount(minYear, maxYear);
        return p.MinYear >= minYear
        && p.MaxYear <= maxYear
        && (c >= minOccurences || minOccurences <= 0)
        && (c <= maxOccurences || maxOccurences <= 0);
      };

      var list = new List<Prenom>();
      if (sex != Prenom.GIRL)
      {
        list.AddRange(PrenomsData.Get(Prenom.BOY, predicate).Take(count));
      }
      if (sex != Prenom.BOY)
      {
        list.AddRange(PrenomsData.Get(Prenom.GIRL, predicate).Take(count));
      }
      vm.Results = list.ToArray();

      return View(vm);
    }

    private PrenomsViewModel GetData(DateTime date)
    {
      var vm = new PrenomsViewModel()
      {
        Date = date
      };

      int dateAsSeed = Int32.Parse(date.ToString(PrenomsViewModel.DATE_FORMAT));
      Random r = new Random(dateAsSeed);

      Predicate<Prenom> predicate = (p) =>
      {
        int c = p.GetCount(PrenomsData.MinYearGlobal, PrenomsData.MaxYearGlobal);
        return c > 500;
      };


      // Pick n girls & n boys
      vm.Boys = PrenomsData.Get(Prenom.BOY, predicate).OrderBy(p => r.NextDouble()).Take(PrenomsViewModel.PRENOMS_COUNT).ToArray();
      vm.Girls = PrenomsData.Get(Prenom.GIRL, predicate).OrderBy(p => r.NextDouble()).Take(PrenomsViewModel.PRENOMS_COUNT).ToArray();
      
      return vm;
    }

    public IActionResult Error()
    {
      return RedirectToAction("Index");
    }
  }
}
