using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrouvePrenoms.ViewModels
{
  public class BaseViewModel
  {
    public virtual int SelectedMenu { get { return -1; } }
  }
}
