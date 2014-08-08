using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilEditor.Models
{
    public class Briefing : ObservableCollection<string>
    {
        /// <summary>
        /// The briefing sections that are displayed in the mission editor
        /// </summary>
        public Dictionary<string, string> BriefingParts { get; set; }
    }
}
