using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for ServiceProvider.xaml
    /// </summary>
    public partial class ServiceProvider : Window
    {
        public ServiceProvider()
        {
            InitializeComponent();
        } 

        public void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> serviceList = new List<string>();
            foreach (CheckBox c in services.Children)
            {
                if (c.IsChecked == true)
                {
                    serviceList.Add(c.Name);
                }
            }
            this.Close();
        }

        private string GetSteamDirectory()
        {
            string steamInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey steamRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Valve\Steam");
            if (steamRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                steamRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Valve\Steam");
            }
            object registryEntry = steamRegistryKey.GetValue("InstallPath");
            if (registryEntry != null)
                steamInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            steamRegistryKey.Close();
            return Path.Combine(steamInstallPath, @"steamapps");
        }
        private string[] GetSteamGameDirectoryList()
        {
            // For the sake of simplicity (since it can only be done manually these days), we will assume that the user only has one Steam library folder, and that it is not external. This means no reading from libraryfolders.vdf
            string steamLibraryPath = Path.Combine(GetSteamDirectory(), @"common");
            if (Directory.Exists(steamLibraryPath))
                return Directory.GetDirectories(steamLibraryPath);
            return null;
        }
        private string[] GetSteamGameList()
        {
            string steamAppPath = GetSteamDirectory();
            List<string> gameList = new List<string>();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach (string line in File.ReadLines(manifest))
                {
                    if (line.Contains("name"))
                    {
                        int quoteCount = 0;
                        int i = 0;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            gameList.Add(line.Substring(i + 1, line.Length - i - 2)); // we chop off the ends to avoid the quotes
                    }
                }
            }
            return gameList.ToArray();
        }
        private string[] GetSteamInstallDirectoryList()
        {
            string steamAppPath = GetSteamDirectory();
            List<string> gameList = new List<string>();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach (string line in File.ReadLines(manifest))
                {
                    if (line.Contains("installdir"))
                    {
                        int quoteCount = 0;
                        int i;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            gameList.Add(line.Substring(i + 1, line.Length - i - 2)); // we chop off the ends to avoid the quotes
                    }
                }
            }
            return gameList.ToArray();
        }
    }
}
