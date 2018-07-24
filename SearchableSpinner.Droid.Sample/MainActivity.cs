using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using SearchableSpinner.Droid.Controls;
using System.Collections.Generic;

namespace SearchableSpinner.Droid.Sample
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
    {
        public SpinnerSearch SpnTest { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var items = new List<SpinnerItem>();
            for (int i = 0; i < 30; i++)
            {
                items.Add(new SpinnerItem { Id = i, Name = "Item " + i });
            }

            SpnTest = FindViewById<SpinnerSearch>(Resource.Id.spnTest);
            SpnTest.SetItems(items, -1, null);

		}

		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

	}
}

