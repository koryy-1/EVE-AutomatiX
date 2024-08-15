using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class TB : InGameWnd
    {
        ClientParams _clientParams;

        public TB(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public List<TargetInBar> GetInfo()
        {
            var TargetInBarEntry = UITreeReader.GetUITrees(_clientParams, "TargetInBar", 6, true);
            if (TargetInBarEntry == null)
                return new List<TargetInBar>();

            List<TargetInBar> TargetsInBar = new List<TargetInBar>();

            for (int i = 0; i < TargetInBarEntry.children.Length; i++)
            {
                TargetInBar Target = new TargetInBar();

                var targetInBarEntryCoords = GetCoordsEntityOnScreen(TargetInBarEntry.children[i]);

                Target.Pos.x = targetInBarEntryCoords.x + 55;
                Target.Pos.y = targetInBarEntryCoords.y + 55;

                var valueStr = TargetInBarEntry.children[i].children[1].children.Last().dictEntriesOfInterest["_setText"].ToString();
                int value;
                int.TryParse(string.Join("", valueStr.Where(c => char.IsDigit(c))), out value);

                Target.Distance.Value = value;

                Target.Distance.Measure = valueStr.Split().Last();

                Target.Name = TargetInBarEntry.children[i].children[1].children[0].dictEntriesOfInterest["_setText"].ToString();

                if (TargetInBarEntry.children[i].children[0].children[0].children[3].pythonObjectTypeName == "ActiveTargetOnBracket")
                {
                    Target.AimOnTargetLocked = true;
                }

                //effects
                if (TargetInBarEntry.children[i].children.Last().children[0].children != null
                    &&
                    TargetInBarEntry.children[i].children.Last().children[0].children[0].pythonObjectTypeName == "Weapon")
                {
                    Target.WeaponWorking = true;
                }

                TargetsInBar.Add(Target);
            }
            return TargetsInBar;
        }
    }
}
