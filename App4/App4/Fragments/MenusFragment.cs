
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using App4;
using FloatingActionButton = Com.Github.Clans.Fab.FloatingActionButton;
using FloatingActionMenu = Com.Github.Clans.Fab.FloatingActionMenu;
using Fragment = Android.Support.V4.App.Fragment;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using Android.Preferences;

namespace App4.Fragments
{
    public class MenusFragment : Fragment, View.IOnClickListener, EMDKManager.IEMDKListener
    {
        #region EMDKVariables
        private TextView statusView = null;
        private TextView dataView = null;

        EMDKManager emdkManager = null;
        BarcodeManager barcodeManager = null;
        Scanner scanner = null;
        #endregion
        private FloatingActionMenu menuRed;
        private FloatingActionButton fab1;
        private FloatingActionButton fab2;
        private FloatingActionButton fab3;



        private List<FloatingActionMenu> menus = new List<FloatingActionMenu> (6);
        private Handler mUiHandler = new Handler ();


        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate (Resource.Layout.menus_fragment, container, false);
        }

        public override void OnViewCreated (View view, Bundle savedInstanceState)
        {
            base.OnViewCreated (view, savedInstanceState);

            menuRed = view.FindViewById<FloatingActionMenu> (Resource.Id.menu_red);

            fab1 = view.FindViewById<FloatingActionButton> (Resource.Id.fab1);
            fab2 = view.FindViewById<FloatingActionButton> (Resource.Id.fab2);
            fab3 = view.FindViewById<FloatingActionButton> (Resource.Id.fab3);

            statusView = view.FindViewById<TextView>(Resource.Id.statusView);
            dataView = view.FindViewById<TextView>(Resource.Id.DataView);


            ContextThemeWrapper context = new ContextThemeWrapper (this.Activity, Resource.Style.MenuButtonsStyle);
            FloatingActionButton programFab2 = new FloatingActionButton (context);

            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);
            if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                statusView.Text = "Status: EMDKManager object creation failed ...";
            }
            else
            {
                statusView.Text = "Status: EMDKManager object creation succeeded ...";
            }
            programFab2.LabelText = "Programmatically added button";
            programFab2.SetImageResource (Resource.Drawable.ic_edit);
            fab1.Enabled = false;
            menuRed.SetOnMenuButtonClickListener (this);
            menuRed.SetClosedOnTouchOutside (true);
            menuRed.HideMenuButton (false);
;
        }
        #region EMDK



        void displayStatus(String status)
        {
            if (Looper.MainLooper.Thread == Java.Lang.Thread.CurrentThread())
            {
                statusView.Text = status;
            }
        }


        void displaydata(string data)
        {
            //if (Looper.MainLooper.Thread == Java.Lang.Thread.CurrentThread())
            //{
            //    dataView.Text += (data + "\n");
            //}

            ((MainActivity)this.Activity).setData(data);


            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            //ISharedPreferencesEditor editor = prefs.Edit();
            //editor.PutString("my_data", data);
            //editor.Apply();

        }

        void EMDKManager.IEMDKListener.OnOpened(EMDKManager emdkManager)
        {
            statusView.Text = "Status: EMDK Opened successfully ...";

            this.emdkManager = emdkManager;

            InitScanner();
        }
        void InitScanner()
        {
            if (emdkManager != null)
            {

                if (barcodeManager == null)
                {
                    try
                    {

                        //Get the feature object such as BarcodeManager object for accessing the feature.
                        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                        scanner = barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (scanner != null)
                        {

                            //Attahch the Data Event handler to get the data callbacks.
                            scanner.Data += scanner_Data;

                            //Attach Scanner Status Event to get the status callbacks.
                            scanner.Status += scanner_Status;

                            scanner.Enable();

                            //EMDK: Configure the scanner settings
                            ScannerConfig config = scanner.GetConfig();
                            config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                            config.ScanParams.DecodeLEDFeedback = true;
                            config.ReaderParams.ReaderSpecific.ImagerSpecific.PickList = ScannerConfig.PickList.Enabled;
                            config.DecoderParams.Code39.Enabled = true;
                            config.DecoderParams.Code128.Enabled = false;
                            scanner.SetConfig(config);

                        }
                        else
                        {
                            displayStatus("Failed to enable scanner.\n");
                        }
                    }
                    catch (ScannerException e)
                    {
                        displayStatus("Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        displayStatus("Error: " + ex.Message);
                    }
                }
            }


        }
        void DeinitScanner()
        {
            if (emdkManager != null)
            {

                if (scanner != null)
                {
                    try
                    {

                        scanner.Data -= scanner_Data;
                        scanner.Status -= scanner_Status;

                        scanner.Disable();


                    }
                    catch (ScannerException e)
                    {
                        Log.Debug(this.Class.SimpleName, "Exception:" + e.Result.Description);
                    }
                }

                if (barcodeManager != null)
                {
                    emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }
                barcodeManager = null;
                scanner = null;
            }


        }
        void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            String statusStr = "";

            //EMDK: The status will be returned on multiple cases. Check the state and take the action.
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                statusStr = "Scanner is idle and ready to submit read.";
                try
                {
                    if (scanner.IsEnabled && !scanner.IsReadPending)
                    {
                        scanner.Read();
                    }
                }
                catch (ScannerException e1)
                {
                    statusStr = e1.Message;
                }
            }
            if (state == StatusData.ScannerStates.Waiting)
            {
                statusStr = "Waiting for Trigger Press to scan";
            }
            if (state == StatusData.ScannerStates.Scanning)
            {
                statusStr = "Scanning in progress...";
            }
            if (state == StatusData.ScannerStates.Disabled)
            {
                statusStr = "Scanner disabled";
            }
            if (state == StatusData.ScannerStates.Error)
            {
                statusStr = "Error occurred during scanning";

            }
            displayStatus(statusStr);


        }
        void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    //displaydata(data.LabelType + " : " + data.Data);
                    displaydata(data.Data);
                }
            }


        }
        void EMDKManager.IEMDKListener.OnClosed()
        {
            statusView.Text = "Status: EMDK Open failed unexpectedly. ";

            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }
        #endregion
        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            base.OnActivityCreated (savedInstanceState);

     
            menus.Add (menuRed);
            fab1.Click += ActionButton_Click;
            fab2.Click += ActionButton_OpenSettings;
            fab3.Click += ActionButton_SetWallpaper;

            int delay = 400;
            foreach (var menu in menus) 
            {
                mUiHandler.PostDelayed (() => menu.ShowMenuButton (true), delay);
                delay += 150;
            }


        }

        private void ActionButton_SetWallpaper(object sender, EventArgs e)
        {
            WallpaperManager myWallpaperManager = WallpaperManager.GetInstance(Context.ApplicationContext);
            myWallpaperManager.SetResource(Resource.Mipmap.thdwallpaper);
        }

        private void ActionButton_OpenSettings(object sender, EventArgs e)
        {
            var pm = Application.Context.PackageManager;
            var packageName = "com.android.settings";
            var launchIntent = pm.GetLaunchIntentForPackage(packageName);
            Application.Context.StartActivity(launchIntent);
       
        }
        private void ActionButton_Click (object sender, EventArgs e)
        {
            FloatingActionButton fabButton = sender as FloatingActionButton;
            if (fabButton != null) 
            {
                if (fabButton.Id == Resource.Id.fab2) 
                {
                    fabButton.Visibility = ViewStates.Gone;
                } 
                else if (fabButton.Id == Resource.Id.fab3) 
                {
                    fabButton.Visibility = ViewStates.Visible;
                }
                Toast.MakeText (this.Activity, fabButton.LabelText, ToastLength.Short).Show ();
            }
        }


        public void OnClick (View v)
        {
            FloatingActionMenu menu = (FloatingActionMenu)v.Parent;
            if (menu.Id == Resource.Id.menu_red && menu.IsOpened) 
            {
                Toast.MakeText (this.Activity, menu.MenuButtonLabelText, ToastLength.Short).Show ();
            }

            menu.Toggle (animate: true);
        }

        private void ProgramFab1_Click (object sender, EventArgs e)
        {
            var fab = sender as FloatingActionButton;

            if (fab != null) 
            {
                fab.SetLabelColors (ContextCompat.GetColor (this.Activity, Resource.Color.grey),
                                    ContextCompat.GetColor (this.Activity, Resource.Color.light_grey),
                                    ContextCompat.GetColor (this.Activity, Resource.Color.white_transparent));
                fab.SetLabelTextColor (ContextCompat.GetColor (this.Activity, Resource.Color.black));
            }
        }

    }
}

