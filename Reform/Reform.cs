using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Core.Movement;
using AOSharp.Common.GameData;
using System.IO;
using AOSharp.Core.GameData;
using AOSharp.Core.UI.Options;
using AOSharp.Pathfinding;
using System.Data;
using AOSharp.Core.IPC;
using System.Collections.Concurrent;

namespace Reform
{
    public class Reform : IAOPluginEntry
    {
        private StateMachine _stateMachine;
        public static NavMeshMovementController NavMeshMovementController { get; private set; }
        public static IPCChannel IPCChannel { get; private set; }
        public static Config Config { get; private set; }
        public static GlobalSettings ActiveGlobalSettings;
        public static bool Running = false;
        public static Identity Leader = Identity.None;
        public static bool IsLeader = false;
        private List<Identity> foundPlayers = new List<Identity>();
        private Identity[] _teamCache;


        public void Run(string pluginDir)
        {
            Chat.WriteLine("Reform Loaded!");

            try
            {
                _stateMachine = new StateMachine(new IdleState());

                Chat.RegisterCommand("reform", ReformCommand);
                Chat.RegisterCommand("disband", DisbandCommand);
                Chat.RegisterCommand("inv", InviteCommand);


                Team.TeamRequest += OnTeamRequest;
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void Start()
        {
            if (IsLeader)
                _stateMachine.SetState(new ReformState());
        }


        private void OnUpdate(object s, float deltaTime)
        {
            if (Game.IsZoning)
                return;

            _stateMachine.Tick();

            if (!IsLeader)
                return;
        }

        private void OnTeamRequest(object s, TeamRequestEventArgs e)
        {
            e.Accept();
        }

        private void ReformCommand(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {

                if (!Team.IsLeader)
                {
                    chatWindow.WriteLine("Only the team leader can reform team", ChatColor.Yellow);
                    return;
                }
                IsLeader = true;
                Chat.WriteLine("reforming...");
                Start();
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
        private void DisbandCommand(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {

                if (!Team.IsLeader)
                {
                    chatWindow.WriteLine("Only the team leader can disband team", ChatColor.Yellow);
                    return;
                }
                IsLeader = true;
                Chat.WriteLine("Disbanding...");
                Team.Disband();
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }
        private void InvitePlayer(Identity player)
        {
            if (player == DynelManager.LocalPlayer.Identity || Team.Members.Any(j => j.Identity == player) || foundPlayers.Any(j => j == player))
                return;

            Team.Invite(player);
            Chat.WriteLine($"Inviting {player}");

            foundPlayers.Add(player);
        }
        private void InviteCommand(string command, string[] param, ChatWindow chatWindow)
        {
            try
            {

                foreach (SimpleChar player in DynelManager.Players)
                {

                    if (player.Name == "Ciobanprost" || player.Name == "Ciobanfaraoi" || player.Name == "Cioban" 

                        || player.Name == "Evolutzion" || player.Name == "Xcentric" || player.Name == "Izzgimerr")

                    {
                        foundPlayers.Add(player.Identity);
                        Team.Invite(player);
                    }

                }

            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

    }
}
