using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reform
{
    public class IdleState : IState
    {
        public IState GetNextState()
        {
            if (!Reform.Running)
                return null;

            //We have no possible action if you aren't in a team.
            if(!DynelManager.LocalPlayer.IsInTeam())
                return null;


            return null;
        }

        public void OnStateEnter()
        {
            Chat.WriteLine("IdleState::OnStateEnter");
        }

        public void OnStateExit()
        {
            Chat.WriteLine("IdleState::OnStateExit");
        }

        public void Tick()
        {
        }
    }
}
