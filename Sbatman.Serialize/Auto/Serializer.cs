using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sbatman.Serialize.Auto
{
    class Serializer
    {
        List<TypeContract> _CachedTypeContracts = new List<TypeContract>();

        public Packet Serialize(Object o, UInt16 packetType = 0)
        {
            Packet returnPacket = new Packet(packetType);
            Type objectType = o.GetType();
            TypeContract typeContract = _CachedTypeContracts.FirstOrDefault(a => a.ClassType == objectType);
            if (typeContract == null)
            {
                typeContract = TypeContract.ConstructTypeContract(objectType);
                _CachedTypeContracts.Add(typeContract);
            }
            foreach (Tuple<String, PropertyInfo> propertyType in typeContract.PropertyTypes)
            {
                returnPacket.Add(propertyType.Item2.GetValue(o));
            }
        }
    }
}
