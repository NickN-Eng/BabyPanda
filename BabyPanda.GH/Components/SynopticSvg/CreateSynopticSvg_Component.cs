using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyPanda;
using BabyPanda.SynopticSvg;
using Microsoft.Office.Interop.Excel;
using Rhino.Geometry;
using BabyPanda.SynopticSvg;
using Svg;

namespace BabyPanda.GH
{
    public class CreateSynopticSvg_Component : BP_Component
    {
        public CreateSynopticSvg_Component()
          : base(name: "Create Synoptic Svg", nickname: "CSSvg",
            description: "Create a Synoptic Panel Svg.",
            category: GhUIConstants.RibbonCategory, subCategory: GhUIConstants.Ribbon_Svg)
        {
        }

        public override Guid ComponentGuid => new Guid("e0a15282-5c7f-4705-b062-d1ed54cbbdbe");

        public float SPRegion_CurveThickness { get; set; } = 1;
        //public System.Drawing.Color SPRegion_CurveColour { get; set; }
        //public System.Drawing.Color SPRegion_Fill { get; set; }
        public SvgColourServer SPRegion_CurveColour { get; set; } = SPColors.Black;
        public SvgColourServer SPRegion_Fill { get; set; } = SPColors.FromRgb(173, 216, 230, 100);

        //public System.Drawing.Color Background_Fill { get; set; }
        public SvgColourServer Background_Fill { get; set; } = SPColors.Gray;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Svg Filepath", "FP", "Filepath for Svg to be written to", GH_ParamAccess.item);

            pManager.AddCurveParameter("SPRegion Curve", "SP Crv", "A closed XY curve for a hilighted region on a Synoptic Panel SVG. Only curves which can be easily converted to a polyline are supported. E.g. degree 1 NurbsCurve, a PolylineCurve, and some PolyCurves.", GH_ParamAccess.list);
            pManager.AddTextParameter("SPRegion Id", "SP Id", "The data Id for a hilighted region for a Synoptic Panel SVG.", GH_ParamAccess.list);
            pManager.AddTextParameter("SPRegion Subcategory", "SP Sub", "The data sub category for a hilighted region for a Synoptic Panel SVG.", GH_ParamAccess.list, "");

            pManager.AddCurveParameter("Background Curve", "Bk Crv", "A closed XY curve for a background lines for a Synoptic Panel SVG. Only curves which can be easily converted to a polyline are supported. E.g. degree 1 NurbsCurve, a PolylineCurve, and some PolyCurves.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Background Thickness", "Bk Thk", "The line thickness for background lines on a Synoptic Panel SVG.", GH_ParamAccess.list);
            pManager.AddColourParameter("Background Colour", "Bk Col", "The line colour for background lines on a Synoptic Panel SVG.", GH_ParamAccess.list);

            pManager.AddBooleanParameter("Write", "W", "Set to true to write the Svg.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Success", "S", "True if writing is successful", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA_GetDataAndCheckNull(DA, 7, out bool Write)) return;
            if (!Write)
            {
                DA.SetData(0, false);
                return;
            }

            if (!DA_GetDataAndCheckNull(DA, 0, out string FilePath)) return;
            if (!DA_GetDataList(DA, 1, out List<Curve> RegionCrv)) return;
            if (!DA_GetDataList(DA, 2, out List<string> RegionId)) return;
            if (!DA_GetDataList(DA, 3, out List<string> RegionSub)) return;

            if (!DA_GetDataList(DA, 4, out List<Curve> BackgroundCrv)) return;
            if (!DA_GetDataList(DA, 5, out List<double> BackgroundThickness)) return;
            if (!DA_GetDataList(DA, 6, out List<System.Drawing.Color> BackgroundColour)) return;

            if (!CheckListsAreSameLength("SPRegion lists", RegionCrv, RegionId, RegionSub)) return;
            if (!CheckListsAreSameLength("Background lists", BackgroundCrv, BackgroundThickness, BackgroundColour)) return;

            var svgdoc = new SPSvgDocument();

            for (int i = 0; i < BackgroundCrv.Count; i++)
            {
                if (!TryGetPolygon(BackgroundCrv[i], out SPPolygon ipolygon)) continue;
                var spelem = ipolygon.AsSPBackground().WithAppearance((float)BackgroundThickness[i], FromDrawingColour(BackgroundColour[i]), SPRegion_Fill);
                svgdoc.Elements.Add(spelem);
            }

            for (int i = 0; i < RegionCrv.Count; i++)
            {
                if (!TryGetPolygon(RegionCrv[i], out SPPolygon ipolygon)) continue;
                var spelem = ipolygon.AsSPArea(RegionId[i], RegionSub[i]).WithAppearance(SPRegion_CurveThickness, SPRegion_CurveColour, SPRegion_Fill);
                svgdoc.Elements.Add(spelem);
            }

            svgdoc.Generate();

            try
            {
                svgdoc.WriteTo(FilePath);
            }
            catch (Exception e)
            {
                DA.SetData(0, false);
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to write svg to file.");
                return;
            }

            DA.SetData(0, true);
        }


        private bool TryGetPolygon(Curve curve, out SPPolygon polygon)
        {
            if(curve.TryGetPolyline(out Polyline pline))
            {
                bool isClosed = pline.IsClosed; //Rhino behaviour to be tested.
                var points = pline.Select(pt => new BabyPanda.SynopticSvg.Vector2f(pt.X, pt.Y)).ToArray();
                polygon = SPPolygon.FromPoints(isClosed, points);

                return true;
            }
            polygon = null;
            return false;
        }

        private SvgColourServer FromDrawingColour(System.Drawing.Color colour)
        {
            return new SvgColourServer(colour);
        }

        protected override System.Drawing.Bitmap Icon => null;

    }
}
