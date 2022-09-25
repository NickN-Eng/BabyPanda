//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Reflection;
//using Grasshopper;
//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Data;
//using Grasshopper.Kernel.Types;
//using Rhino.DocObjects;
//using Rhino.Geometry;

//namespace BabyPanda.GHReflectionComponents
//{
//    /// <summary>
//    /// WORK IN PROGRESS
//    /// </summary>
//    public abstract partial class ComponentTemplate : GH_Component
//    {
//        private Dictionary<Type, ComponentReflectionData> allReflectionDatas = new Dictionary<Type, ComponentReflectionData>();

//        private ComponentReflectionData reflectionData
//        {
//            get
//            {
//                //TODO: Put this into a static constructor which scans the assembly for types inheriting from Component template???

//                //if this type is already in the dictionary, get the reflection data, otherwise, parse the class once and for all
//                var typ = this.GetType();
//                if (allReflectionDatas.TryGetValue(typ, out var data))
//                    return data;
//                var newData = new ComponentReflectionData(typ);
//                allReflectionDatas[typ] = newData;
//                return newData;
//            }
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {           
//            foreach (var input in reflectionData.Inputs)
//            {
//                AddParam(pManager, input);
//            }
//        }



//        private void AddParam(GH_InputParamManager pManager, InputReflectionItem input)
//        {
//            string name = input.Name;
//            string nickname = input.InputAttribute.ShortName;
//            string description = input.InputAttribute.Description;
//            GH_ParamAccess access = input.ParamAccess;
//            var unitType = input.UnitType;

//            int paramIndex = -1;
//            object defaultValue = input.DefaultValue;

//            if(defaultValue == null)
//            {
//                if (unitType == typeof(double)) paramIndex = pManager.AddAngleParameter(name, nickname, description, access);
//                else if (unitType == typeof(Arc)) paramIndex = pManager.AddArcParameter(name, nickname, description, access);
//                else if (unitType == typeof(bool)) paramIndex = pManager.AddBooleanParameter(name, nickname, description, access);
//                else if (unitType == typeof(Box)) paramIndex = pManager.AddBoxParameter(name, nickname, description, access);
//                else if (unitType == typeof(Brep)) paramIndex = pManager.AddBrepParameter(name, nickname, description, access);
//                else if (unitType == typeof(Circle)) paramIndex = pManager.AddCircleParameter(name, nickname, description, access);
//                else if (unitType == typeof(System.Drawing.Color)) paramIndex = pManager.AddColourParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_ComplexNumber)) paramIndex = pManager.AddComplexNumberParameter(name, nickname, description, access);
//                else if (unitType == typeof(CultureInfo)) paramIndex = pManager.AddCultureParameter(name, nickname, description, access);
//                else if (unitType == typeof(Curve)) paramIndex = pManager.AddCurveParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_Field)) paramIndex = pManager.AddFieldParameter(name, nickname, description, access);
//                else if (unitType == typeof(GeometryBase)) paramIndex = pManager.AddGeometryParameter(name, nickname, description, access);
//                else if (unitType == typeof(Group)) paramIndex = pManager.AddGroupParameter(name, nickname, description, access);
//                else if (unitType == typeof(int)) paramIndex = pManager.AddIntegerParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_Interval2D)) paramIndex = pManager.AddInterval2DParameter(name, nickname, description, access);
//                else if (unitType == typeof(Interval)) paramIndex = pManager.AddIntervalParameter(name, nickname, description, access);
//                else if (unitType == typeof(Line)) paramIndex = pManager.AddLineParameter(name, nickname, description, access);
//                else if (unitType == typeof(Matrix)) paramIndex = pManager.AddMatrixParameter(name, nickname, description, access);
//                else if (unitType == typeof(MeshFace)) paramIndex = pManager.AddMeshFaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(Mesh)) paramIndex = pManager.AddMeshParameter(name, nickname, description, access);
//                else if (unitType == typeof(double)) paramIndex = pManager.AddNumberParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_Path)) paramIndex = pManager.AddPathParameter(name, nickname, description, access);
//                else if (unitType == typeof(Plane)) paramIndex = pManager.AddPlaneParameter(name, nickname, description, access);
//                else if (unitType == typeof(Point3d)) paramIndex = pManager.AddPointParameter(name, nickname, description, access);
//                else if (unitType == typeof(Rectangle3d)) paramIndex = pManager.AddRectangleParameter(name, nickname, description, access);
//                else if (unitType == typeof(SubD)) paramIndex = pManager.AddSubDParameter(name, nickname, description, access);
//                else if (unitType == typeof(Surface)) paramIndex = pManager.AddSurfaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(string)) paramIndex = pManager.AddTextParameter(name, nickname, description, access);
//                else if (unitType == typeof(DateTime)) paramIndex = pManager.AddTimeParameter(name, nickname, description, access);
//                else if (unitType == typeof(Transform)) paramIndex = pManager.AddTransformParameter(name, nickname, description, access);
//                else if (unitType == typeof(Vector3d)) paramIndex = pManager.AddVectorParameter(name, nickname, description, access);
//            }
//            if (access == GH_ParamAccess.item)
//            {
//                if (unitType == typeof(double)) paramIndex = pManager.AddAngleParameter(name, nickname, description, access, (double)defaultValue);
//                else if (unitType == typeof(Arc)) paramIndex = pManager.AddArcParameter(name, nickname, description, access, (Arc)defaultValue);
//                else if (unitType == typeof(bool)) paramIndex = pManager.AddBooleanParameter(name, nickname, description, access, (bool)defaultValue);
//                else if (unitType == typeof(Box)) paramIndex = pManager.AddBoxParameter(name, nickname, description, access, (Box)defaultValue);
//                else if (unitType == typeof(Brep)) paramIndex = pManager.AddBrepParameter(name, nickname, description, access);
//                else if (unitType == typeof(Circle)) paramIndex = pManager.AddCircleParameter(name, nickname, description, access, (Circle)defaultValue);
//                else if (unitType == typeof(System.Drawing.Color)) paramIndex = pManager.AddColourParameter(name, nickname, description, access, (System.Drawing.Color)defaultValue);
//                else if (unitType == typeof(GH_ComplexNumber)) paramIndex = pManager.AddComplexNumberParameter(name, nickname, description, access, (GH_ComplexNumber)defaultValue);
//                else if (unitType == typeof(CultureInfo)) paramIndex = pManager.AddCultureParameter(name, nickname, description, access, (CultureInfo)defaultValue);
//                else if (unitType == typeof(Curve)) paramIndex = pManager.AddCurveParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_Field)) paramIndex = pManager.AddFieldParameter(name, nickname, description, access);
//                else if (unitType == typeof(GeometryBase)) paramIndex = pManager.AddGeometryParameter(name, nickname, description, access);
//                else if (unitType == typeof(Group)) paramIndex = pManager.AddGroupParameter(name, nickname, description, access);
//                else if (unitType == typeof(int)) paramIndex = pManager.AddIntegerParameter(name, nickname, description, access, (int)defaultValue);
//                else if (unitType == typeof(GH_Interval2D)) paramIndex = pManager.AddInterval2DParameter(name, nickname, description, access);
//                else if (unitType == typeof(Interval)) paramIndex = pManager.AddIntervalParameter(name, nickname, description, access, (Interval)defaultValue);
//                else if (unitType == typeof(Line)) paramIndex = pManager.AddLineParameter(name, nickname, description, access, (Line)defaultValue);
//                else if (unitType == typeof(Matrix)) paramIndex = pManager.AddMatrixParameter(name, nickname, description, access);
//                else if (unitType == typeof(MeshFace)) paramIndex = pManager.AddMeshFaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(Mesh)) paramIndex = pManager.AddMeshParameter(name, nickname, description, access);
//                else if (unitType == typeof(double)) paramIndex = pManager.AddNumberParameter(name, nickname, description, access, (double)defaultValue);
//                else if (unitType == typeof(GH_Path)) paramIndex = pManager.AddPathParameter(name, nickname, description, access);
//                else if (unitType == typeof(Plane)) paramIndex = pManager.AddPlaneParameter(name, nickname, description, access, (Plane)defaultValue);
//                else if (unitType == typeof(Point3d)) paramIndex = pManager.AddPointParameter(name, nickname, description, access, (Point3d)defaultValue);
//                else if (unitType == typeof(Rectangle3d)) paramIndex = pManager.AddRectangleParameter(name, nickname, description, access, (Rectangle3d)defaultValue);
//                else if (unitType == typeof(SubD)) paramIndex = pManager.AddSubDParameter(name, nickname, description, access);
//                else if (unitType == typeof(Surface)) paramIndex = pManager.AddSurfaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(string)) paramIndex = pManager.AddTextParameter(name, nickname, description, access, (string)default);
//                else if (unitType == typeof(DateTime)) paramIndex = pManager.AddTimeParameter(name, nickname, description, access, (DateTime)defaultValue);
//                else if (unitType == typeof(Transform)) paramIndex = pManager.AddTransformParameter(name, nickname, description, access);
//                else if (unitType == typeof(Vector3d)) paramIndex = pManager.AddVectorParameter(name, nickname, description, access, (Vector3d)defaultValue);

//            }
//            else if(access == GH_ParamAccess.list)// || access == GH_ParamAccess.tree)
//            {
//                if(access == GH_ParamAccess.tree)
//                {
//                    //Get the first branch of a data tree as the default value
                    
//                }

//                if (unitType == typeof(double)) paramIndex = pManager.AddAngleParameter(name, nickname, description, access, (IEnumerable<double>)defaultValue);
//                else if (unitType == typeof(Arc)) paramIndex = pManager.AddArcParameter(name, nickname, description, access);
//                else if (unitType == typeof(bool)) paramIndex = pManager.AddBooleanParameter(name, nickname, description, access, (IEnumerable<bool>)defaultValue);
//                else if (unitType == typeof(Box)) paramIndex = pManager.AddBoxParameter(name, nickname, description, access);
//                else if (unitType == typeof(Brep)) paramIndex = pManager.AddBrepParameter(name, nickname, description, access);
//                else if (unitType == typeof(Circle)) paramIndex = pManager.AddCircleParameter(name, nickname, description, access);
//                else if (unitType == typeof(System.Drawing.Color)) paramIndex = pManager.AddColourParameter(name, nickname, description, access, (IEnumerable<System.Drawing.Color>)defaultValue);
//                else if (unitType == typeof(GH_ComplexNumber)) paramIndex = pManager.AddComplexNumberParameter(name, nickname, description, access);
//                else if (unitType == typeof(CultureInfo)) paramIndex = pManager.AddCultureParameter(name, nickname, description, access);
//                else if (unitType == typeof(Curve)) paramIndex = pManager.AddCurveParameter(name, nickname, description, access);
//                else if (unitType == typeof(GH_Field)) paramIndex = pManager.AddFieldParameter(name, nickname, description, access);
//                else if (unitType == typeof(GeometryBase)) paramIndex = pManager.AddGeometryParameter(name, nickname, description, access);
//                else if (unitType == typeof(Group)) paramIndex = pManager.AddGroupParameter(name, nickname, description, access);
//                else if (unitType == typeof(int)) paramIndex = pManager.AddIntegerParameter(name, nickname, description, access, (IEnumerable<int>)defaultValue);
//                else if (unitType == typeof(GH_Interval2D)) paramIndex = pManager.AddInterval2DParameter(name, nickname, description, access);
//                else if (unitType == typeof(Interval)) paramIndex = pManager.AddIntervalParameter(name, nickname, description, access, (IEnumerable<Interval>)defaultValue);
//                else if (unitType == typeof(Line)) paramIndex = pManager.AddLineParameter(name, nickname, description, access);
//                else if (unitType == typeof(Matrix)) paramIndex = pManager.AddMatrixParameter(name, nickname, description, access);
//                else if (unitType == typeof(MeshFace)) paramIndex = pManager.AddMeshFaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(Mesh)) paramIndex = pManager.AddMeshParameter(name, nickname, description, access);
//                else if (unitType == typeof(double)) paramIndex = pManager.AddNumberParameter(name, nickname, description, access, (IEnumerable<double>)defaultValue);
//                else if (unitType == typeof(GH_Path)) paramIndex = pManager.AddPathParameter(name, nickname, description, access);
//                else if (unitType == typeof(Plane)) paramIndex = pManager.AddPlaneParameter(name, nickname, description, access);
//                else if (unitType == typeof(Point3d)) paramIndex = pManager.AddPointParameter(name, nickname, description, access, (IEnumerable<Point3d>)defaultValue);
//                else if (unitType == typeof(Rectangle3d)) paramIndex = pManager.AddRectangleParameter(name, nickname, description, access);
//                else if (unitType == typeof(SubD)) paramIndex = pManager.AddSubDParameter(name, nickname, description, access);
//                else if (unitType == typeof(Surface)) paramIndex = pManager.AddSurfaceParameter(name, nickname, description, access);
//                else if (unitType == typeof(string)) paramIndex = pManager.AddTextParameter(name, nickname, description, access, (IEnumerable<string>)defaultValue);
//                else if (unitType == typeof(DateTime)) paramIndex = pManager.AddTimeParameter(name, nickname, description, access);
//                else if (unitType == typeof(Transform)) paramIndex = pManager.AddTransformParameter(name, nickname, description, access);
//                else if (unitType == typeof(Vector3d)) paramIndex = pManager.AddVectorParameter(name, nickname, description, access);

//            }




//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {

//        }

//        public abstract void SolveFromProperties();

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
//        /// to store data in output parameters.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            //TODO: Reset to default values between runs??

//            var rd = reflectionData;

//            // Then we need to access the input parameters individually. 
//            // When data cannot be extracted from a parameter, we should abort this method.
//            foreach (var input in rd.Inputs)
//            {
//                object inputRefVar = Activator.CreateInstance(input.UnitType);
//                if (input.ParamAccess == GH_ParamAccess.list)
//                {
//                    var listType = typeof(List<>);
//                    var constructedListType = listType.MakeGenericType(input.UnitType);
//                    inputRefVar = Activator.CreateInstance(constructedListType);
//                }
//                if (input.ParamAccess == GH_ParamAccess.tree)
//                {
//                    var listType = typeof(DataTree<>);
//                    var constructedListType = listType.MakeGenericType(input.UnitType);
//                    inputRefVar = Activator.CreateInstance(constructedListType);
//                }

//                object[] args = new object[] { input.Index, inputRefVar };
//                var getDataMinfo = ComponentReflectionData.GetDataMethods[input.ParamAccess];
//                MethodInfo getDataGeneric = getDataMinfo.MakeGenericMethod(new Type[] { input.UnitType });
//                bool success = (bool)getDataGeneric.Invoke(DA, args);
//                if (success)
//                {
//                    inputRefVar = args[1];
//                    input.Property.SetValue(this, inputRefVar);
//                }
//                else
//                {
//                    return;
//                }
//            }

//            SolveFromProperties();

//            foreach (var output in rd.Outputs)
//            {
//                var outputValue = output.Property.GetValue(this);

//                object[] args = new object[] { output.Index, outputValue };
//                var setDataMinfo = ComponentReflectionData.GetDataMethods[output.ParamAccess];
//                bool success = (bool)setDataMinfo.Invoke(DA, args);
//                if (success) { }
//                else { }
//            }
//        }

//        /// <summary>
//        /// The Exposure property controls where in the panel a component icon 
//        /// will appear. There are seven possible locations (primary to septenary), 
//        /// each of which can be combined with the GH_Exposure.obscure flag, which 
//        /// ensures the component will only be visible on panel dropdowns.
//        /// </summary>
//        public override GH_Exposure Exposure => GH_Exposure.primary;

//        /// <summary>
//        /// Provides an Icon for every component that will be visible in the User Interface.
//        /// Icons need to be 24x24 pixels.
//        /// You can add image files to your project resources and access them like this:
//        /// return Resources.IconForThisComponent;
//        /// </summary>
//        //protected override System.Drawing.Bitmap Icon => null;

//        /// <summary>
//        /// Each component must have a unique Guid to identify it. 
//        /// It is vital this Guid doesn't change otherwise old ghx files 
//        /// that use the old ID will partially fail during loading.
//        /// </summary>
//        public override Guid ComponentGuid => new Guid("FCC746FD-55EC-487D-A686-4AA1D3E7C50C");
//    }

//}
