using Android.Content;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using SearchableSpinner.Droid.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = Java.Lang.Object;

namespace SearchableSpinner.Droid.Controls
{
    public class SpinnerSearch : BaseSpinnerSearch
    {
        private ISearchableSpinnerListener Listener;

        public SpinnerSearch(Context context) : base(context)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public SpinnerSearch(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public SpinnerItem GetSelectedItem() => Items.FirstOrDefault(i => i.IsSelected);

        public override void OnCancel(IDialogInterface dialog)
        {
            string spinnerText = Items.FirstOrDefault(i => i.IsSelected)?.Name;
            if (spinnerText == null)
            {
                spinnerText = DefaultText;
            }

            ArrayAdapter<string> adapterSpinner = new ArrayAdapter<string>(Context, Resource.Layout.item_select_single, Resource.Id.txvItem, new string[] { spinnerText });
            SetAdapter(adapterSpinner);

            if (ItemAdapter != null)
                ItemAdapter.NotifyDataSetChanged();

            Listener?.OnItemSelected(this, Items.FirstOrDefault(i => i.IsSelected));
        }

        public void SetItems(List<SpinnerItem> items, int position, ISearchableSpinnerListener listener)
        {
            Items = items;
            Listener = listener;
            IsMultiSelect = false;

            foreach (SpinnerItem item in Items)
            {
                if (item.IsSelected)
                {
                    DefaultText = item.Name;
                    break;
                }
            }

            ArrayAdapter<string> adapterSpinner = new ArrayAdapter<string>(Context, Resource.Layout.item_select_single, Resource.Id.txvItem, new string[] { DefaultText });
            SetAdapter(adapterSpinner);

            if (position != -1)
            {
                items[position].IsSelected = true;
                OnCancel(null);
            }
        }

        public void SetSelected(int position)
        {
            if (position != -1)
            {
                Items[position].IsSelected = true;
                OnCancel(null);
            }
        }
    }

}