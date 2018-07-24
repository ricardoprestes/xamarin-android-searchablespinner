using SearchableSpinner.Droid.Controls;
using System.Collections.Generic;

namespace SearchableSpinner.Droid.Interfaces
{
    public interface ISpinnerListener
    {
        void OnItemsSelected(List<SpinnerItem> items);
    }
}