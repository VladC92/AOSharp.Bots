
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;
using System;
using System.Collections.Generic;
using DynelManager = AOSharp.Core.DynelManager;
using Vector3 = AOSharp.Common.GameData.Vector3;
using System.Linq;
using System.IO;

namespace Desu
{

    public class Search : AOPluginEntry
    {
        private string targetFile;

        private Dictionary<Profession, Vector3> ProfessionColors = new Dictionary<Profession, Vector3>
        {
            { Profession.Doctor , DebuggingColor.Red} ,
            { Profession.Trader , DebuggingColor.LightBlue} ,
            { Profession.Engineer , DebuggingColor.Green} ,
            { Profession.NanoTechnician , DebuggingColor.White} ,
            { Profession.Agent , DebuggingColor.Yellow} ,
            { Profession.MartialArtist , DebuggingColor.Purple} ,
            { Profession.Adventurer , DebuggingColor.White} ,
            { Profession.Enforcer , DebuggingColor.White} ,
            { Profession.Soldier , DebuggingColor.LightBlue} ,
            { Profession.Shade , DebuggingColor.White} ,
            { Profession.Keeper , DebuggingColor.White} ,
            { Profession.Bureaucrat , DebuggingColor.White} ,
            { Profession.Metaphysicist , DebuggingColor.White} ,
        };

        private List<string> _playersToHighlight = new List<string>();
        public override void Run(string pluginDir)
        {
            try
            {
                string targetPath;
                targetPath = Environment.GetEnvironmentVariable(@"C:\Users\Dj_vl\OneDrive\Desktop\TARGET_PATH");
                if (targetPath == null)
                    targetPath = Directory.GetCurrentDirectory();

                targetFile = targetPath + "\\" + "targets.txt";


                Chat.WriteLine("Search Loaded!", ChatColor.LightBlue);
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void PrintAssistCommandUsage(ChatWindow chatWindow)
        {
            string help = "Usage:\n" +
                "/search [name] -- name is optional\n" +
                "/target [name] -- name is optional\n";
            //"/stop ";    

            chatWindow.WriteLine(help, ChatColor.LightBlue);
        }

        public Search()
        {


            Game.OnUpdate += OnUpdate;
            Game.TeleportEnded += OnZoned;
            Chat.RegisterCommand("search", SearchPlayers);
            Chat.RegisterCommand("target", TargetPlayers);
            // Chat.RegisterCommand("stop", StopAssist);


        }

        (Profession, bool) GetProf(string name)
        {
            bool isProf = false;
            Profession prof;
            switch (name)
            {
                case "doc":
                case "doctor":
                case "doctors":
                case "docs":
                    isProf = true;
                    prof = Profession.Doctor;

                    break;
                case "crats":
                case "crat":
                case "bureaucrat":
                case "bureaucrats":
                    isProf = true;
                    prof = Profession.Bureaucrat;

                    break;
                case "sol":
                case "sols":
                case "soldier":
                case "soldiers":
                    isProf = true;
                    prof = Profession.Soldier;

                    break;
                case "trad":
                case "trads":
                case "traders":
                case "trader":
                    isProf = true;
                    prof = Profession.Trader;

                    break;
                case "agent":
                case "agents":
                    isProf = true;
                    prof = Profession.Agent;

                    break;
                case "nt":
                case "nts":
                    isProf = true;
                    prof = Profession.NanoTechnician;
                    break;
                case "mp":
                case "mps":
                    isProf = true;
                    prof = Profession.Metaphysicist;
                    break;
                case "engi":
                case "engis":
                case "engs":
                case "eng":
                case "engineers":
                case "engineer":
                    isProf = true;
                    prof = Profession.Engineer;

                    break;
                case "adv":
                case "advi":
                case "advis":
                    isProf = true;
                    prof = Profession.Adventurer;

                    break;
                case "enf":
                case "enfs":
                case "enfos":
                case "enforcer":
                case "enforcers":
                    isProf = true;
                    prof = Profession.Enforcer;
                    break;
                case "fix":
                case "fixers":
                case "fixer":
                    isProf = true;
                    prof = Profession.Fixer;
                    break;
                case "keep":
                case "keeper":
                case "keepers":
                    isProf = true;
                    prof = Profession.Keeper;
                    break;
                case "shade":
                case "shades":
                    isProf = true;
                    prof = Profession.Shade;
                    break;
                default:
                    isProf = false;
                    prof = Profession.Unknown;
                    break;


            }
            return (prof, isProf);
        }
        private void TargetPlayers(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {
                _playersToHighlight.Clear();
                if (param.Length < 1)
                {
                    PrintAssistCommandUsage(chatWindow);

                    if (File.Exists(targetFile))
                    {
                        string[] enemiesList = File.ReadAllLines(targetFile);

                        foreach (string enemy in enemiesList)
                        {
                            foreach (SimpleChar p in DynelManager.Players)
                            {
                                if (p.Name == enemy)
                                {
                                    _playersToHighlight.Add(enemy);
                                    Targeting.SetTarget(p);
                                    return;
                                }
                            }
                        }
                    }
                    return;
                }
                else // we are looking for someone or some prof
                {
                    string name = param[0].ToLower();

                    if (name == DynelManager.LocalPlayer.Name.ToLower())
                    {
                        Chat.WriteLine($"That's yourself N00b!", ChatColor.DarkPink);
                        return;
                    }
                    bool isProf;
                    Profession prof;
                    
                    (prof, isProf) = GetProf(name);

                    foreach (SimpleChar p in DynelManager.Players)
                    {
                        if (isProf == true)
                        {
                            if (p.Profession == prof && p.Side != Side.Clan && p.Level > 218)
                            {
                                Chat.WriteLine($"Found : " + p.Name, ChatColor.Gold);
                                _playersToHighlight.Add(p.Name);
                                Targeting.SetTarget(p);

                                return;
                            }

                        }
                        else
                        {

                            if (p.Name.ToLower().Contains(name))
                            {
                                _playersToHighlight.Add(p.Name);
                                Targeting.SetTarget(p);

                                return;
                            }
                        }
                    }
                    if (_playersToHighlight.Count == 0)
                        Chat.WriteLine($"Player / Profession {param[0]} not found", ChatColor.Yellow);
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
        private void SearchPlayers(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {
                _playersToHighlight.Clear();

                if (param.Length < 1)
                {
                    PrintAssistCommandUsage(chatWindow);

                    if (File.Exists(targetFile))
                    {
                        string[] enemiesList = File.ReadAllLines(targetFile);

                        foreach (string enemy in enemiesList)
                        {
                            foreach (SimpleChar p in DynelManager.Players)
                            {
                                if (p.Name == enemy)
                                {
                                    string profession = "Undefined";
                                    if (p.Profession != (Profession) 255)
                                        profession = p.Profession.ToString();
                                    Chat.WriteLine($"Enemy: " + p.Name + " profession: " + profession, ChatColor.Gold);
                                    _playersToHighlight.Add(enemy);
                                    break;
                                }
                            }
                        }
                    }
                    return;
                }
                else // we are looking for someone or some prof
                {
                    string name = param[0].ToLower();

                    if (name == DynelManager.LocalPlayer.Name.ToLower())
                    {
                        Chat.WriteLine($"That's yourself N00b!", ChatColor.DarkPink);
                        return;
                    }
                    bool isProf;
                    Profession prof;

                    (prof, isProf) = GetProf(name);

                    if (prof != Profession.Unknown)
                        Chat.WriteLine($"Searching for {prof.ToString()}...", ChatColor.DarkPink);
                    foreach (SimpleChar p in DynelManager.Players)
                    {
                        if (isProf == true)
                        {
                            if (p.Profession == prof && p.Side != Side.Clan && p.Level > 218)
                            {
                                Chat.WriteLine($"Found : " + p.Name, ChatColor.Gold);
                                _playersToHighlight.Add(p.Name);
                            }

                        }
                        else
                        {

                            if (p.Name.ToLower().Contains(name))
                            {
                                _playersToHighlight.Add(p.Name);
                                Chat.WriteLine("");
                                Chat.WriteLine($"Name: {p.Name}", ChatColor.White);
                                Chat.WriteLine($"Profession: {p.Profession}", ChatColor.White);
                                Chat.WriteLine($"Side: {p.Side}", ChatColor.White);
                                Chat.WriteLine($"Level: {p.Level}", ChatColor.White);
                                Chat.WriteLine($"Health: {p.Health}", ChatColor.White);
                                Chat.WriteLine($"Nano: {p.Nano}", ChatColor.White);
                                Chat.WriteLine($"Location: {p.Position}", ChatColor.White);
                                Chat.WriteLine("");
                                return;
                            }
                        }
                    }
                    if (_playersToHighlight.Count == 0)
                        Chat.WriteLine($"Player / Profession {param[0]} not found", ChatColor.Yellow);
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void DrawPlayer(SimpleChar player)
        {
            try
            {
                if (player != null)
                {
              
                    Debug.DrawSphere(player.Position, 1,DebuggingColor.Red);
                    Debug.DrawLine(DynelManager.LocalPlayer.Position, player.Position, DebuggingColor.Red);

                    if (player.FightingTarget != null && player.FightingTarget.IsInLineOfSight)
                    {
                        Debug.DrawSphere(player.FightingTarget.Position, 1, DebuggingColor.Green);
                        Debug.DrawLine(DynelManager.LocalPlayer.Position, player.FightingTarget.Position, DebuggingColor.Green);

                    }
                }
                else
                {
                    return;
                }
            }
           
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
        private void DrawFoundPlayers()
        {
            try
            {
                foreach (string playerName in _playersToHighlight)
                {

                    foreach (SimpleChar p in DynelManager.Players)
                    {
                        if (p.Name == playerName)
                        {
                            DrawPlayer(p);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
        private void OnZoned(object s, EventArgs e)
        {
            _playersToHighlight.Clear();
            
        }
        private void OnUpdate(object sender, float e)
        {
            DrawFoundPlayers();
        }
    }
}