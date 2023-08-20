using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Parsers
{
    public static class PS
    {
        public static List<ProbeScanItem> GetInfo(ClientParams clientProcess)
        {
            throw new NotImplementedException();
        }

        public static List<ProbeScanItem> FindAnomalies(List<string> AnomalyNames) // todo: Parsers second method with param
        {
            return GetInfo(ClientProcess clientProcess).FindAll(
                (anomaly) =>
                {
                    foreach (var anomalyName in AnomalyNames)
                    {
                        return anomaly.Name.Contains(anomalyName);
                    }
                    return false;
                }
                );
        }
    }
}
