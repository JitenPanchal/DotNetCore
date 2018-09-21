using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static T ToXml<T>(this string content) where T : class
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            return serializer.ReadObject(stream) as T;
        }
    }
}
