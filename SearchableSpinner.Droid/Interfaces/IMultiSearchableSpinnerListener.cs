using Android.Views;
using SearchableSpinner.Droid.Controls;
using System.Collections.Generic;

namespace SearchableSpinner.Droid.Interfaces
{
    public interface IMultiSearchableSpinnerListener
    {
        void OnItemsSelected(View view, List<SpinnerItem> items);
    }
}