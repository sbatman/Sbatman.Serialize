using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Sbatman.Serialize.Auto
{
    public class TypeContract
    {
        private Type _ClassType;
        internal Type ClassType
        {
            get { return _ClassType; }
        }
       
        private List<Tuple<string, PropertyInfo>> _PropertyTypes;
        internal List<Tuple<String, PropertyInfo>> PropertyTypes
        {
            get { return _PropertyTypes; }
        }

        public static TypeContract ConstructTypeContract(Type t)
        {
            TypeContract returnContract = new TypeContract
            {
                _ClassType = t,
                _PropertyTypes = new List<Tuple<String, PropertyInfo>>()
            };

            List<PropertyInfo> properties = t.GetRuntimeProperties().ToList();
            foreach (PropertyInfo info in properties)
            {
                if (!info.CanRead || !info.CanWrite) continue;
                Packet.ParamTypes packedType = Packet.DetermineParamType(info.PropertyType);
                if (packedType == Packet.ParamTypes.UNKNOWN) continue;
                returnContract._PropertyTypes.Add(new Tuple<String, PropertyInfo>(info.Name, info));
            }

            return returnContract;
        }
    }
}
