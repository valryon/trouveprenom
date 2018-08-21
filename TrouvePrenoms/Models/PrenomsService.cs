// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TrouvePrenoms.Models
{
  [System.Serializable]
  public class PrenomsData
  {
    public Dictionary<string, Prenom> Boys = new Dictionary<string, Prenom>(50000);
    public Dictionary<string, Prenom> Girls = new Dictionary<string, Prenom>(50000);

    public int MinYearGlobal;
    public int MaxYearGlobal;
  }

  public class PrenomsService
  {
    // Singleton
    private static PrenomsService instance;

    #region Members

    private PrenomsData data;

    #endregion

    #region Constructor

    public static void Initialize(string file, string cacheFile)
    {
      instance = new PrenomsService();
      bool loadedWithCache = false;
      try
      {
        if (File.Exists(cacheFile))
        {
          loadedWithCache = instance.LoadCache(cacheFile);
        }
      }
      catch (Exception) { }

      if (loadedWithCache == false || instance.data == null)
      {
        instance.LoadData(file);
        instance.SaveCache(cacheFile);
      }
    }

    private void LoadData(string prenomsFile)
    {
      // Load file
      if (File.Exists(prenomsFile) == false)
      {
        throw new ArgumentException("Configuration file not found! " + prenomsFile);
      }

      data = new PrenomsData();
      data.MinYearGlobal = 2999;
      data.MaxYearGlobal = -1;

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
              var collection = p.Sex == Prenom.BOY ? data.Boys : data.Girls;

              if (collection.ContainsKey(p.Value) == false)
              {
                for (int i = 1900; i < DateTime.Now.Year; i++)
                {
                  if (p.Counts.ContainsKey(i) == false)
                  {
                    p.Counts.Add(i, 0);
                  }
                }

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

              var first = p.Counts.Where(a => a.Value > 0).FirstOrDefault();
              var last = p.Counts.Where(a => a.Value > 0).LastOrDefault();

              p.MinYear = first.Key;
              p.MaxYear = last.Key;

              if (p.MinYear > 0) data.MinYearGlobal = Math.Min(MinYearGlobal, p.MinYear);
              if (p.MaxYear > 0) data.MaxYearGlobal = Math.Max(MaxYearGlobal, p.MaxYear);
            }
          }

          n++;
        }
      }

      // Sort by key
      foreach (var g in data.Boys.Values)
      {
        g.Counts = g.Counts.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
      }
      foreach (var f in data.Girls.Values)
      {
        f.Counts = f.Counts.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
      }
    }

    /// <summary>
    /// Convert a line of the line into an objet
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static Prenom Parse(string line)
    {
      Prenom p = new Prenom();

      var data = line.Split('\t');

      p.Sex = Int32.Parse(data[0]);
      p.Value = data[1];

      if (p.Value == "_PRENOMS_RARES") return null;
      if (p.Value == "Anonyme") return null;

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

      var collection = sex == Prenom.BOY ? instance.data.Boys : instance.data.Girls;

      if (predicate == null)
      {
        return collection.Values.ToArray();
      }
      else
      {
        return collection.Values.Where(p => predicate(p)).ToArray();
      }
    }

    public static Prenom[] Get(Predicate<Prenom> predicate = null)
    {
      if (instance == null) return null;

      var collection = instance.data.Boys.Values.Union(instance.data.Girls.Values);

      if (predicate == null)
      {
        return collection.ToArray();
      }
      else
      {
        return collection.Where(p => predicate(p)).ToArray();
      }
    }

    public static string[] GetAllPrenoms()
    {
      if (instance == null) return null;

      return instance.data.Boys.Keys.Union(instance.data.Girls.Keys).ToArray();
    }

    #endregion

    #region Serialization & caching

    private bool LoadCache(string file)
    {
      // Disable caching here
      return false;

      if (File.Exists(file) == false) return false;
      try
      {
        var binData = File.ReadAllBytes(file);
        data = (PrenomsData)Serializer.DeSerialize(binData);

        return true;
      }
      catch (Exception) { }
      return false;
    }

    private void SaveCache(string file)
    {
      try
      {
        var binData = Serializer.Serialize(data);
        File.WriteAllBytes(file, binData);
      }
      catch (Exception) { }
    }
    #endregion

    public static int MinYearGlobal { get { return instance.data.MinYearGlobal; } }
    public static int MaxYearGlobal { get { return instance.data.MaxYearGlobal; } }
  }
}
