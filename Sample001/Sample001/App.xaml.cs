﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Sample001
{
	public partial class App : Application
	{
        public static string dbPath;
        public App(string dbPath)
        {

            //AppのdbPathに引数のパスを設定します
            App.dbPath = dbPath;
            InitializeComponent();
            MainPage = new NavigationPage
            {
                BindingContext = new MainPageViewModel(),
            };
        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage{
                BindingContext = new MainPageViewModel(),
            };
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
