using LocalQuest.Models.Mid2018;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalQuest
{
    internal static class Setup
    {
        public static float Version = .3f;
        public static bool Beta = true;
        public static void Defaults()
        {
            Console.Clear();
            UiTools.WriteTitle();
            UiTools.ShowBanner("Setting up...");
            string NewUsername = AccountManager.CreateName();
            CreateDefault("Username", NewUsername + new Random().Next(0, 9999));
            CreateDefault("DisplayName", NewUsername);
            CreateDefault("AccountId", new Random().Next().ToString());

            if(FileManager.GetBytes("Images/DefaultPFP.png").Length == 0)
            {
                FileManager.WriteBytes("Images/DefaultPFP.png", Convert.FromBase64String(AccountManager.DefaultPFP));
            }

            if (FileManager.GetBytes("Images/Spooky.png").Length == 0)
            {
                FileManager.WriteBytes("Images/Spooky.png", Convert.FromBase64String(AccountManager.SpookyQuesterImg));
            }

            if (FileManager.GetBytes("Images/Mail.png").Length == 0)
            {
                FileManager.WriteBytes("Images/Mail.png", Convert.FromBase64String(AccountManager.MailImg));
            }

            if (FileManager.GetBytes("Images/A.png").Length == 0)
            {
                FileManager.WriteBytes("Images/A.png", Convert.FromBase64String(AccountManager.AImg));
            }

            List<string> Defaults = new List<string>()
            {
                "DefaultPFP",
                "Spooky",
                "Mail",
                "A"
            };

            CreateDefault("PFP", Defaults[new Random().Next(0,Defaults.Count)]);

            Console.Clear();
            if (FileManager.GetJSON<PlayerAvatar>("Avatar/LocalAvatar") == null)
            {
                FileManager.WriteJSON("Avatar/LocalAvatar", new PlayerAvatar()
                {
                    SkinColor = "",
                    OutfitSelections = "",
                    FaceFeatures = "",
                    HairColor = ""
                });
            }
            if (FileManager.GetJSON<List<PlayerSetting>>("PlayerData/Settings") == null)
            {
                FileManager.WriteJSON("PlayerData/Settings", new List<PlayerSetting>());
            }
        }

        public static void Tutorial()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Tutorial\n");
            Console.WriteLine("Welcome to LocalQuest, a localhost rec room custom server. \nBefore you start, let's set up your profile! [|=3]");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();

            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(AppData + "\\RebornRec\\"))
            {
                ReSetup();
            }
            else
            {
                ProfileSetup();
            }
        }

        static void ProfileSetup()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Tutorial");
            UiTools.ShowBanner("hint: use [UP ARROW] and [DOWN ARROW] to navigate.");
            Console.WriteLine("Your default @username is @" + Config.GetString("Username"));
            Console.WriteLine("and your default DisplayName is " + Config.GetString("DisplayName"));
            Console.WriteLine("\nwould you like to change these?");
            List<string> Options = new List<string>()
            {
                "Yeah!",
                "Nope!"
            };
            string Result = UiTools.WriteControls(Options);
            switch(Result)
            {
                case "Yeah!":
                    ChangeStringSetting("Username");
                    ChangeStringSetting("DisplayName");
                    SteamIdSetup();
                    Config.SetBool("FirstRun", true);
                    break;
                case "Nope!":
                    SteamIdSetup();
                    Config.SetBool("FirstRun", true);
                    return;
            }
        }

        static void SteamIdSetup()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Setup\nPlease enter your Steam ID (required for 2021 auth):\n");
            Console.WriteLine("You can find it at steamcommunity.com/id/me (the number in the URL)");
            Console.Write("> ");
            string? Result = Console.ReadLine();
            if (!string.IsNullOrEmpty(Result))
            {
                Config.SetString("SteamId", Result);
            }
        }

        static void ChangeStringSetting(string Setting)
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Change your " + Setting + "\nPlease input a value:\n");
            Console.WriteLine("Current value: " + Config.GetString(Setting));
            Console.Write("> ");
            string? Result = Console.ReadLine();
            if (!string.IsNullOrEmpty(Result))
            {
                Config.SetString(Setting, Result);
            }
        }

        public static void CreateDefault(string Name, string Value)
        {
            if(string.IsNullOrEmpty(Config.GetString(Name)))
            {
                Config.SetString(Name, Value);
            }
        }

        public static void ReSetup()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Welcome!\n");
            Console.WriteLine("It seems like you've used RebornRec before! Would you like to enable RebornRec compatibility mode?");
            UiTools.ShowBanner("This will make this server work with builds modded for RebornRec, but will break compatibility with builds modded to work with EpicQuest. (this CAN be changed later!)", ConsoleColor.Yellow);

            List<string> Settings = new List<string>()
            {
                "Yeah!",
                "Nope!"
            };

            string Response = UiTools.WriteControls(Settings);

            switch(Response)
            {
                case "Yeah!":
                    Config.SetBool("ReCompat", true);
                    Config.SetString("ServerPort", "2059");
                    ReSave();
                    break;
                case "Nope!":
                    ReSave();
                    break;
                default:
                    ReSetup();
                    break;
            }
        }

        public static void ReSave()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Import?\n");
            Console.WriteLine("Would you like to import your save data from RebornRec?");
            UiTools.ShowBanner("This WILL overwrite your LocalQuest SaveData, if you already have any.", ConsoleColor.Yellow);

            List<string> Settings = new List<string>()
            {
                "Yeah!",
                "Nope!"
            };

            string Response = UiTools.WriteControls(Settings);

            switch (Response)
            {
                case "Yeah!":
                    ImportData();
                    break;
                case "Nope!":
                    ProfileSetup();
                    break;
                default:
                    ReSetup();
                    break;
            }
        }

        public static void ImportData()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Data Import - RebornRec\n");
            UiTools.ShowBanner("Please drag and drop your RebornRec \"SaveData\" folder into this console window.", ConsoleColor.Blue);
            Console.WriteLine("You can also press [ENTER] to get out");

            List<string> Completes = new List<string>();
            List<string> Failures = new List<string>();

            string S = "";
            bool Dropped = false;
            bool Failure = false;
            while (!Dropped)
            {
                ConsoleKeyInfo Info = Console.ReadKey();
                char c = Info.KeyChar;
                S += c;
                Console.CursorLeft = 0;
                Console.Write(" ");
                Console.CursorLeft = 0;
                if (S.EndsWith("SaveData"))
                {
                    Dropped = true;
                }
                else if (Info.Key == ConsoleKey.Enter)
                {
                    Dropped = true;
                    Failure = true;
                }
            }
            if(Failure)
            {
                ImportComplete(new List<string>(), new List<string>() { "Invalid folder." });
                return;
            }
            string Path = S;
            if(!Path.EndsWith("\\") && !Path.EndsWith("/"))
            {
                Path += "/";
            }
            
            Console.WriteLine("Importing Username...");
            if(File.Exists(Path + "Profile/username.txt"))
            {
                string Username = File.ReadAllText(Path + "Profile/username.txt");
                if (WordFilter.IsPure(Username))
                {
                    Config.SetString("Username", Username);
                    Config.SetString("DisplayName", Username);
                    Completes.Add("@Username: " + Config.GetString("Username"));
                    Completes.Add("DisplayName: " + Config.GetString("DisplayName"));
                }
                else
                {
                    Log.Error("Bad username!");
                    Failures.Add("Username failed word filter!");
                }
            }
            else
            {
                Log.Error("Username file was not found!");
                Failures.Add("Username file was not found!");
            }

            Console.WriteLine("Importing Cheer...");
            if (File.Exists(Path + "Profile/cheer.txt"))
            {
                try
                {
                    string Cheer = File.ReadAllText(Path + "Profile/cheer.txt");
                    if (!string.IsNullOrEmpty(Cheer))
                    {
                        Config.SetInt("CheerCategory", int.Parse(Cheer));
                        Completes.Add("Cheer category: " + (CheerType?)int.Parse(Cheer));
                    }
                    else
                    {
                        Config.SetInt("CheerCategory", null);
                        Completes.Add("Cheer category: none");
                    }
                }
                catch
                {
                    Log.Error("Failed to import cheer badge");
                    Failures.Add("Failed to import cheer badge");
                }
            }
            else
            {
                Log.Error("Cheer file was not found!");
                Failures.Add("Cheer file was not found!");
            }

            Console.WriteLine("Importing Level...");
            if (File.Exists(Path + "Profile/level.txt"))
            {
                try
                {
                    string Level = File.ReadAllText(Path + "Profile/level.txt");
                    if (!string.IsNullOrEmpty(Level))
                    {
                        Config.SetInt("Level", int.Parse(Level));
                        Completes.Add("Level: " + int.Parse(Level));
                    }
                    else
                    {
                        Failures.Add("Invalid level");
                    }
                }
                catch
                {
                    Log.Error("Failed to import level");
                    Failures.Add("Failed to import level");
                }
            }
            else
            {
                Log.Error("Level file was not found!");
                Failures.Add("Level file was not found!");
            }

            Console.WriteLine("Importing PFP...");
            if (File.Exists(Path + "profileimage.png"))
            {
                try
                {
                    byte[] PFP = File.ReadAllBytes(Path + "profileimage.png");
                    string NewPFP = "ImportPFP" + new Random().Next();
                    FileManager.WriteBytes("Images/" + NewPFP + ".png", PFP);
                    Config.SetString("PFP", NewPFP);
                    Completes.Add("New PFP: " + NewPFP);
                }
                catch
                {
                    Log.Error("Failed to import PFP!");
                    Failures.Add("Failed to import PFP!");
                }
            }
            else
            {
                Log.Error("PFP file was not found!");
                Failures.Add("PFP file was not found!");
            }

            Console.WriteLine("Importing avatar...");
            if (File.Exists(Path + "avatar.txt"))
            {
                try
                {
                    PlayerAvatar? Avatar = JsonSerializer.Deserialize<PlayerAvatar>(File.ReadAllText(Path + "avatar.txt"));
                    if(Avatar != null)
                    {
                        FileManager.WriteJSON<PlayerAvatar>("Avatar/LocalAvatar", Avatar);
                        Completes.Add("Imported avatar [|=3]");
                    }
                    else
                    {
                        Log.Error("invalid avatar!");
                        Failures.Add("invalid avatar!");
                    }
                }
                catch
                {
                    Log.Error("Failed to import avatar!");
                    Failures.Add("Failed to import avatar!");
                }
            }
            else
            {
                Log.Error("avatar file was not found!");
                Failures.Add("avatar file was not found!");
            }

            Console.WriteLine("Importing settings...");
            if (File.Exists(Path + "settings.txt"))
            {
                try
                {
                    List<PlayerSetting>? Settings = JsonSerializer.Deserialize<List<PlayerSetting>>(File.ReadAllText(Path + "settings.txt"));
                    if (Settings != null)
                    {
                        FileManager.WriteJSON<List<PlayerSetting>>("PlayerData/Settings", Settings);
                        Completes.Add("Imported " + Settings.Count + " settings [|=3]");
                    }
                    else
                    {
                        Log.Error("invalid settings!");
                        Failures.Add("invalid settings!");
                    }
                }
                catch
                {
                    Log.Error("Failed to import settings!");
                    Failures.Add("Failed to import settings!");
                }
            }
            else
            {
                Log.Error("settings file was not found!");
                Failures.Add("settings file was not found!");
            }

            int RoomSuccess = 0;
            int RoomFail = 0;
            string[] Rooms = Directory.GetDirectories(Path + "Rooms/");
            foreach(var Room in Rooms)
            {
                break;
                try
                {
                    if (File.Exists(Room + "RoomDetails.json"))
                    {
                        string Data = File.ReadAllText(Room + "RoomDetails.json");
                        Dictionary<string, object>? RoomData = JsonSerializer.Deserialize<Dictionary<string, object>>(Data);
                        if(RoomData == null)
                        {
                            RoomFail += 1;
                            continue;
                        }
                    }
                    RoomSuccess += 1;
                }
                catch
                {
                    RoomFail += 1;
                }
            }

            Completes.Add("Imported " + RoomSuccess + " rooms (coming soon)");
            if(RoomFail > 0)
            {
                Failures.Add("Failed to import " + RoomFail + " rooms (coming soon)");
            }

            Config.SetBool("FirstRun", true);
            ImportComplete(Completes, Failures);
        }

        public static void ImportOpenData()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Data Import - OpenRec\n");
            UiTools.ShowBanner("Please drag and drop your OpenRec \"SaveData\" folder into this console window.", ConsoleColor.Blue);
            Console.WriteLine("You can also press [ENTER] to get out");

            List<string> Completes = new List<string>();
            List<string> Failures = new List<string>();

            string S = "";
            bool Dropped = false;
            bool Failure = false;
            while (!Dropped)
            {
                ConsoleKeyInfo Info = Console.ReadKey();
                char c = Info.KeyChar;
                S += c;
                Console.CursorLeft = 0;
                Console.Write(" ");
                Console.CursorLeft = 0;
                if (S.EndsWith("SaveData"))
                {
                    Dropped = true;
                }
                else if (Info.Key == ConsoleKey.Enter)
                {
                    Dropped = true;
                    Failure = true;
                }
            }
            if (Failure)
            {
                ImportComplete(new List<string>(), new List<string>() { "Invalid folder." });
                return;
            }
            string Path = S;
            if (!Path.EndsWith("\\") && !Path.EndsWith("/"))
            {
                Path += "/";
            }

            Console.WriteLine("Importing Username...");
            if (File.Exists(Path + "Profile/username.txt"))
            {
                string Username = File.ReadAllText(Path + "Profile/username.txt");
                if (WordFilter.IsPure(Username))
                {
                    Config.SetString("Username", Username);
                    Config.SetString("DisplayName", Username);
                    Completes.Add("@Username: " + Config.GetString("Username"));
                    Completes.Add("DisplayName: " + Config.GetString("DisplayName"));
                }
                else
                {
                    Log.Error("Bad username!");
                    Failures.Add("Username failed word filter!");
                }
            }
            else
            {
                Log.Error("Username file was not found!");
                Failures.Add("Username file was not found!");
            }

            Console.WriteLine("Importing Level...");
            if (File.Exists(Path + "Profile/level.txt"))
            {
                try
                {
                    string Level = File.ReadAllText(Path + "Profile/level.txt");
                    if (!string.IsNullOrEmpty(Level))
                    {
                        Config.SetInt("Level", int.Parse(Level));
                        Completes.Add("Level: " + int.Parse(Level));
                    }
                    else
                    {
                        Failures.Add("Invalid level");
                    }
                }
                catch
                {
                    Log.Error("Failed to import level");
                    Failures.Add("Failed to import level");
                }
            }
            else
            {
                Log.Error("Level file was not found!");
                Failures.Add("Level file was not found!");
            }

            Console.WriteLine("Importing PFP...");
            if (File.Exists(Path + "profileimage.png"))
            {
                try
                {
                    byte[] PFP = File.ReadAllBytes(Path + "profileimage.png");
                    string NewPFP = "ImportPFP" + new Random().Next();
                    FileManager.WriteBytes("Images/" + NewPFP + ".png", PFP);
                    Config.SetString("PFP", NewPFP);
                    Completes.Add("New PFP: " + NewPFP);
                }
                catch
                {
                    Log.Error("Failed to import PFP!");
                    Failures.Add("Failed to import PFP!");
                }
            }
            else
            {
                Log.Error("PFP file was not found!");
                Failures.Add("PFP file was not found!");
            }

            Console.WriteLine("Importing avatar...");
            if (File.Exists(Path + "avatar.txt"))
            {
                try
                {
                    PlayerAvatar? Avatar = JsonSerializer.Deserialize<PlayerAvatar>(File.ReadAllText(Path + "avatar.txt"));
                    if (Avatar != null)
                    {
                        FileManager.WriteJSON<PlayerAvatar>("Avatar/LocalAvatar", Avatar);
                        Completes.Add("Imported avatar [|=3]");
                    }
                    else
                    {
                        Log.Error("invalid avatar!");
                        Failures.Add("invalid avatar!");
                    }
                }
                catch
                {
                    Log.Error("Failed to import avatar!");
                    Failures.Add("Failed to import avatar!");
                }
            }
            else
            {
                Log.Error("avatar file was not found!");
                Failures.Add("avatar file was not found!");
            }

            Console.WriteLine("Importing settings...");
            if (File.Exists(Path + "settings.txt"))
            {
                try
                {
                    List<PlayerSetting>? Settings = JsonSerializer.Deserialize<List<PlayerSetting>>(File.ReadAllText(Path + "settings.txt"));
                    if (Settings != null)
                    {
                        FileManager.WriteJSON<List<PlayerSetting>>("PlayerData/Settings", Settings);
                        Completes.Add("Imported " + Settings.Count + " settings [|=3]");
                    }
                    else
                    {
                        Log.Error("invalid settings!");
                        Failures.Add("invalid settings!");
                    }
                }
                catch
                {
                    Log.Error("Failed to import settings!");
                    Failures.Add("Failed to import settings!");
                }
            }
            else
            {
                Log.Error("settings file was not found!");
                Failures.Add("settings file was not found!");
            }

            Config.SetBool("FirstRun", true);
            ImportComplete(Completes, Failures);
        }

        public static void ImportModernProfile()
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Data Import - Modern\n");

            Console.WriteLine("Please input your @username from modern:");
            Console.Write("> ");
            string? S = Console.ReadLine();

            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Data Import - Modern - Importing...\n");

            List<string> Completes = new List<string>();
            List<string> Failures = new List<string>();

            if(string.IsNullOrEmpty(S))
            {
                Console.Clear();
                UiTools.WriteTitle();
                UiTools.ShowBanner("Must input a username!");
                UiTools.WriteControls(new List<string>() { "okay..." });
                return;
            }
            Console.WriteLine("Downloading profile...");
            Models.Modern.Profile? NewProfile = NetworkFiles.GetFullData<Models.Modern.Profile>("https://apim.rec.net/accounts/account?username=" + S).Result;
            if(NewProfile == null)
            {
                Console.Clear();
                UiTools.WriteTitle();
                UiTools.ShowBanner("Invalid profile!");
                UiTools.WriteControls(new List<string>() { "okay..." });
                return;
            }
            Config.SetString("Username", NewProfile.username);
            Completes.Add("Username: " + NewProfile.username);
            Config.SetString("DisplayName", NewProfile.displayName);
            Completes.Add("DisplayName: " + NewProfile.displayName);
            Console.WriteLine("Downloading PFP...");
            if (NewProfile.profileImage != null)
            {
                try
                {
                    byte[] PFP = NetworkFiles.GetFullData("https://img.rec.net/" + NewProfile.profileImage + "?cropSquare=1").Result;
                    string NewPFP = "ImportPFP" + new Random().Next();
                    FileManager.WriteBytes("Images/" + NewPFP + ".png", PFP);

                    Config.SetString("PFP", NewPFP);
                    Completes.Add("PFP: " + NewPFP);
                }
                catch
                {
                    Failures.Add("Failed to import PFP");
                    Log.Error("Failed to get profile image");
                }
            }
            else
            {
                Failures.Add("Profile didn't have a PFP");
            }

            Models.Modern.Bio? NewBio = NetworkFiles.GetFullData<Models.Modern.Bio>("https://apim.rec.net/accounts/account/" + NewProfile.accountId + "/bio").Result;

            if(NewBio != null)
            {
                if (!string.IsNullOrEmpty(NewBio.bio))
                {
                    Config.SetString("Bio", NewBio.bio);
                    Completes.Add("Bio: " + NewBio.bio);
                }
                else
                {
                    Failures.Add("Profile didn't have a bio [|X3]");
                }
            }
            else
            {
                Failures.Add("Failed to get bio [|X3]");
            }

            ImportComplete(Completes, Failures);
        }

        static void ImportComplete(List<string> Completes, List<string> Failures)
        {
            Console.Clear();
            UiTools.WriteTitle();
            Console.WriteLine("LocalQuest - Data Import - final!\n");
            foreach(var Complete in Completes)
            {
                Console.WriteLine(Complete);
            }
            foreach (var Failure in Failures)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failure - " + Failure);
            }
            Console.ForegroundColor = ConsoleColor.White;
            UiTools.ShowBanner("Successfully imported SaveData! Press [ENTER] to continue...", ConsoleColor.Green);
            UiTools.WriteControls(new List<string>() { "Continue" });
        }
    }
}
