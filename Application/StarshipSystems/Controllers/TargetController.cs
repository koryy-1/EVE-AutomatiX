using Application.ClientWindow;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.StarshipSystems.Controllers
{
    public class TargetController
    {
        private Client _client;
        private BotBehavior _botBehavior;
        private Distance _weaponsRange;

        public TargetController(Client client, BotBehavior botBehavior, int weaponsRange)
        {
            _client = client;
            _botBehavior = botBehavior;
            _weaponsRange = new Distance();
            _weaponsRange.value = weaponsRange;
            _weaponsRange.measure = "km";
        }
        public bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.TargetControllerEnabled;
        }

        public void Work()
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
            if (!AttackingTargetIsAvailable())
            {
                ChangeToNearestTarget();
            }
        }

        private bool AttackingTargetIsAvailable()
        {
            //var TargetsInBar = TB.GetInfo();

            //if (Finder.GetEnemies()
            //    .Exists(item => item.AimOnTargetLocked && (Finder.Km2m(item.Distance) < Finder.Km2m(_weaponsRange))))
            //{
            //    return true;
            //}
            //return false;
            throw new NotImplementedException();
        }

        private void ChangeToNearestTarget()
        {
            //var target = Finder.GetEnemies()
            //    .Find(item => Finder.Km2m(item.Distance) < Finder.Km2m(_weaponsRange));

            //if (target == null)
            //    return;

            //_client.Emulators.ClickLB(target.Pos);
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
                .FindAll(item => _client.Parser.OV.GetColorInfo(item.Color) == "red");
            LockTargets(EnemyList);
        }

        private bool ExtraTargetsLocked()
        {
            var extraTarget = _client.Parser.OV.GetInfo()
                .Find(item => item.TargetLocked && _client.Parser.OV.GetColorInfo(item.Color) != "red");
            if (extraTarget != null)
            {
                return true;
            }
            return false;
        }

        private void UnlockExtraTargets()
        {
            var extraTargets = _client.Parser.OV.GetInfo()
                .FindAll(item => item.TargetLocked && _client.Parser.OV.GetColorInfo(item.Color) != "red");
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
