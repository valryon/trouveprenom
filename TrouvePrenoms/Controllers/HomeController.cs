// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        DateTime date = DateTime.Parse(dateString);
        return View("Index", GetData(date));
      }
      catch (Exception)
      {
        return RedirectToAction("Index");
      }
    }

    private PrenomsViewModel GetData(DateTime date)
    {
      var vm = new PrenomsViewModel()
      {
        Date = date
      };

      int dateAsSeed = Int32.Parse(date.ToString("yyyyMMdd"));
      Random r = new Random(dateAsSeed);

      // Pick 5 girls & 5 boys
      vm.Boys = PrenomsData.Get(Prenom.BOY).OrderBy(p => r.NextDouble()).Take(5).ToArray();
      vm.Girls = PrenomsData.Get(Prenom.GIRL).OrderBy(p => r.NextDouble()).Take(5).ToArray();
      
      return vm;
    }

    public IActionResult Error()
    {
      return RedirectToAction("Index");
    }
  }
}
