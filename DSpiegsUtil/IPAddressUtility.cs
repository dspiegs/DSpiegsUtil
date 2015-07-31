using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil
{
    public class IPAddressUtility
    {
        public static BigInteger IPAddressToBigInteger(string ipAddress)
        {
            return IPAddressToBigInteger(IPAddress.Parse(ipAddress));
        }

        public static BigInteger IPAddressToBigInteger(IPAddress ipAddress)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException("ipAddress");
            }
            List<Byte> ipFormat = ipAddress.GetAddressBytes().ToList();
            ipFormat.Reverse();
            ipFormat.Add(0);
            return new BigInteger(ipFormat.ToArray());
        }
    }
}
