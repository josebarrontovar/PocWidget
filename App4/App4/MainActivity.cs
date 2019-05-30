using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;

using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using FloatingActionButton = Com.Github.Clans.Fab.FloatingActionButton;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using App4.Fragments;
using Android.Runtime;
using Android.Graphics;

namespace App4
{
    [Activity(Label = "Widget THD Test...", MainLauncher = true, Icon = "@mipmap/android")]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout drawerLayout;
        private ActionBarDrawerToggle toggle;
        private NavigationView navigationView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            StatusBar();
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.toggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawerLayout.AddDrawerListener(this.toggle);

            this.navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            if (savedInstanceState == null)
            {
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.fragment, new MenusFragment()).Commit();
            }
            else
            navigationView.SetCheckedItem(Resource.Id.home);
        }

        private void StatusBar()
        {
           WindowManagerLayoutParams localLayoutParams = new WindowManagerLayoutParams();

            localLayoutParams.Type = WindowManagerTypes.SystemError;
            localLayoutParams.Gravity = GravityFlags.Top;
            localLayoutParams.Flags = WindowManagerFlags.NotFocusable | 
                WindowManagerFlags.NotTouchModal | WindowManagerFlags.LayoutInScreen;
            localLayoutParams.Width = WindowManagerLayoutParams.MatchParent;
            localLayoutParams.Height = (int)(50 * Resources.DisplayMetrics.ScaledDensity);
            localLayoutParams.Format = Format.Transparent;

            IWindowManager manager = ApplicationContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            customViewGroup view = new customViewGroup(this);

            manager.AddView(view, localLayoutParams);
        }
        public class customViewGroup : ViewGroup
        {
            public customViewGroup(Context context) : base(context)
            {
            }
            public override bool OnTouchEvent(MotionEvent ev)
            {
                return true;
            }
            protected override void OnLayout(bool changed, int l, int t, int r, int b)
            {
                // throw new NotImplementedException();
            }
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            this.toggle.SyncState();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.navigationView.NavigationItemSelected -= NavigationView_NavigationItemSelected;
        }

        public override void OnBackPressed()
        {
            if (drawerLayout != null && drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
            {
                drawerLayout.CloseDrawer((int)GravityFlags.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            //this.drawerLayout.CloseDrawer((int)GravityFlags.Start);
            //Fragment fragment = null;
            //FragmentTransaction ft = SupportFragmentManager.BeginTransaction();

            //switch (e.MenuItem.ItemId)
            //{
                
            //    case Resource.Id.menus:
            //        fragment = new MenusFragment();
            //        break;
                
            //}

            //ft.Replace(Resource.Id.fragment, fragment).Commit();
            //e.Handled = true;
        }
    }
}