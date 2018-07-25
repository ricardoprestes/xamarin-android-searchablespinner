using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace SearchableSpinner.Droid.Controls
{
    public class BaseSpinnerSearch : AppCompatSpinner, IDialogInterfaceOnCancelListener
    {
        public List<SpinnerItem> Items { get; set; }
        public string DefaultText { get; set; }
        public string SpinnerTitle { get; set; }
        public SpinnerItemAdapter ItemAdapter { get; set; }
        public Android.Widget.SearchView SearchView { get; set; }
        public TextView EmptyText { get; set; }
        public bool IsMultiSelect { get; set; }

        public static AlertDialog.Builder Builder;
        public static AlertDialog Dialog;

        public BaseSpinnerSearch(Context context) : base(context)
        {
        }

        public BaseSpinnerSearch(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public virtual void OnCancel(IDialogInterface dialog)
        {

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
            ItemAdapter = new SpinnerItemAdapter(Context, Items, this, EmptyText, IsMultiSelect);
            ItemAdapter.Filter(null);
            listView.SetAdapter(ItemAdapter);
            listView.SetLayoutManager(new LinearLayoutManager(Context));

            SearchView.QueryTextChange += (s, e) =>
            {
                ItemAdapter.Filter(e.NewText);
            };

            Builder.SetPositiveButton(Android.Resource.String.Ok, (s, e) =>
            {
                OnCancel(Dialog);
            });

            Builder.SetOnCancelListener(this);
            Dialog = Builder.Show();
            return true;
        }
    }

    public class SpinnerItemAdapter : RecyclerView.Adapter
    {
        public List<SpinnerItem> Items { get; set; }
        public List<SpinnerItem> OriginalValues { get; set; }
        public LayoutInflater Inflater { get; set; }
        public BaseSpinnerSearch Spinner { get; set; }
        public TextView EmptyText { get; set; }
        public bool IsMultiSelect { get; set; }
        public override int ItemCount => Items.Count;

        public SpinnerItemAdapter(Context context, List<SpinnerItem> items, BaseSpinnerSearch spinner, TextView emptyText, bool isMultiSelect = false)
        {
            OriginalValues = items;
            Items = new List<SpinnerItem>();
            Inflater = LayoutInflater.From(context);
            Spinner = spinner;
            EmptyText = emptyText;
            IsMultiSelect = isMultiSelect;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as SpinnerItemViewHolder;
            holder.TextView.Text = item.Name;
            if (IsMultiSelect)
                holder.CheckBox.Checked = item.IsSelected;
            else
            {
                if (item.IsSelected)
                    holder.TextView.SetTypeface(null, TypefaceStyle.Bold);
                else
                    holder.TextView.SetTypeface(null, TypefaceStyle.Normal);
            }

            holder.ItemView.Click += (s, e) =>
            {
                if (IsMultiSelect)
                {
                    Items[position].IsSelected = !Items[position].IsSelected;
                    holder.CheckBox.Checked = Items[position].IsSelected;
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        Items[i].IsSelected = i == position;
                    }
                    BaseSpinnerSearch.Dialog.Dismiss();
                    Spinner.OnCancel(BaseSpinnerSearch.Dialog);
                }
            };

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = IsMultiSelect ? Resource.Layout.item_select_multiple : Resource.Layout.item_select_single;
            View itemView = LayoutInflater.From(parent.Context).Inflate(layout, parent, false);
            var vh = new SpinnerItemViewHolder(itemView);
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

    class SpinnerItemViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; set; }
        public CheckBox CheckBox { get; set; }

        public SpinnerItemViewHolder(View itemView) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.txvItem);
            CheckBox = itemView.FindViewById<CheckBox>(Resource.Id.chkItemChecked);
        }
    }
}