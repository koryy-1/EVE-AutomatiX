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
                return routePanel;

            routePanel.NextSystemInRoute = GetNextSystemInRoute(infoPanelRoute);

            routePanel.CurrentDestination = GetCurrentDestination(infoPanelRoute);

            routePanel.Systems = GetSolarSystems(infoPanelRoute);

            routePanel.ButtonLoc = GetButtonLoc(infoPanelRoute);

            return routePanel;
        }

        public LocationInfo GetLocation()
        {
            var InfoPanelLocationInfo = UITreeReader.GetUITrees(_clientParams, "InfoPanelLocationInfo", 7);
            if (InfoPanelLocationInfo == null)
                return new LocationInfo();

            var nearestLocationEntity = FindNodesByInterestName(InfoPanelLocationInfo, "nearestLocationInfo").FirstOrDefault();
            if (nearestLocationEntity == null)
                return new LocationInfo();

            // _name :  nearestLocationInfo
            // _name : headerLabelSystemName
            // todo: в структуре в _setText вместо Perimeter стоит какойто pythonObjectTypeName "Link"
            // cur system - perimeter
            // nearest location - niyabainen

            var FullSystemName = nearestLocationEntity.dictEntriesOfInterest["_setText"]?.ToString()
                .Split('>')[1].Split()[0];

            LocationInfo LocationInfo = new LocationInfo()
            {
                Name = FullSystemName
            };

            return LocationInfo;
        }

        private string GetNextSystemInRoute(UITreeNode infoPanelRoute)
        {
            var nextWaypointPanelNode = FindNodesByObjectName(infoPanelRoute, "NextWaypointPanel").FirstOrDefault();

            if (nextWaypointPanelNode is null)
                return null;

            var nodeWithNextSystem = FindNodesByObjectName(nextWaypointPanelNode, "EveLabelMedium").FirstOrDefault();

            var nextSystemInRoute = nodeWithNextSystem.dictEntriesOfInterest["_setText"].ToString()
                .Split("Next System in Route\">")[1]
                .Split("<")[0];

            return nextSystemInRoute;
        }

        private string GetCurrentDestination(UITreeNode infoPanelRoute)
        {
            var destWaypointPanelNode = FindNodesByObjectName(infoPanelRoute, "DestinationWaypointPanel").FirstOrDefault();

            if (destWaypointPanelNode is null)
                return null;

            var nodeWithCurrentDest = FindNodesByObjectName(destWaypointPanelNode, "EveLabelMedium").FirstOrDefault();

            var currentDestination = nodeWithCurrentDest.dictEntriesOfInterest["_setText"].ToString()
                .Split("Current Destination\">")[1]
                .Split("<")[0];

            return currentDestination;
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
