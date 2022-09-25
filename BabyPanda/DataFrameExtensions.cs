using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda
{
    /// <summary>
    /// A collection of helper methods to assist with updates to 
    /// dataframes through iterating row by row through lambda functions.
    /// </summary>
    public static class DataFrameExtensions
    {
        /// <summary>
        /// Iterate through a dataframe row by row, accessing data through the DfRowIterator by column name.
        /// New columns can be declared through the DfRowIterator, according to the type of the first object supplied.
        /// This function is less performant than working with strongly typed DataFrameColumns. 
        /// ... because a lot of casting and column lookups takes place.
        /// </summary>
        /// <param name="df">Dataframe to iterate through.</param>
        /// <param name="action">Action to be done on 1 row.</param>
        public static void ExecuteByRow(this DataFrame df, Action<DfRowIterator> action)
        {
            var iterator = new DfRowIterator(df);
            for (int i = 0; i < df.Rows.Count; i++)
            {
                action.Invoke(iterator);
                iterator.Row++;
            }
        }

        /// <summary>
        /// Iterate through 2 dataframes row by row, accessing data through the DfRowIterator by column name.
        /// For example, can be used to process data from one dataframe to another.
        /// The two dataframes must be of equal row length.
        /// New columns can be declared through the DfRowIterator, according to the type of the first object supplied.
        /// This function is less performant than working with strongly typed DataFrameColumns. 
        /// ... because a lot of casting and column lookups takes place.
        /// </summary>
        /// <param name="df1"></param>
        /// <param name="action">
        /// action_T1_DfRowIterator. The dataframe where the row data originates
        /// action_T2_DfRowIterator. The dataframe where the result will be saved.
        /// </param>
        /// <param name="df2">The second dataframe. A new one will be created if none is provided.</param>
        public static DataFrame ExecuteByRow(this DataFrame df1, 
            Action<DfRowIterator, DfRowIterator> action, 
            DataFrame df2 = null )
        {
            if (df2 == null) df2 = new DataFrame();

            var reader = new DfRowIterator(df1);
            var writer = new DfRowIterator(df2);
            for (int i = 0; i < df1.Rows.Count; i++)
            {
                action.Invoke(reader, writer);
                reader.Row++;
                writer.Row++;
            }
            return df2;
        }


    }

}
