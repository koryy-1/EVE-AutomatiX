using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_AutomatiX.Starship.Controllers
{
    public class TargetController : ThreadWrapper
    {
        private Client _client;
        private BotBehavior _botBehavior;
        private int _weaponsRange;

        public TargetController(Client client, BotBehavior botBehavior, int weaponsRange)
        {
            _client = client;
            _botBehavior = botBehavior;
            _weaponsRange = weaponsRange;
        }
        public override bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.TargetControllerEnabled;
        }

        public override void Work()
        {
            while (true)
            {
                EnsureTargetsLocked();

                EnsureExtraTargetsUnlocked();

                EnsureTargetIsAvailable();

                Thread.Sleep(300);
            }
        }

        private void EnsureTargetIsAvailable()
        {
            //var TargetsInBar = TB.GetInfo();
            var OverviewTargetsInfo = _client.Parser.OV.GetInfo().Find(item => item.AimOnTargetLocked);
            if (OverviewTargetsInfo == null)
                return;

            if (!AttackingTargetIsAvailable(OverviewTargetsInfo))
            {
                ChangeToNearestTarget();
            }
        }

        private bool AttackingTargetIsAvailable(OverviewItem OverviewTargetsInfo)
        {
            if (OverviewTargetsInfo.Distance.measure == "km" && 
                OverviewTargetsInfo.Distance.value < _weaponsRange || 
                OverviewTargetsInfo.Distance.measure == "m")
            {
                return true;
            }
            return false;
        }

        private void ChangeToNearestTarget()
        {
            OverviewItem Target = _client.Parser.OV.GetInfo().Find(
                item => item.Distance.measure == "km" &&
                item.Distance.value < _weaponsRange ||
                item.Distance.measure == "m");

            if (Target == null)
            {
                return;
            }

            Emulators.ClickLB(Target.Pos.x, Target.Pos.y);
            Console.WriteLine("target changed");
        }

        private void EnsureTargetsLocked()
        {
            if (!TargetsInProcessOfLocking())
            {
                LockEnemies();
            }
        }

        private void EnsureExtraTargetsUnlocked()
        {
            if (ExtraTargetsLocked())
            {
                UnlockExtraTargets();
            }
        }

        private bool TargetsInProcessOfLocking()
        {
            var spaceObject = _client.Parser.OV.GetInfo()
                .Find(item => item.TargetLocked);
            if (spaceObject != null)
            {
                return true;
            }
            return false;
        }

        private void LockEnemies()
        {
            var EnemyList = _client.Parser.OV.GetInfo()
                .FindAll(item => _client.Parser.OV.GetColorInfo(item.Colors) == "red");
            LockTargets(EnemyList);
        }

        private bool ExtraTargetsLocked()
        {
            var extraTarget = _client.Parser.OV.GetInfo()
                .Find(item => item.TargetLocked && _client.Parser.OV.GetColorInfo(item.Colors) != "red");
            if (extraTarget != null)
            {
                return true;
            }
            return false;
        }

        private void UnlockExtraTargets()
        {
            var extraTargets = _client.Parser.OV.GetInfo()
                .FindAll(item => item.TargetLocked && _client.Parser.OV.GetColorInfo(item.Colors) != "red");
            UnlockTargets(extraTargets);
        }

        private void LockTargets(List<OverviewItem> EnemyList)
        {
            throw new NotImplementedException();
        }

        private void UnlockTargets(List<OverviewItem> EnemyList)
        {
            throw new NotImplementedException();
        }
    }
}
