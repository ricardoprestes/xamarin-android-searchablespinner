using Android.Views;
using SearchableSpinner.Droid.Controls;

namespace SearchableSpinner.Droid.Interfaces
{
    public interface ISearchableSpinnerListener
    {
        void OnItemSelected(View view, SpinnerItem item);
    }
}