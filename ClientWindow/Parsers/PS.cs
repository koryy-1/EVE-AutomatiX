using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Parsers
{
    public class PS
    {
        ClientParams _clientParams;

        public PS(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }
        public List<ProbeScanItem> GetInfo()
        {
            throw new NotImplementedException();
        }

        public List<ProbeScanItem> FindAnomalies(List<string> AnomalyNames) // todo: Parsers second method with param
        {
            return GetInfo().FindAll(
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
