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
    public class SpinnerSearch : AppCompatSpinner, IDialogInterfaceOnCancelListener, IDialogInterfaceOnClickListener
    {
        List<SpinnerItem> Items;
        string DefaultText { get; set; }
        public string SpinnerTitle { get; set; }
        private ISpinnerListener Listener;
        ItemAdapter ItemAdapter { get; set; }
        public static AlertDialog.Builder Builder;
        public static AlertDialog Dialog;

        public Android.Widget.SearchView SearchView { get; set; }
        public TextView EmptyText { get; set; }

        public SpinnerSearch(Context context) : base(context)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public SpinnerSearch(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
        }

        public SpinnerItem GetSelectedItem()
        {
            return Items.FirstOrDefault(i => i.IsSelected);
        }

        public void OnCancel(IDialogInterface dialog)
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

            Listener?.OnItemSelected(Items.FirstOrDefault(i => i.IsSelected));
        }

        public override bool PerformClick()
        {
            DefaultText = Context.GetString(Resource.String.text_nome);
            Builder = new AlertDialog.Builder(Context);
            Builder.SetTitle(SpinnerTitle);

            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);

            View view = inflater.Inflate(Resource.Layout.dialog_recyclerview_search, null);
            Builder.SetView(view);

            EmptyText = (TextView)view.FindViewById(Resource.Id.txvEmpty);
            SearchView = view.FindViewById<Android.Widget.SearchView>(Resource.Id.searchText);
            SearchView.SetQueryHint(Context.GetString(Resource.String.text_search_hint));

            var listView = view.FindViewById<RecyclerView>(Resource.Id.rvItems);
            ItemAdapter = new ItemAdapter(Context, Items, this, EmptyText);
            ItemAdapter.Filter(null);
            listView.SetAdapter(ItemAdapter);
            listView.SetLayoutManager(new LinearLayoutManager(Context));

            SearchView.QueryTextChange += (s, e) =>
            {
                ItemAdapter.Filter(e.NewText);
            };

            Builder.SetPositiveButton(Android.Resource.String.Ok, this);
            Builder.SetOnCancelListener(this);
            Dialog = Builder.Show();
            return true;
        }

        public void SetItems(List<SpinnerItem> items, int position, ISpinnerListener listener)
        {
            Items = items;
            Listener = listener;

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
    }

    public class ItemAdapter : RecyclerView.Adapter
    {
        public List<SpinnerItem> Items { get; set; }
        public List<SpinnerItem> OriginalValues { get; set; }
        public LayoutInflater Inflater { get; set; }
        public SpinnerSearch Spinner { get; set; }
        public TextView EmptyText { get; set; }

        public override int ItemCount => Items.Count;

        public ItemAdapter(Context context, List<SpinnerItem> items, SpinnerSearch spinner, TextView emptyText)
        {
            OriginalValues = items;
            Items = new List<SpinnerItem>();
            Inflater = LayoutInflater.From(context);
            Spinner = spinner;
            EmptyText = emptyText;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as ItemViewHolder;
            holder.TextView.Text = item.Name;
            if (item.IsSelected)
                holder.TextView.SetTypeface(null, TypefaceStyle.Bold);
            else
                holder.TextView.SetTypeface(null, TypefaceStyle.Normal);

            holder.TextView.Click += (s, e) =>
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].IsSelected = false;
                    if (i == position)
                    {
                        Items[i].IsSelected = true;
                    }
                }
                SpinnerSearch.Dialog.Dismiss();
                Spinner.OnCancel(SpinnerSearch.Dialog);
            };

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                   Inflate(Resource.Layout.item_select_single, parent, false);
            var vh = new ItemViewHolder(itemView);
            return vh;
        }

        public void Filter(string query)
        {
            var count = Items.Count;
            var items = !string.IsNullOrWhiteSpace(query) ? OriginalValues.Where(i => i.Name.Contains(query)).ToList() : OriginalValues;
            Items = items;
            EmptyText.Visibility = Items.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            NotifyItemRangeRemoved(0, count);
            NotifyItemRangeInserted(0, Items.Count);
        }

    }

    class ItemViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }

        public ItemViewHolder(View itemView) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.txvItem);
        }
    }
}