using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.AtomicDataModels
{
    public enum SuggestionAction
    {
        add,
        remove,
        change,
        nothing
    }

    public enum SuggestionEntity
    {
        Bin,
        Day
    }

}
