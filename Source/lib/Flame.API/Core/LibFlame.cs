namespace FlameAPI.Configuration
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Newtonsoft.Json;
    using RC.Framework.Net.Arch;

    public class LibFlame
    {
        public List<string> keySpaces = new List<string>();
        public List<string> keys = new List<string>();

        public void AddKeySpace(string keyspace)
        {
            if (!keySpaces.Contains(keyspace))
                keySpaces.Add(keyspace);
        }
        public void AddKey(string key)
        {
            if (!keys.Contains(key))
                keys.Add(key);
        }

        

        public static T Decode<T>(byte[] array)
        {
            var arh = ArchManagedByte.InvokeReader(array);

            int mapHash = arh.rInt();
            int mapID = arh.rInt();
            DateTime dt = arh.rDateTime();
            string json = arh.rString();
            if (mapID != 50) throw new WrongMapLibException("Wrong mapID");

            return JsonConvert.DeserializeObject<T>(json);
        }
        public static byte[] Encode<T>(T t)
        {
            var arh = ArchManagedByte.InvokeWriter();

            int mapHash = new Random().Next();
            int mapID = 50;
            DateTime dt = DateTime.Now;
            string json = JsonConvert.SerializeObject(t);

            arh.wI(mapHash);
            arh.wI(mapID);
            arh.wDateTime(dt);
            arh.wSt(json);

            return arh.toArray();
        }
    }
}