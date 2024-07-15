using Application.ClientWindow;
using Domen.Models;
using EVE_AutomatiX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.StarshipSystems.Monitors
{
    public class TargetMonitor
    {
        private Client _client;
        private BotBehavior _botBehavior;

        public TargetMonitor(Client client, BotBehavior botBehavior)
        {
            _client = client;
            _botBehavior = botBehavior;
        }

        public bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.BattleModeEnabled;
        }

        public void Work()
        {
            while (true)
            {
                _botBehavior.CombatFlags.AllowToOpenFire = TargetsInAffectedArea();

                Thread.Sleep(300);
            }
        }

        private bool TargetsInAffectedArea()
        {
            // todo: AimTargetLockedOutOfWeaponRange()
            if (EnemiesOutOfWeaponRange())
            {
                return false;
            }
            if (AimOnTargetLockedOutOfWeaponRange())
            {
                _botBehavior.CombatFlags.TargetControllerEnabled = true;
                return false;
            }
            if (IsEmptyTargetBar())
            {
                _botBehavior.CombatFlags.TargetControllerEnabled = true;
                return false;
            }
            if (ExtraTargetsLocked())
            {
                _botBehavior.CombatFlags.TargetControllerEnabled = true;
                return false;
            }

            _botBehavior.CombatFlags.TargetControllerEnabled = false;
            return true;
        }

        private bool AimOnTargetLockedOutOfWeaponRange()
        {
            throw new NotImplementedException();
        }

        private bool ExtraTargetsLocked()
        {
            throw new NotImplementedException();
        }

        private bool IsEmptyTargetBar()
        {
            throw new NotImplementedException();
        }

        private bool EnemiesOutOfWeaponRange()
        {
            /// parse distance nearby enemy
            _botBehavior.CombatFlags.CloseDistanceToEnemy = true;
            return true;
        }
    }
}
