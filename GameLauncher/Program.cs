﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncherReborn;
using Microsoft.Win32;
using CommandLine;
using System.Globalization;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.InsiderKit;
using System.Reflection;
using WindowsFirewallHelper;
using GameLauncher.App.Classes.Events;
using Newtonsoft.Json;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.ModNet;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Visuals;

namespace GameLauncher
{
    internal static class Program
    {
        private static string LatestUpdaterBuildVersion = "1.0.0.4";

        internal class Arguments
        {
            [Option('p', "parse", Required = false, HelpText = "Parses URI")]
            public string Parse { get; set; }
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(Main2);
        }

        private static void Main2(Arguments args)
        {

            if (!DetectLinux.LinuxDetected())
            {
                //Check if User has .NETFramework 4.6.2 or later Installed
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    if (ndpKey != null && ndpKey.GetValue("Release") != null && (int)ndpKey.GetValue("Release") >= 394802)
                    {
                        //Install Carbon Crew CA
                        CertificateStore.Check();
                    }
                    else
                    {
                        DialogResult frameworkError = MessageBox.Show(null, "This application requires one of the following versions of the .NET Framework:\n" +
                            " .NETFramework, Version=v4.6.2 \n\nDo you want to install this .NET Framework version now?", "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                        if (frameworkError == DialogResult.Yes)
                        {
                            Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                        }
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                }
            }

            File.Delete("communication.log");
            File.Delete("launcher.log");

            Log.StartLogging();

            FileSettingsSave.NullSafeSettings();
            FileAccountSave.NullSafeAccount();

            Self.currentLanguage = CultureInfo.CurrentCulture.Name.Split('-')[0].ToUpper();
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            if (UriScheme.IsCommandLineArgumentsInstalled())
            {
                UriScheme.InstallCommandLineArguments(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), AppDomain.CurrentDomain.FriendlyName));
                if (args.Parse != null)
                {
                    new UriScheme(args.Parse);
                }
            }

            if (EnableInsider.ShouldIBeAnInsider() == true)
            {
                Log.Build("INSIDER: GameLauncher " + Application.ProductVersion + "_" + EnableInsider.BuildNumber());
            }
            else
            {
                Log.Build("BUILD: GameLauncher " + Application.ProductVersion);
            }

            if (Properties.Settings.Default.IsRestarting)
            {
                Properties.Settings.Default.IsRestarting = false;
                Properties.Settings.Default.Save();
                Thread.Sleep(3000);
            }

            if (!DetectLinux.LinuxDetected())
            {
               //Windows Firewall Runner
                if (!string.IsNullOrEmpty(FileSettingsSave.FirewallStatus))
                {
                    string nameOfLauncher = "SBRW - Game Launcher";
                    string localOfLauncher = Assembly.GetEntryAssembly().Location;

                    string nameOfUpdater = "SBRW - Game Launcher Updater";
                    string localOfUpdater = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "GameLauncherUpdater.exe");

                    string groupKeyLauncher = "Game Launcher for Windows";
                    string descriptionLauncher = "Soapbox Race World";

                    bool removeFirewallRule = false;
                    bool firstTimeRun = false;

                    if (FileSettingsSave.FirewallStatus == "Not Excluded")
                    {
                        firstTimeRun = true;
                        FileSettingsSave.FirewallStatus = "Excluded";
                    }
                    else if (FileSettingsSave.FirewallStatus == "Reset")
                    {
                        removeFirewallRule = true;
                        FileSettingsSave.FirewallStatus = "Not Excluded";
                    }

                    FileSettingsSave.SaveSettings();

                    //Inbound & Outbound
                    FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfLauncher, localOfLauncher, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);
                    FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfUpdater, localOfUpdater, groupKeyLauncher, descriptionLauncher, FirewallProtocol.Any);

                    //This Removes the Game File Exe From Firewall
                    //To Find the one that Adds the Exe To Firewall -> Search for `OnDownloadFinished()`
                    string CurrentGameFilesExePath = Path.Combine(FileSettingsSave.GameInstallation + "\\nfsw.exe");

                    if (File.Exists(CurrentGameFilesExePath) && removeFirewallRule == true)
                    {
                        string nameOfGame = "SBRW - Game";
                        string localOfGame = CurrentGameFilesExePath;

                        string groupKeyGame = "Need for Speed: World";
                        string descriptionGame = groupKeyGame;

                        //Inbound & Outbound
                        FirewallHelper.DoesRulesExist(removeFirewallRule, firstTimeRun, nameOfGame, localOfGame, groupKeyGame, descriptionGame, FirewallProtocol.Any);
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            VisualsAPIChecker.PingAPIStatus();

            /* Set Launcher Directory */
            Log.Info("CORE: Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            if (!DetectLinux.LinuxDetected()) 
            {
                Log.Info("CORE: Checking current directory");

                switch (Self.CheckFolder(Directory.GetCurrentDirectory())) 
                {
                    case FolderType.IsTempFolder:
                        MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsUsersFolders:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher.\n\nSpecial Folders such as:" +
                            "\n\nDownloads, Documents, Desktop, Videos, Music, OneDrive, or Any Type of User Folders" +
                            "\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsProgramFilesFolder:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher." +
                            "\n\nSpecial Folders such as:\n\nProgram Files or Program Files (x86)\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsWindowsFolder:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher." +
                            "\n\nSpecial Folder such as:\n\nWindows\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                }

                if (!Self.HasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath)))
                {
                    MessageBox.Show("This application requires admin priviledge");
                }

                //Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon
                try
                {
                    try
                    {
                        switch (APIStatusChecker.CheckStatus("http://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest"))
                        {
                            case API.Online:
                                WebClient update_data = new WebClient();
                                update_data.CancelAsync();
                                update_data.Headers.Add("user-agent", "GameLauncherUpdater " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                update_data.DownloadStringAsync(new Uri("http://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest"));
                                update_data.DownloadStringCompleted += (sender, e) => {
                                    GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(e.Result);

                                    if (GHAPI.TagName != null)
                                    {
                                        Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GHAPI.TagName);
                                        LatestUpdaterBuildVersion = GHAPI.TagName;
                                    }
                                    Log.Info("LAUNCHER UPDATER: Latest Version -> " + LatestUpdaterBuildVersion);
                                };
                                break;
                            default:
                                Log.Error("LAUNCHER UPDATER: Failed to Retrive Latest Updater Information from GitHub");
                                break;
                        }
                    }
                    catch
                    {
                        var GetLatestUpdaterBuildVersion = new WebClient().DownloadString(Self.secondstaticapiserver + "/Version.txt");
                        if (!string.IsNullOrEmpty(GetLatestUpdaterBuildVersion))
                        {
                            Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GetLatestUpdaterBuildVersion);
                            LatestUpdaterBuildVersion = GetLatestUpdaterBuildVersion;
                        }
                    }
                    Log.Info("LAUNCHER UPDATER: Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                }
                catch (Exception ex)
                {
                    Log.Error("LAUNCHER UPDATER: Failed to get new version file: " + ex.Message);
                }
            }

            if (!DetectLinux.LinuxDetected())
            {
                //Windows 7 Fix
                if ((string.IsNullOrEmpty(FileSettingsSave.Win7UpdatePatches) && WindowsProductVersion.GetWindowsNumber() == 6.1) || FileSettingsSave.Win7UpdatePatches == "0")
                {
                    if (Self.GetInstalledHotFix("KB3020369") == false || Self.GetInstalledHotFix("KB3125574") == false)
                    {
                        String messageBoxPopupKB = String.Empty;
                        messageBoxPopupKB = "Hey Windows 7 User, we've detected a potential issue of some missing Updates that are required.\n";
                        messageBoxPopupKB += "We found that these Windows Update packages are showing as not installed:\n\n";

                        if (Self.GetInstalledHotFix("KB3020369") == false) messageBoxPopupKB += "- Update KB3020369\n";
                        if (Self.GetInstalledHotFix("KB3125574") == false) messageBoxPopupKB += "- Update KB3125574\n";

                        messageBoxPopupKB += "\nAditionally, we must add a value to the registry:\n";

                        messageBoxPopupKB += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n/SCHANNEL/Protocols/TLS 1.2/Client\n";
                        messageBoxPopupKB += "- Value: DisabledByDefault -> 0\n\n";

                        messageBoxPopupKB += "Would you like to add those values?";
                        DialogResult replyPatchWin7 = MessageBox.Show(null, messageBoxPopupKB, "GameLauncherReborn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (replyPatchWin7 == DialogResult.Yes)
                        {
                            RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                            key.SetValue("DisabledByDefault", 0x0);

                            MessageBox.Show(null, "Registry option set, Remember that the changes may require a system reboot to take effect", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show(null, "Roger that, There may be some issues connecting to the servers.", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        FileSettingsSave.Win7UpdatePatches = "1";
                        FileSettingsSave.SaveSettings();
                    }
                }

                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
                {
                    var result = MessageBox.Show(
                        "You do not have the 32-bit 2015-2019 VC++ Redistributable Package installed.\n \nThis will install in the Background\n \nThis may restart your computer. \n \nClick OK to install it.",
                        "Compatibility",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    var wc = new WebClient();
                    wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x86.exe", "VC_redist.x86.exe");
                    var proc = Process.Start(new ProcessStartInfo
                    {
                        Verb = "runas",
                        Arguments = "/quiet",
                        FileName = "VC_redist.x86.exe"
                    });

                    if (proc == null)
                    {
                        MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                if (Environment.Is64BitOperatingSystem == true)
                {
                    if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x64))
                    {
                        var result = MessageBox.Show(
                            "You do not have the 64-bit 2015-2019 VC++ Redistributable Package installed.\n \nThis will install in the Background\n \nThis may restart your computer. \n \nClick OK to install it.",
                            "Compatibility",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning);

                        if (result != DialogResult.OK)
                        {
                            MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        var wc = new WebClient();
                        wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x64.exe", "VC_redist.x64.exe");
                        var proc = Process.Start(new ProcessStartInfo
                        {
                            Verb = "runas",
                            Arguments = "/quiet",
                            FileName = "VC_redist.x64.exe"
                        });

                        if (proc == null)
                        {
                            MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            Console.WriteLine("Application path: " + Path.GetDirectoryName(Application.ExecutablePath));

            if (!string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                if (!Self.HasWriteAccessToFolder(FileSettingsSave.GameInstallation))
                {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                }
            }

            //INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
            //Updated RedTheKitsune Code so it downloads the file if its missing. It also restarts the launcher if the user click on yes on Prompt. - DavidCarbon
            if (!File.Exists("LZMA.dll"))
            {
                try
                {
                    Log.Warning("CORE: Starting LZMA downloader");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(new Uri(Self.fileserver + "/LZMA.dll"), "LZMA.dll");
                    }

                    DialogResult restartApp = MessageBox.Show(null, "Downloaded Missing LZMA.dll File. \nPlease Restart Launcher, Thanks!", "GameLauncher Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (restartApp == DialogResult.Yes)
                    {
                        Properties.Settings.Default.IsRestarting = true;
                        Properties.Settings.Default.Save();
                        Application.Restart();

                    }
                    
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
                catch (Exception ex)
                {
                    Log.Error("CORE: Failed to download LZMA. " + ex.Message);
                }
            }

            //StaticConfiguration.DisableErrorTraces = false;

            if (!File.Exists("servers.json"))
            {
                try 
                {
                    File.WriteAllText("servers.json", "[]");
                } catch { /* ignored */ }
            }

            if (Properties.Settings.Default.IsRestarting)
            {
                Properties.Settings.Default.IsRestarting = false;
                Properties.Settings.Default.Save();
                Thread.Sleep(3000);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (Debugger.IsAttached)
            {
                ShowMainScreen();
            } 
            else
            {
                if (NFSW.IsNFSWRunning())
                {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try
                {
                    if (mutex.WaitOne(0, false))
                    {
                        string[] files = {
                            "CommandLine.dll - 2.8.0",
                            "DiscordRPC.dll - 1.0.169.0",
                            "Flurl.dll - 3.0.1",
                            "Flurl.Http.dll - 2.4.2",
                            "INIFileParser.dll - 2.5.2",
                            "LZMA.dll - 9.10 beta",
                            "Microsoft.WindowsAPICodePack.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.Shell.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.ShellExtensions.dll - 1.1.0.0",
                            "Nancy.dll - 2.0.0",
                            "Nancy.Hosting.Self.dll - 2.0.0",
                            "Newtonsoft.Json.dll - 12.0.3",
                            "System.Runtime.InteropServices.RuntimeInformation.dll - 4.6.24705.01. Commit Hash: 4d1af962ca0fede10beb01d197367c2f90e92c97",
                            "System.ValueTuple.dll - 4.6.26515.06 @BuiltBy: dlab-DDVSOWINAGE059 @Branch: release/2.1 @SrcCode: https://github.com/dotnet/corefx/tree/30ab651fcb4354552bd4891619a0bdd81e0ebdbf",
                            "WindowsFirewallHelper.dll - 1.6.3.40"
                        };

                        var missingfiles = new List<string>();

                        if (!DetectLinux.LinuxDetected())
                        { //MONO Hates that...
                            foreach (var file in files) {
                                var splitFileVersion = file.Split(new string[] { " - " }, StringSplitOptions.None);

                                if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + splitFileVersion[0]))
                                {
                                    missingfiles.Add(splitFileVersion[0] + " - Not Found");
                                } 
                                else
                                {
                                    try
                                    {
                                        var versionInfo = FileVersionInfo.GetVersionInfo(splitFileVersion[0]);
                                        string[] versionsplit = versionInfo.ProductVersion.Split('+');
                                        string version = versionsplit[0];

                                        if (version == "")
                                        {
                                            missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                        } 
                                        else
                                        { 
                                            if (Self.CheckArchitectureFile(splitFileVersion[0]) == false) 
                                            {
                                                missingfiles.Add(splitFileVersion[0] + " - Wrong Architecture");
                                            } 
                                            else
                                            {
                                                if (version != splitFileVersion[1])
                                                {
                                                    missingfiles.Add(splitFileVersion[0] + " - Invalid Version (" + splitFileVersion[1] + " != " + version + ")");
                                                }
                                            }
                                        }
                                    } 
                                    catch
                                    {
                                        missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                    }
                                }
                            }
                        }
                        if (missingfiles.Count != 0)
                        {
                            ShowSplashScreen(false);

                            var message = "Cannot launch GameLauncher. The following files are invalid:\n\n";

                            foreach (var file in missingfiles)
                            {
                                message += "• " + file + "\n";
                            }

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Theming.CheckIfThemeExists();
                            ShowSplashScreen(true);
                        }
                    } 
                    else
                    {
                        ShowSplashScreen(false);
                        MessageBox.Show(null, "An instance of Launcher is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } 
                finally
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

        private static void ShowSplashScreen(bool Status)
        {
            if (!Debugger.IsAttached && !DetectLinux.LinuxDetected())
            {
                SplashScreen f = new SplashScreen();
                f.Shown += new EventHandler((o, e) =>
                {
                    Thread ST = new Thread(() =>
                    {
                        Log.Info("SPLASH SCREEN: Closing Splash Screen");
                        Thread.Sleep(4000);
                        f.Invoke(new Action(() => { f.Close(); }));
                    })
                    {
                        IsBackground = true
                    };
                    ST.Start();
                });
                Application.Run(f);
            }

            if (Status == true)
            {
                ShowMainScreen();
            }
        }

        private static void ShowMainScreen()
        {
            if (VisualsAPIChecker.WOPLAPI == false)
            {
                DialogResult restartAppNoApis = MessageBox.Show(null, "There's no internet connection, Launcher might crash \n \nClick Yes to Close Launcher \nor \nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (restartAppNoApis == DialogResult.No)
                {
                    MessageBox.Show("Good Luck... \n No Really \n ...Good Luck", "GameLauncher Will Continue, When It Failed To Connect To API");
                    Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and Will Continue");
                }

                if (restartAppNoApis == DialogResult.Yes)
                {
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
            }

            ServerListUpdater.GetList();
            CDNListUpdater.GetList();
            LauncherUpdateCheck.CheckAvailability();

            if (!DetectLinux.LinuxDetected())
            {
                if (!File.Exists("GameLauncherUpdater.exe"))
                {
                    Log.Info("LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                            {
                                if (new FileInfo("GameLauncherUpdater.exe").Length == 0)
                                {
                                    File.Delete("GameLauncherUpdater.exe");
                                }
                            };
                            wc.DownloadFile(new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("LAUCHER UPDATER: Failed to download updater. " + ex.Message);
                    }
                }
                else if (File.Exists("GameLauncherUpdater.exe"))
                {
                    String GameLauncherUpdaterLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameLauncherUpdater.exe");
                    var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(GameLauncherUpdaterLocation);
                    var LauncherUpdaterBuildNumber = LauncherUpdaterBuild.FileVersion;
                    var UpdaterBuildNumberResult = LauncherUpdaterBuildNumber.CompareTo(LatestUpdaterBuildVersion);

                    Log.Build("LAUNCHER UPDATER BUILD: GameLauncherUpdater " + LauncherUpdaterBuildNumber);
                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: " + UpdaterBuildNumberResult + " Builds behind latest Updater!");
                    }
                    else
                    {
                        Log.Info("LAUNCHER UPDATER: Latest GameLauncherUpdater!");
                    }

                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: Downloading New GameLauncherUpdater.exe");
                        File.Delete("GameLauncherUpdater.exe");

                        try
                        {
                            using (WebClient wc = new WebClient())
                            {
                                wc.DownloadFile(new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("LAUNCHER UPDATER: Failed to download new updater. " + ex.Message);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                var linksPath = Path.Combine(FileSettingsSave.GameInstallation + "\\.links");
                ModNetLinksCleanup.CleanLinks(linksPath);
            }

            Log.Info("PROXY: Starting Proxy");
            ServerProxy.Instance.Start();

            Log.Visuals("CORE: Starting MainScreen");
            Application.Run(new MainScreen());
        }
    }
}
