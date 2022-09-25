using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace BabyPanda.GH
{
    public class BabyPanda_GHInfo : GH_AssemblyInfo
    {
        public override string Name => "BabyPanda";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("27fe7bfa-cd31-44c1-903f-31f1815186c2");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        static BabyPanda_GHInfo()
        {
            var poop = TypeMaster.TypeFuctions;
            TypeMaster.RegisterTypesFromAssembly(new DataFrame_Goo().GetType().Assembly);
            var poopi = TypeMaster.TypeFuctions;

        }
    }
}