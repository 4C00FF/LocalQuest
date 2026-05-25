using LocalQuest.Models.Mid2018;
using QuerryNetworking.Core;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace LocalQuest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // clears the console
            Console.Clear();
            // sets the LocalQuest log level to debug
            Log.LogLevel = LogLevel.Debug;
            // set output to unicode (for emojis 🤑)
            Console.OutputEncoding = Encoding.Unicode;
            // set the console title to LocalQuest
            Console.Title = "LocalQuest";
            // hide the cursor for fancy ui 🙀
            Console.CursorVisible = false;

            bool FromLauncher = false;

            for(int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--URL")
                {
                    if(i == args.Length)
                    {
                        NetworkFiles.BaseURL = args[i + 1];
                    }
                    else
                    {
                        Log.Warn("Url change requested but not provided");
                    }
                }
                if(args[i] == "--FromLauncher")
                {
                    FromLauncher = true;
                }
            }

            // setup config file
            Config.Setup();
            // setup files
            FileManager.Setup();
            // setup defaults
            Setup.Defaults();
            // waits for the RoomManager to download the rooms
            RoomManager.DownloadRooms().Wait();
            // waits for the AvatarManager to setup the avatar items
            AvatarManager.SetupAvatarItems().Wait();

            // if QuerryNetworking debug logging is enabled
            if (Config.GetBool("NetworkDebug"))
            {
                // set the log level!!
                QuerryNetworking.Logging.Log.LogLevel = QuerryNetworking.Logging.LogLevel.Debug;
            }
            else
            {
                // else set it to info
                QuerryNetworking.Logging.Log.LogLevel = QuerryNetworking.Logging.LogLevel.Info;
            }
            // if autodetect doesn't exist
            if(string.IsNullOrEmpty(Config.GetString("AutoDetect")))
            {
                // set it true
                Config.SetBool("AutoDetect", true);
            }
            // if private dorm doesn't exist
            if(!Config.Exists("PrivateDorm"))
            {
                // default true
                Config.SetBool("PrivateDorm", true);
            }
            // if level config doesn't exist
            if (!Config.Exists("Level"))
            {
                // set it to 1 ✨
                Config.SetInt("Level", 1);
            }

            // if app hasn't been run before
            if (!Config.GetBool("FirstRun"))
            {
                // show setup/tutorial
                Setup.Tutorial();
                // clear the console
                Console.Clear();
            }
            // write the title
            UiTools.WriteTitle();
            // subtitle
            Console.WriteLine("LocalQuest - EpicQuest but localhost?! YEAH");

            // string to add to the end of 'server version:'
            string BetaString = "";
            // if beta
            if(Setup.Beta)
            {
                // add BETA
                BetaString = " [BETA]";
            }

            // write the info
            Console.WriteLine("Server Version: " + Setup.Version + BetaString);

            // show welcome banner
            UiTools.ShowBanner("Welcome, " + Config.GetString("DisplayName"));
            // reset foreground color 😭
            Console.ForegroundColor = ConsoleColor.White;

            // main menu options
            List<string> Options = new List<string>()
            {
                "Start server",
                "Modify profile",
                "Settings",
            };

            // create a random
            Random R = new Random();
            // 1/500 chance
            if(R.Next(0,500) == 1)
            {
                // for SILLY
                Options.Add("What's mew :3");
            }
            else
            {
                // else normal :(
                Options.Add("What's new");
            }

            // add join the discord option
            Options.Add("Join the discord");
            Options.Add("Give feedback");

            if(FromLauncher)
            {
                Options.Add("Go back to the launcher 😶‍🌫️");
            }

            // get user selection
            string Selection = UiTools.WriteControls(Options);

            // switch selection
            switch (Selection)
            {
                case "Start server":
                    // build choice (which will go to auto-detect if enabled)
                    BuildChoice();
                    return;

                case "Settings":
                    // go to settings
                    Settings();
                    return;

                case "Modify profile":
                    // go to profile settings
                    ProfileSettings();
                    return;

                case "What's new":
                    // go to what's new
                    WhatNew();
                    return;

                case "What's mew :3":
                    // go to what's new but SILLY
                    WhatNew();
                    return;

                case "Go back to the launcher 😶‍🌫️":
                    break;

                case "Give feedback":
                    // start process for opening discord
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "https://forms.gle/Kz6Z9GbPrg4M6CMt5",
                        UseShellExecute = true
                    });
                    Main(new string[0]);
                    break;

                case "Join the discord":
                    // start process for opening discord
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    { 
                        FileName = "https://discord.gg/CnzM6PXYsn",
                        UseShellExecute = true
                    });
                    Main(new string[0]);
                    return;
            }
        }

        static void Settings()
        {
            // clear the console
            Console.Clear();
            // write the title
            UiTools.WriteTitle();
            // write the subtitle
            Console.WriteLine("LocalQuest - Settings");

            // the settings options
            List<string> Controls = new List<string>()
            {
                "Go back",
                "Skip tutorial",
                "Manage data",
                "Private rooms",
                "Auto-Detect game version [currently: " + Config.GetBool("AutoDetect") + "]",
                "Use OpenRec/RebornRec style ui nav [currently: " + Config.GetBool("ReControl") + "]",
                "Log QuerryNetworking debug [currently: " + Config.GetBool("NetworkDebug") + "]",
            };

            // get if RebornRec compatibility mode is enabled
            bool ReCompat = Config.GetBool("ReCompat");
            // if it's not
            if (!ReCompat)
            {
                // show server port override option
                Controls.Add("Server port override [currently: " + SettingStringValue("ServerPort") + "]");
            }

            // add rebornrec compatibility option
            Controls.Add("RebornRec compatibility mode [currently: " + Config.GetBool("ReCompat") + "]");

            // write the controls
            string Selection = UiTools.WriteControls(Controls);

            // switch selection
            switch (Selection)
            {
                case "Go back":
                    // go back to the main menu
                    Main(new string[0]);
                    return;

                case "Skip tutorial":
                    // skip the tutorial by setting OOBE to '100' (which doesn't work on every build LOLL)
                    LocalQuest.Settings.SetSetting("Recroom.OOBE", "100");
                    // clear the console
                    Console.Clear();
                    // write the title
                    UiTools.WriteTitle();
                    // show complete
                    UiTools.ShowBanner("Complete!");
                    // press to go back
                    Console.WriteLine("Press any key to go back...");
                    // wait for input
                    Console.ReadKey();
                    // go back to settings
                    Settings();
                    break;

                case "Private rooms":
                    // go to private rooms settings
                    PrivateRoomSettings();
                    break;

                case "Manage data":
                    // go to data settings
                    SaveSettings();
                    break;

                case string S when S == ("Log QuerryNetworking debug [currently: " + Config.GetBool("NetworkDebug") + "]"):
                    // toggle QuerryNetworking debug logs
                    Config.SetBool("NetworkDebug", !Config.GetBool("NetworkDebug"));
                    // go back to settings
                    Settings();
                    break;
                
                case string S when S == ("Auto-Detect game version [currently: " + Config.GetBool("AutoDetect") + "]"):
                    // toggle auto-detect game version
                    Config.SetBool("AutoDetect", !Config.GetBool("AutoDetect"));
                    // go back to settings
                    Settings();
                    break;

                case string S when S == ("Use OpenRec/RebornRec style ui nav [currently: " + Config.GetBool("ReControl") + "]"):
                    // toggle FAST controls
                    Config.SetBool("ReControl", !Config.GetBool("ReControl"));
                    // go back to settings
                    Settings();
                    break;

                case string S when S == ("Server port override [currently: " + SettingStringValue("ServerPort") + "]"):
                    // change the 'ServerPort' setting 🚨
                    ChangeStringSetting("ServerPort");
                    // go back to settings
                    Settings();
                    break;

                case string S when S == ("RebornRec compatibility mode [currently: " + Config.GetBool("ReCompat") + "]"):
                    // toggle rebornrec compatibility mode
                    Config.SetBool("ReCompat", !Config.GetBool("ReCompat"));
                    if (Config.GetBool("ReCompat"))
                    {
                        // set server port to RebornRec port
                        Config.SetString("ServerPort", "2059");
                    }
                    else
                    {
                        // reset server port
                        Config.SetString("ServerPort", "");
                    }
                    // go back to settings
                    Settings();
                    break;
                
                default:
                    // settings
                    Settings();
                    break;
            }
        }

        static void PrivateRoomSettings()
        {
            // clear the console
            Console.Clear();
            // write title
            UiTools.WriteTitle();
            // write subtitle
            Console.WriteLine("Settings - Private Rooms");

            // the options
            List<string> Options = new List<string>()
            {
                "Go back",
                "Private Code [currently: " + SettingStringValue("PrivateCode") + "]",
                "Private DormRoom [currently: " + Config.GetBool("PrivateDorm") + "]"
            };
            // get a selection
            string Selection = UiTools.WriteControls(Options);
            // switch selection
            switch(Selection)
            {
                case "Go back":
                    // go back to settings
                    Settings();
                    break;
                case string S when S == ("Private Code [currently: " + SettingStringValue("PrivateCode") + "]"):
                    // change the private code setting
                    ChangeStringSetting("PrivateCode");
                    // go back to private rooms settings
                    PrivateRoomSettings();
                    break;
                case string S when S == ("Private DormRoom [currently: " + Config.GetBool("PrivateDorm") + "]"):
                    // toggle the private dorm setting
                    Config.SetBool("PrivateDorm", !Config.GetBool("PrivateDorm"));
                    // go back to private rooms settings
                    PrivateRoomSettings();
                    break;
            }
        }

        static void ProfileSettings()
        {
            // clear the console
            Console.Clear();
            // write the title
            UiTools.WriteTitle();
            // write subtitle
            Console.WriteLine("Settings - Profile");

            // the possible choices
            string Selection = UiTools.WriteControls(new List<string>()
            {
                "Go back",
                "Import from modern",
                "Reset your profile picture [currently: " + SettingStringValue("PFP") + "]",
                "Change Username [Currently: @" + Config.GetString("Username") + "]",
                "Change DisplayName [Currently: " + Config.GetString("DisplayName") + "]",
                "Change Level [Currently: " + Config.GetInt("Level") + "]",
                "Change your group [currently: " + SettingStringValue("CurrentGroup") + "]"
            });

            // switch selection
            switch (Selection)
            {
                case "Go back":
                    // go back to main menu
                    Main(new string[0]);
                    break;
                case "Import from modern":
                    // import profile from modern
                    Setup.ImportModernProfile();
                    // go back to profile settings
                    ProfileSettings();
                    break;
                case string S when S == ("Reset your profile picture [currently: " + SettingStringValue("PFP") + "]"):
                    // set pfp to defaultpfp
                    Config.SetString("PFP", "DefaultPFP");
                    // go back to profile settings
                    ProfileSettings();
                    break;
                case string S when S == "Change Username [Currently: @" + Config.GetString("Username") + "]":
                    // change username setting string
                    ChangeStringSetting("Username");
                    // go back to profile settings
                    ProfileSettings();
                    break;
                case string S when S == "Change DisplayName [Currently: " + Config.GetString("DisplayName") + "]":
                    // change displayname setting string
                    ChangeStringSetting("DisplayName");
                    // go back to profile settings
                    ProfileSettings();
                    break;
                case string S when S == ("Change your group [currently: " + SettingStringValue("CurrentGroup") + "]"):
                    // change the current group setting
                    ChangeStringSetting("CurrentGroup");
                    // go back to profile settings
                    ProfileSettings();
                    break;
                case string S when S == ("Change Level [Currently: " + Config.GetInt("Level") + "]"):
                    // change level setting
                    ChangeIntSetting("Level");
                    // go back to profile settings
                    ProfileSettings();
                    break;
                default:
                    // go back to profile settings
                    ProfileSettings();
                    break;
            }
        }

        static void SaveSettings()
        {
            // clear the console
            Console.Clear();
            // write the title
            UiTools.WriteTitle();
            // write subtitle
            Console.WriteLine("Settings - Data");
            // list the options
            List<string> Options = new List<string>()
            {
                "Go back",
                "Reset data",
                "Update data",
                "Import data from somewhere else"
            };
            // get a selection from the user
            string Result = UiTools.WriteControls(Options);
            // swithc result
            switch(Result)
            {
                case "Go back":
                    // go back to settings
                    Settings();
                    break;
                case "Reset data":
                    // clear the console
                    Console.Clear();
                    // write the title
                    UiTools.WriteTitle();
                    // subtitle
                    Console.WriteLine("LocalQuest - Reset data");
                    // ask if sure
                    Console.WriteLine("Are you sure?");
                    // get a response
                    string O = UiTools.WriteControls(new List<string>() { "yes", "no" });
                    // if they say no
                    if (O == "no")
                    {
                        // go back
                        Settings();
                        return;
                    }

                    // clear the console again
                    Console.Clear();
                    // write the title
                    UiTools.WriteTitle();
                    // subtitle
                    Console.WriteLine("Deleting data folder...");
                    try
                    {
                        // delete data folder and everything else :sob:
                        Directory.Delete(Directory.GetCurrentDirectory() + "/Data/", true);
                    }
                    catch
                    {
                        // log error
                        Log.Error("Failed to delete data folder!");
                    }
                    // show clearing config
                    Console.WriteLine("Clearing config data...");
                    try
                    {
                        // clear config
                        Config.Clear();
                    }
                    catch
                    {
                        // log error
                        Log.Error("Failed to clear config data!");
                    }
                    // show banner complete! 📊
                    UiTools.ShowBanner("Done! Press [ENTER] to continue...");
                    // go back to main menu (tutorial?!)
                    Main(new string[0]);
                    break;
                case "Import data from somewhere else":
                    // go to data import
                    ImportChoice();
                    break;
                case "Update data":
                    // download avatar items
                    List<AvatarItem> NewItems = AvatarManager.DownloadAvatarItems().Result;
                    if (AvatarManager.AvatarItems != null && NewItems.Count < AvatarManager.AvatarItems.Count)
                    {
                        Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAHHH");
                        Thread.Sleep(150);
                        Console.WriteLine("OWWWWWW");
                        Thread.Sleep(150);
                        Console.WriteLine("IT'S SO PAINFULL");
                        Thread.Sleep(150);
                        Console.WriteLine("HELPPPPPPPPP");
                        Thread.Sleep(750);
                    }
                    // save updated avatar items
                    FileManager.WriteJSON("Avatar/AvatarItems", NewItems);
                    // go back to save settings
                    SaveSettings();
                    break;
                default:
                    // go back to save settings
                    SaveSettings();
                    break;
            }
        }

        static void ImportChoice()
        {
            // clear the console
            Console.Clear();
            // write title
            UiTools.WriteTitle();
            // subtitle
            Console.WriteLine("Where are you importing from?");

            // show banner OVERWRITE
            UiTools.ShowBanner("This WILL overwrite your existing SaveData", ConsoleColor.Yellow);

            // the options
            List<string> Options = new List<string>()
            {
                "None! Go back",
                "RebornRec",
                "OpenRec"
            };

            // get a selection
            string Select = UiTools.WriteControls(Options);

            switch(Select)
            {
                case "None! Go back":
                    SaveSettings();
                    break;
                case "RebornRec":
                    Setup.ImportData();
                    Main(new string[0]);
                    break;
                case "OpenRec":
                    Setup.ImportOpenData();
                    Main(new string[0]);
                    break;
                default:
                    ImportChoice();
                    break;
            }
        }

        public static string SettingStringValue(string Setting)
        {
            string Result = Config.GetString(Setting);
            if(string.IsNullOrEmpty(Result))
            {
                return "none";
            }
            return Result;
        }

        static void ChangeStringSetting(string Setting)
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Change your " + Setting + "\nPlease input a value:\n");
            Console.WriteLine("Current value: " + Config.GetString(Setting));
            Console.Write("> ");
            string? Result = Console.ReadLine();
            if(!string.IsNullOrEmpty(Result))
            {
                Config.SetString(Setting, Result);
            }
            else
            {
                Config.SetString(Setting, "");
            }
        }

        static void ChangeIntSetting(string Setting)
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Change your " + Setting + "\nPlease input an integer value:\n");
            Console.WriteLine("Current value: " + Config.GetInt(Setting));
            Console.Write("> ");
            string? Result = Console.ReadLine();
            if (!string.IsNullOrEmpty(Result) && Result.All(char.IsDigit))
            {
                Config.SetInt(Setting, int.Parse(Result));
            }
            else
            {
                Console.Clear();
                UiTools.WriteTitle();
                UiTools.ShowBanner("Must be an integer value!");
                UiTools.WriteControls(new List<string>() { "okay..." });
                ChangeIntSetting(Setting);
            }
        }

        static void BuildChoice()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Choose a build");

            string? PortOverride = Config.GetString("ServerPort");
            if (string.IsNullOrEmpty(PortOverride))
            {
                PortOverride = "16512";
            }

            if(Config.GetBool("AutoDetect"))
            {
                Api DetectServer = new Api("http://+:" + PortOverride + "/");
                if(Config.GetBool("ReCompat"))
                {
                    DetectServer.Listener.Prefixes.Add("http://+:2056/");
                }
                DetectServer.OnRequest += StartManager.CheckRestart;

                DetectServer.StartServer(new string[] { "LocalQuest.Controllers.BuildDetection" }, "Please start a build now", "BuildDetection");
                StartManager.StartSelected();
                return;
            }

            string Selection = UiTools.WriteControls(new List<string>()
            {
                "Go back",
                "2021",
                "late 2019+",
                "late 2018-mid 2019",
                "mid-late 2018",
                "mid 2018",
                "2017 (test)",
            });


            switch (Selection)
            {
                case "Go back":
                    Main(new string[0]);
                    return;
                case "2021":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - 2021";
                    Console.WriteLine("LocalQuest - 2021");
                    try
                    {
                        StartManager.GameVersion = "20210101";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
                case "late 2019+":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - late 2019+";
                    Console.WriteLine("LocalQuest - late 2019+");
                    try
                    {
                        StartManager.GameVersion = "20200403";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
                case "late 2018-mid 2019":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - late 2018";
                    Console.WriteLine("LocalQuest - late 2018");
                    try
                    {
                        StartManager.GameVersion = "20181108";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
                case "mid 2018":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - mid 2018";
                    Console.WriteLine("LocalQuest - mid 2018");
                    try
                    {
                        StartManager.GameVersion = "20180716";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
                case "2017 (test)":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - 2017??";
                    Console.WriteLine("LocalQuest - 2017??");
                    try
                    {
                        StartManager.GameVersion = "20170716";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
                case "mid-late 2018":
                    Console.Clear();
                    UiTools.WriteTitle();
                    Console.Title = "LocalQuest - 2018??";
                    Console.WriteLine("LocalQuest - 2018??");
                    try
                    {
                        StartManager.GameVersion = "20180827";
                        StartManager.StartSelected();
                    }
                    catch
                    {
                        StartFailure("Failed to start the server. [|X3]");
                    }
                    break;
            }
        }

        static void StartFailure(string Error)
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - startup failure");
            UiTools.ShowBanner(Error);
            Console.ForegroundColor = ConsoleColor.White;
            string Selection = UiTools.WriteControls(new List<string>()
            {
                "Go back",
            });

            switch (Selection)
            {
                case "Go back":
                    Main(new string[0]);
                    return;
            }
        }

        static void WhatNew()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - What's new");

            Console.WriteLine("Loading...");

            string? WhatNew = NetworkFiles.GetText("UpdateNotesV" + Setup.Version + ".txt").Result;
            Console.CursorTop -= 1;

            if (string.IsNullOrEmpty(WhatNew))
            {
                Console.WriteLine("Update notes not found!");
            }
            else
            {
                Console.WriteLine(WhatNew);
            }

            Console.WriteLine();

            string Selection = UiTools.WriteControls(new List<string>()
            {
                "Go back",
            });

            switch (Selection)
            {
                case "Go back":
                    Main(new string[0]);
                    return;
            }
        }
    }
}
