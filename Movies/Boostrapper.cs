﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using VideoPlayer;
using VideoPlayerView.FilePlayer;
using Common.Interfaces;
using Common.ApplicationCommands;
using Common.FileHelper;

namespace RealMediaControl
{
    public class Bootstrapper : UnityBootstrapper
    {

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    var moduleCatalog = new DirectoryModuleCatalog();
        //    //moduleCatalog.ModulePath = @".\Modules";
        //    //moduleCatalog.ModulePath = AppDomain.CurrentDomain.BaseDirectory + "Modules";
        //    return moduleCatalog;
        //}

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            return mappings;
        }

        protected override DependencyObject CreateShell()
        {
            CreateHelper.Folder();
            CreateHelper.LoadFiles();

            MainView shell = new MainView();
            this.Container.RegisterInstance<IShell>(shell);
            this.Container.RegisterType<IPlayFile,PlayFile>();
            return shell;
        }

        

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }
    }
}
