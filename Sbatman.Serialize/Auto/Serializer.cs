using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sbatman.Serialize.Auto
{
    public class Serializer
    {
        readonly List<TypeContract> _CachedTypeContracts = new List<TypeContract>();

        public Serializer(IEnumerable<Type> typeToBeAwareOf = null)
        {
            if (typeToBeAwareOf == null) return;
            foreach (Type type in typeToBeAwareOf) AcceptType(type);
        }

        public Packet Serialize(Object o)
        {
            Packet returnPacket = new Packet(UInt16.MaxValue - 67);

            List<Tuple<Guid, Object>> objectsToSerialize = new List<Tuple<Guid, Object>> { new Tuple<Guid, Object>(Guid.NewGuid(), o) };
            Dictionary<Guid, Object> objectsSerialized = new Dictionary<Guid, Object>();

            while (objectsToSerialize.Count > 0)
            {
                (Guid item1, Object item2) = objectsToSerialize[objectsToSerialize.Count - 1];
                objectsToSerialize.RemoveAt(objectsToSerialize.Count - 1);
                if (item2 == null) continue;

                if (objectsSerialized.ContainsValue(item2))
                {
                    // returnPacket.Add(objectsSerialized.FirstOrDefault(a => a.Value == objectBeingSerialized.Item2).Key);
                    continue;
                }

                objectsSerialized.Add(item1, item2);

                TypeContract typeContract = GetTypeContract(item2.GetType());

                returnPacket.Add(item1);
                returnPacket.Add(typeContract.UID);
                foreach (Tuple<String, PropertyInfo, Packet.ParamTypes> propertyType in typeContract.PropertyTypes)
                {
                    if (propertyType.Item3 == Packet.ParamTypes.AUTOPACK_REFERENCE)
                    {
                        Guid guid = Guid.NewGuid();
                        Object deserializationTarget = propertyType.Item2.GetValue(item2);
                        if (deserializationTarget != null) objectsToSerialize.Add(new Tuple<Guid, Object>(guid, deserializationTarget));
                        returnPacket.AddObject(guid);
                    }
                    else
                    {
                        returnPacket.AddObject(propertyType.Item2.GetValue(item2));
                    }
                }
            }
            return returnPacket;
        }

        public void AcceptType(Type t)
        {
            if (t.GetTypeInfo().GetCustomAttribute(typeof(AutoPacketable)) != null)
            {
                GetTypeContract(t);
            }
            else
            {
                throw new ArgumentException("Type must have the AutoPacketable attribute", nameof(t));
            }
        }

        private TypeContract GetTypeContract(Type t)
        {
            TypeContract typeContract = _CachedTypeContracts.FirstOrDefault(a => a.ClassType == t);
            if (typeContract != null) return typeContract;
            typeContract = TypeContract.ConstructTypeContract(t);
            _CachedTypeContracts.Add(typeContract);
            return typeContract;
        }

        public Object Deserialize(Packet p)
        {
            Object[] dataObjects = p.GetObjects();
            List<Tuple<Guid, Object>> deserializedObjects = new List<Tuple<Guid, Object>>();
            List<Tuple<Object, Guid, PropertyInfo>> pendingReferences = new List<Tuple<Object, Guid, PropertyInfo>>();

            Int32 dataPos = 0;

            while (dataPos < dataObjects.Length)
            {
                Guid objectGuid = (Guid)dataObjects[dataPos++];
                String typeString = (String)dataObjects[dataPos++];
                TypeContract typeContract = _CachedTypeContracts.FirstOrDefault(a => a.UID == typeString);
                Debug.Assert(typeContract != null, nameof(typeContract) + " != null");
                Type objectType = typeContract.ClassType;

                Object builtObject = Activator.CreateInstance(objectType);

                foreach ((String _, PropertyInfo item2, Packet.ParamTypes item3) in typeContract.PropertyTypes)
                {
                    if (item3 == Packet.ParamTypes.AUTOPACK_REFERENCE)
                    {
                        pendingReferences.Add(new Tuple<Object, Guid, PropertyInfo>(builtObject, (Guid)dataObjects[dataPos++], item2));
                    }
                    else
                    {
                        item2.SetValue(builtObject, dataObjects[dataPos++]);
                    }
                }
                deserializedObjects.Add(new Tuple<Guid, Object>(objectGuid, builtObject));
            }

            foreach (Tuple<Object, Guid, PropertyInfo> reference in pendingReferences)
            {
                Tuple<Guid, Object> linkedClass = deserializedObjects.FirstOrDefault(a => a.Item1.Equals(reference.Item2));
                if (linkedClass != null) reference.Item3.SetValue(reference.Item1, linkedClass.Item2);
            }
            return deserializedObjects[0].Item2;
        }

    }
}
