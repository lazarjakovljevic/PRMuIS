using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    [Serializable]
    public class NacinKomunikacije
    {
        private EndPoint clientEndPoint;
        private string algorithm;
        private string usedKey;
        public EndPoint ClientEndPoint { get => clientEndPoint; set => clientEndPoint = value; }
        public string Algorithm { get => algorithm; set => algorithm = value; }
        public string UsedKey { get => usedKey; set => usedKey = value; }

        public NacinKomunikacije(string algorithm, string usedKey)
        {
            ClientEndPoint = null;
            Algorithm = algorithm;
            UsedKey = usedKey;
        }
    }
}
