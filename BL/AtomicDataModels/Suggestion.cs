using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public class Suggestion
    {
        public SuggestionAction suggestionAction { get; set; }
        public SuggestionEntity suggestionEntity { get; set; }
        public List<int> EntityIds { get; set; }
    }
}
