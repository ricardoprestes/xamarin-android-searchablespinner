using Android.Content;
using Android.Util;
using Android.Widget;
using SearchableSpinner.Droid.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SearchableSpinner.Droid.Controls
{
    public class MultiSpinnerSearch : BaseSpinnerSearch
    {
        private IMultiSearchableSpinnerListener Listener;

        public MultiSpinnerSearch(Context context) : base(context)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public MultiSpinnerSearch(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public List<SpinnerItem> GetSelectedItems() => Items.Where(i => i.IsSelected).ToList();

        public override void OnCancel(IDialogInterface dialog)
        {
            var selecteds = GetSelectedItems();
            string spinnerText = "";
            for (int i = 0; i < selecteds.Count; i++)
            {
                if (i > 0)
                    spinnerText += ", " ;
                spinnerText += selecteds[i].Name;
            }

            if (string.IsNullOrWhiteSpace(spinnerText))
                spinnerText = DefaultText;

            ArrayAdapter<string> adapterSpinner = new ArrayAdapter<string>(Context, Resource.Layout.item_select_single, Resource.Id.txvItem, new string[] { spinnerText });
            SetAdapter(adapterSpinner);

            if (ItemAdapter != null)
                ItemAdapter.NotifyDataSetChanged();

            Listener?.OnItemsSelected(this, GetSelectedItems());
        }

        public void SetItems(List<SpinnerItem> items, IMultiSearchableSpinnerListener listener)
        {
            Items = items;
            Listener = listener;
            IsMultiSelect = true;

            var selecteds = GetSelectedItems();
            string spinnerText = "";
            for (int i = 0; i < selecteds.Count; i++)
            {
                if (i > 0)
                    spinnerText += ", ";
                spinnerText += selecteds[i].Name;
            }

            if (string.IsNullOrWhiteSpace(spinnerText))
                spinnerText = DefaultText;

            ArrayAdapter<string> adapterSpinner = new ArrayAdapter<string>(Context, Resource.Layout.item_select_single, Resource.Id.txvItem, new string[] { DefaultText });
            SetAdapter(adapterSpinner);
        }

        public void SetSelecteds(List<int> positions)
        {
            foreach (var position in positions)
            {
                Items[position].IsSelected = true;
            }
            OnCancel(null);
        }
    }
}