﻿using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Parsers
{
    public class NavPanel : InGameWnd
    {
        ClientParams _clientParams;

        public NavPanel(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }
        public RoutePanel GetRoutePanelInfo()
        {
            RoutePanel RoutePanel = new RoutePanel();
            var InfoPanelRoute = UITreeReader.GetUITrees(_clientParams, "InfoPanelRoute", 7);
            if (InfoPanelRoute == null)
                return null;

            RoutePanel.ButtonLoc = GetButtonLoc(InfoPanelRoute);

            var AutopilotDestinationIcon = InfoPanelRoute.FindEntityOfString("AutopilotDestinationIcon");
            if (AutopilotDestinationIcon == null)
                return RoutePanel;

            RoutePanel.Systems = new List<Colors>();

            var AutopilotDestinationIconEntity = AutopilotDestinationIcon.handleEntity("AutopilotDestinationIcon");
            for (int j = 0; j < AutopilotDestinationIconEntity.children.Length; j++)
            {
                Colors SystemColors = new Colors();

                SystemColors.Red = Convert.ToInt32(AutopilotDestinationIconEntity.children[j].children[0].dictEntriesOfInterest["_color"]["rPercent"]);
                SystemColors.Green= Convert.ToInt32(AutopilotDestinationIconEntity.children[j].children[0].dictEntriesOfInterest["_color"]["gPercent"]);
                SystemColors.Blue = Convert.ToInt32(AutopilotDestinationIconEntity.children[j].children[0].dictEntriesOfInterest["_color"]["bPercent"]);

                RoutePanel.Systems.Add(SystemColors);
            }
            return RoutePanel;
        }
        public LocationInfo GetLocationInfo()
        {
            LocationInfo LocationInfo = new LocationInfo();
            var InfoPanelLocationInfo = UITreeReader.GetUITrees(_clientParams, "InfoPanelLocationInfo", 7);
            if (InfoPanelLocationInfo == null)
                return null;

            var nearestLocation = InfoPanelLocationInfo.FindEntityOfStringByDictEntriesOfInterest("_name", "nearestLocationInfo");
            if (nearestLocation == null)
                return null;

            var nearestLocationEntity = nearestLocation.handleEntityByDictEntriesOfInterest("_name", "nearestLocationInfo");

            var FullSystemName = nearestLocationEntity.children[Convert.ToInt32(nearestLocationEntity.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_setText"].ToString();

            LocationInfo.Name = FullSystemName.Split('>')[1].Split()[0];

            return LocationInfo;
        }
        public Point GetButtonLoc(UITreeNode InfoPanel)
        {
            Point ButtonLoc = new Point();

            var NeocomContainer = UITreeReader.GetUITrees(_clientParams, "NeocomContainer", 4);
            int LeftSidebar = 0;

            if (NeocomContainer.dictEntriesOfInterest["_displayWidth"] is Newtonsoft.Json.Linq.JObject)
                LeftSidebar = Convert.ToInt32(NeocomContainer.dictEntriesOfInterest["_displayWidth"]["int_low32"]);
            else
                LeftSidebar = Convert.ToInt32(NeocomContainer.dictEntriesOfInterest["_displayWidth"]);

            var (_, YInfoPanelRoute) = GetCoordsEntityOnScreen(InfoPanel);

            ButtonLoc.x = LeftSidebar + 42;
            ButtonLoc.y = YInfoPanelRoute + 86;

            return ButtonLoc;
        }
    }
}
