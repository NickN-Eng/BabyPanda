using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sd = System.Drawing;

namespace BabyPanda.WPF
{
    public static class RandomDataTableGenerator
    {
        #region Generate random data

        private static Random Random = new Random();
        private static Type[] _types = new Type[] { typeof(string), typeof(sd.Color), typeof(double), typeof(int), typeof(bool) };
        private static string[] _strings = new string[] { "hihi", "pop", "poop", "weeeeee" };
        private static sd.Color[] _colors = new sd.Color[] { sd.Color.AliceBlue, sd.Color.BurlyWood, sd.Color.DarkBlue, sd.Color.Red };

        private static double NullChance = 0.2;

        public static void AddSomeColumns(DataTable tab, int qty = 3)
        {
            for (int i = 0; i < qty; i++)
            {
                var type = GetRandomArrayItem(_types);
                var colNo = tab.Columns.Count;
                var colName = "Col" + colNo.ToString();
                var col = tab.Columns.Add(colName, type);
                for (int j = 0; j < tab.Rows.Count; j++)
                {
                    tab.Rows[j][colNo] = RandomValueByType(type);
                }
            }
        }

        public static void AddSomeRows(DataTable tab, int qty = 3)
        {
            int colCount = tab.Columns.Count;
            for (int i = 0; i < qty; i++)
            {
                var row = new object[colCount];
                for (int j = 0; j < colCount; j++)
                {
                    var type = tab.Columns[j].DataType;
                    row[j] = RandomValueByType(type);
                }
                tab.Rows.Add(row);
            }
        }

        private static object RandomValueByType(Type type)
        {
            if (Random.NextDouble() < NullChance)
            {
                return DBNull.Value;
            }

            if (typeof(string) == type) return GetRandomArrayItem(_strings);
            else if (typeof(sd.Color) == type) return GetRandomArrayItem(_colors);
            else if (typeof(double) == type) return Random.NextDouble();
            else if (typeof(int) == type) return Random.Next(0, 1001);
            else if (typeof(bool) == type) return Random.NextDouble() > 0.5;
            return null;

        }

        private static T GetRandomArrayItem<T>(T[] array) => array[Random.Next(0, array.Length)];
        #endregion
    }
}
