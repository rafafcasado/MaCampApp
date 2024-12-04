using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;

namespace Guia_de_Camping.Droid
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
	public class Splash : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			StartActivity (typeof(MainActivity));
            Finish();
		}
	}
}