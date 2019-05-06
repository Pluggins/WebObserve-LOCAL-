using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Observer.Models;

namespace Observer.API.ViewModels
{
    public class CatchListOutputModel
    {
        public List<CatchesModel> Catches { get; set; }
        public string SelectedId { get; set; }
    }
}