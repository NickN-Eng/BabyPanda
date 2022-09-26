using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using BabyPanda;
using Microsoft.Data.Analysis;

namespace BabyPanda.GH
{
    public class DataFrame_Goo : GH_Goo<DataFrame>
    {
        public DataFrame_Goo()
        {
        }

        public DataFrame_Goo(DataFrame internal_data) : base(internal_data)
        {
        }

        public DataFrame_Goo(GH_Goo<DataFrame> other) : base(other)
        {
        }

        public override bool IsValid => Value != null;

        public override string TypeName => "DataFrame";

        public override string TypeDescription => "A System.Data.Analysis DataFrame for tabular data.";

        public override bool CastFrom(object source)
        {
            if (source == null) return false;
            if(source is DataFrame df)
            {
                this.Value = df;
                return true;
            }
            return false;
        }

        public override bool CastTo<Q>(ref Q target)
        {
            bool flag;
            if (typeof(Q).IsAssignableFrom(typeof(DataFrame)))
            {
                if (m_value == null)
                {
                    return false;
                }
                else
                {
                    //object obj = m_value.Clone(); //Perform a deep copy of the DataFrame??
                    object obj = m_value;
                    target = (Q)obj;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new DataFrame_Goo(Value.Clone());
        }

        public override object ScriptVariable()
        {
            return m_value.Clone();
        }

        public override string ToString()
        {
            return "DataFrame {" + String.Join(",", m_value.Columns.Select(c => c.Name)) + "}";
        }

        public override bool Write(GH_IWriter writer)
        {
            var array = IO_Helpers.Serialise(m_value);
            writer.SetByteArray("DFBytes", array);
            return true;
        }
        public override bool Read(GH_IReader reader)
        {
            var array = reader.GetByteArray("DFBytes");
            m_value = IO_Helpers.DeserialiseDataFrame(array);
            return true;
        }
    }
}
