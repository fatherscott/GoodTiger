using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    class ProtocolConverter
    {
        static Dictionary<ProtocolType, Type> _SystemType = new Dictionary<ProtocolType, Type>();
        static ProtocolConverter()
        {
            foreach (AssemblyName asmName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                Assembly.Load(asmName);

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(ClientProtocol).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var t in types)
            {
                var instance = Activator.CreateInstance(t) as ClientProtocol;
                _SystemType.Add(instance.Type, t);
            }
        }

        public static async Task FillBuffer(ClientProtocol obj, SocketBuffer buffer)
        {
            await using var ms = new MemoryStream(buffer.DataBuffer);
            using var writer = new StreamWriter(ms);

            buffer.Type = (int)obj.Type;
            var text = JsonConvert.SerializeObject(obj);

            writer.Write(text);
            writer.Flush();

            buffer.Length = (int)ms.Position;
            buffer.Worked = 0;
        }

        public static ClientProtocol StreamToClass(ProtocolType type, StreamReader reader)
        {
            if (!_SystemType.ContainsKey(type))
            {
                throw new Exception($"type not found {type.GetType().FullName}");
            }
            var text = reader.ReadToEnd();
            var obj = JsonConvert.DeserializeObject(text, _SystemType[type]);
            return obj as ClientProtocol;
        }
    }
}
