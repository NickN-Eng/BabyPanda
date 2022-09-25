using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Analysis;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BabyPanda
{

    public static class IO_Helpers
    {
        public static byte[] Serialise(DataFrame df)
        {
            var iodf = new IO_DataFrame(df);
            return SerializeWithBinaryFormatter(iodf).GetBuffer();
        }

        public static byte[] Serialise(DataFrameColumn col)
        {
            var iocol = IO_DataFrameColumn.FromDataFrameColumn(col);
            return SerializeWithBinaryFormatter(iocol).GetBuffer();
        }

        private static MemoryStream SerializeWithBinaryFormatter(object obj)
        {
            MemoryStream serializationStream = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(serializationStream, (object)obj);
            serializationStream.Close();
            return serializationStream;
        }

        public static DataFrame DeserialiseDataFrame(byte[] array)
        {
            object obj = Deserialise(array);
            if (obj != null && obj is IO_DataFrame iodf)
                return iodf.ToDataFrame();
            return null;
        }

        public static DataFrameColumn DeserialiseDataFrameColumn(byte[] array)
        {
            object obj = Deserialise(array);
            if (obj != null && obj is IO_DataFrameColumn iocol)
                return iocol.ToDataFrameColumn();
            return null;
        }

        private static object Deserialise(byte[] array)
        {
            MemoryStream serializationStream = new MemoryStream(array);
            var bf = new BinaryFormatter();
            object obj = bf.Deserialize(serializationStream);
            serializationStream.Close();
            return obj;
        }
    }


}
