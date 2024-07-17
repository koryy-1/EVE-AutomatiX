using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class NavPanel : InGameWnd
    {
        ClientParams _clientParams;

        public NavPanel(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public RoutePanel GetRoutePanel()
        {
            RoutePanel routePanel = new RoutePanel();
            var infoPanelRoute = UITreeReader.GetUITrees(_clientParams, "InfoPanelRoute", 7);
            if (infoPanelRoute == null)
                return null;

            routePanel.ButtonLoc = GetButtonLoc(infoPanelRoute);

            routePanel.Systems = GetSolarSystems(infoPanelRoute);

            return routePanel;
        }

        public LocationInfo GetLocation()
        {
            var InfoPanelLocationInfo = UITreeReader.GetUITrees(_clientParams, "InfoPanelLocationInfo", 7);
            if (InfoPanelLocationInfo == null)
                return null;

            var nearestLocationEntity = FindNodesByInterestName(InfoPanelLocationInfo, "nearestLocationInfo").FirstOrDefault();
            if (nearestLocationEntity == null)
                return null;

            var FullSystemName = nearestLocationEntity.dictEntriesOfInterest["_setText"].ToString()
                .Split('>')[1].Split()[0];

            LocationInfo LocationInfo = new LocationInfo()
            {
                Name = FullSystemName
            };

            return LocationInfo;
        }

        private List<Color> GetSolarSystems(UITreeNode infoPanelRoute)
        {
            var AutopilotDestIconNodes = FindNodesByObjectName(infoPanelRoute, "AutopilotDestinationIcon");
            if (!AutopilotDestIconNodes.Any())
                return new List<Color>();

            var systems = new List<Color>();

            foreach (var iconNode in AutopilotDestIconNodes)
            {
                Color systemColor = new Color();

                var colorNode = FindNodesByInterestKey(iconNode, "_color").FirstOrDefault();

                systemColor.Red = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["rPercent"]);
                systemColor.Green = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["gPercent"]);
                systemColor.Blue = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["bPercent"]);

                systems.Add(systemColor);
            }

            return systems;
        }

        private Point GetButtonLoc(UITreeNode InfoPanel)
        {
            var NeocomContainer = UITreeReader.GetUITrees(_clientParams, "NeocomContainer", 4);
            int LeftSidebar = ExtractIntValue(NeocomContainer, "_displayWidth");

            var infoPanelCoords = GetCoordsEntityOnScreen(InfoPanel);

            Point ButtonLoc = new Point()
            {
                x = LeftSidebar + 42,
                y = infoPanelCoords.y + 86
            };

            return ButtonLoc;
        }
    }
}
