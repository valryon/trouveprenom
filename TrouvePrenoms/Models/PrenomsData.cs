// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TrouvePrenoms.Models
{
  public class PrenomsData
  {

    // Singleton
    private static PrenomsData instance;

    #region Members

    private Dictionary<string, Prenom> boys = new Dictionary<string, Prenom>(50000);
    private Dictionary<string, Prenom> girls = new Dictionary<string, Prenom>(50000);

    #endregion

    #region Constructor

    public static void Initialize(string file)
    {
      instance = new PrenomsData();
      instance.LoadData(file);      
    }

    private void LoadData(string prenomsFile)
    {
      // Load file
      if (File.Exists(prenomsFile) == false)
      {
        throw new ArgumentException("Configuration file not found! " + prenomsFile);
      }

      boys.Clear();
      girls.Clear();

      // Extract data line by line
      int n = 0;
      string line;
      using (System.IO.StreamReader file = new System.IO.StreamReader(prenomsFile))
      {
        while ((line = file.ReadLine()) != null)
        {
          // Ignore first 4 lines
          if (n >= 4)
          {

            // Parse
            var p = Parse(line);

            if (p != null)
            {
              var collection = p.Sex == Prenom.BOY ? boys : girls;

              if (collection.ContainsKey(p.Value) == false)
              {
                collection.Add(p.Value, p);
              }
              else
              {
                var pExist = collection[p.Value];

                // Merge stats
                pExist.TotalCount += p.TotalCount;

                foreach (var y in p.Counts.Keys)
                {
                  if (pExist.Counts.ContainsKey(y) == false)
                  {
                    pExist.Counts[y] = 0;
                  }
                  pExist.Counts[y] += p.Counts[y];
                }

                collection[p.Value] = pExist;
              }
            }
          }

          n++;
        }
      }
    }

    /// <summary>
    /// Convert a line of the line into an objet
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static Prenom Parse( string line)
    {
      Prenom p = new Prenom();

      var data = line.Split('\t');

      p.Sex = Int32.Parse(data[0]);
      p.Value = data[1];

      if (p.Value == "_PRENOMS_RARES") return null;

      p.TotalCount = Int32.Parse(data[3]);
      p.Counts = new Dictionary<int, int>();

      if (data[2] != "XXXX")
      {
        try
        {
          int year = Int32.Parse(data[2]);
          p.Counts.Add(year, p.TotalCount);
        }
        catch (FormatException) { }
      }


      return p;
    }

    #endregion

    #region Data access

    public static Prenom[] Get(int sex, Predicate<Prenom> predicate = null)
    {
      if (instance == null) return null;

      var collection = sex == Prenom.BOY ? instance.boys : instance.girls;

      if (predicate == null)
      {
        return collection.Values.ToArray();
      }
      else
      {
        return collection.Values.Where(p => predicate(p)).ToArray();
      }
    }

    #endregion
  }
}
