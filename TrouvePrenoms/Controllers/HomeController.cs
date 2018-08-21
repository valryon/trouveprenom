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
        var vm = GetData(date);

        ViewData["title"] = "Les prénoms du " + vm.DateString;

        return View("Index",vm);
      }
      catch (Exception)
      {
        return RedirectToAction("Index");
      }
    }

    public IActionResult Search([FromQuery] Criteria criteria, int sex = -1, int count = 100, int page = 1)
    {
      page = Math.Max(1, page);

      var vm = new SearchViewModel()
      {
        Criteria = criteria,
        Sex = sex,
        Page = page,
        CountPerPage = count
      };

      Predicate<Prenom> predicate = CreatePredicate(criteria);

      int totalCount = 0;
      var list = new List<Prenom>();
      if (sex != Prenom.GIRL)
      {
        var g = PrenomsService.Get(Prenom.BOY, predicate);
        totalCount += g.Count();
        list.AddRange(g);
      }
      if (sex != Prenom.BOY)
      {
        var g = PrenomsService.Get(Prenom.GIRL, predicate);
        totalCount += g.Count();
        list.AddRange(g);
      }
      vm.TotalCount = totalCount;
      vm.Results = list.OrderByDescending(p => p.GetCount(criteria.MinYear, criteria.MaxYear)).ThenBy(p => p.Value).Skip((page - 1) * count).Take(count).ToArray();

      vm.TotalPages = totalCount / count;

      string sexString = "";
      if (sex == Prenom.BOY) sexString = "masculins";
      if (sex == Prenom.GIRL) sexString = "féminins";
      ViewData["title"] = "Les prénoms "+ sexString + " de " + vm.Criteria.MinYear +" à "+ vm.Criteria.MaxYear;

      return View(vm);
    }

    private PrenomsViewModel GetData(DateTime date)
    {
      int dateAsSeed = Int32.Parse(date.ToString(PrenomsViewModel.DATE_FORMAT));
      Random r = new Random(dateAsSeed);

      int minYear = PrenomsService.MinYearGlobal;
      int maxYear = PrenomsService.MaxYearGlobal;
      int minOcc = Criteria.MIN_COUNT_THRESHOLD;
      int maxOcc = -1;

      // Make more specific choices
      int mode = r.Next(15);
      switch (mode)
      {
        // Old
        case 1:
          maxYear = 1960;
          break;

        // New
        case 2:
          minYear = 2000;
          break;

        // Very New
        case 3:
          minYear = 2010;
          break;

        // Generation X
        case 4:
          minYear = 1970;
          minYear = 1980;
          break;

        // Generation Y
        case 5:
          minYear = 1980;
          minYear = 1995;
          break;

        // Ultra common
        case 6:
          minOcc = 200000;
          break;

        // Rare
        case 7:
          minOcc = 250;
          maxOcc = 500;
          break;

        // Randoms
        case 9:
        case 8:
          minYear = r.Next(minYear, maxYear - 50);
          maxYear = r.Next(minYear, maxYear);
          minOcc = r.Next(10/(maxYear - minYear), 500/(maxYear - minYear));
          break;

        default:
          // Defaut parameters
          break;
      }

      var criteria = new Criteria()
      {
        MinYear = minYear,
        MaxYear = maxYear,
        MinOccurences = minOcc,
        MaxOccurences = maxOcc
      };

      Predicate<Prenom> predicate = CreatePredicate(criteria);

      var vm = new PrenomsViewModel()
      {
        Date = date,
        Criteria = criteria
      };

      // Pick n girls & n boys
      vm.Boys = PrenomsService.Get(Prenom.BOY, predicate).OrderBy(p => r.NextDouble()).Take(PrenomsViewModel.PRENOMS_COUNT).ToArray();
      vm.Girls = PrenomsService.Get(Prenom.GIRL, predicate).OrderBy(p => r.NextDouble()).Take(PrenomsViewModel.PRENOMS_COUNT).ToArray();

      return vm;
    }

    public IActionResult Name(string name, string search =null)
    {
      if (string.IsNullOrEmpty(search) == false) name = search;

      NameViewModel vm = new NameViewModel();
      vm.Search = name;

      if (string.IsNullOrWhiteSpace(name) == false)
      {
        Predicate<Prenom> pre = (p) =>
        {
          return p.Value.MinLevenshteinDistance(name) <= 2;
        };

        var t = PrenomsService.Get(pre);
        vm.Results = t.OrderBy(p => p.Value.Split("-").Length).ThenBy(p => p.Value.MinLevenshteinDistance(name)).ThenByDescending(p => p.TotalCount).Take(10).ToArray();
      }
      else
      {
        vm.Results = new Prenom[0];
      }

      ViewData["title"] = name + " : statistiques et prénoms similaires";

      return View(vm);
    }

    private static Predicate<Prenom> CreatePredicate(Criteria criteria)
    {
      // Search with LINQ
      return (p) =>
      {
        int c = p.GetCount(criteria.MinYear, criteria.MaxYear);
        return p.MinYear >= criteria.MinYear
        && p.MaxYear <= criteria.MaxYear
        && (c >= criteria.MinOccurences || criteria.MinOccurences <= 0)
        && (c <= criteria.MaxOccurences || criteria.MaxOccurences <= 0);
      };
    }

    public IActionResult Error()
    {
      return RedirectToAction("Index");
    }
  }
}
