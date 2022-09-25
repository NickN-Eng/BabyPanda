using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BabyPanda.SynopticSvg
{
    public class SPSvgDocument
    {
        private SvgDocument SvgDocument = new SvgDocument();

        public List<SPElement> Elements = new List<SPElement>();

        /// <summary>
        /// The padding around the elements
        /// </summary>
        public float SvgViewboxBuffer = 10f;

        /// <summary>
        /// Generates the svg document. Must be run before .WriteTo()
        /// </summary>
        public void Generate()
        {
            // Create a viewbox which includes every shape
            // By expanding an axis alingned box to fit every element
            var box = Elements[0].BoundingBox;
            for (int i = 1; i < Elements.Count; i++)
            {
                box.Contain(Elements[i].BoundingBox);
            }
            box.Expand(SvgViewboxBuffer);
            SvgDocument.ViewBox = new SvgViewBox(box.Min.x, box.Min.y, box.Width, box.Height);

            // Add the xml for each element to the document
            foreach (var elem in Elements)
            {
                var svgElem = elem.GetElement(Vector2f.Zero, false);
                SvgDocument.Children.Add(svgElem);
            }
        }

        /// <summary>
        /// Generates the svg document. Must be run before .WriteTo()
        /// Flips and offsets coordinates
        /// </summary>
        public void GenerateFromRealWorldCoords()
        {
            // Create a viewbox which includes every shape
            // By expanding an axis alingned box to fit every element
            var box = Elements[0].BoundingBox; 
            for (int i = 1; i < Elements.Count; i++)
            {
                box.Contain(Elements[i].BoundingBox);
            }
            box.Expand(SvgViewboxBuffer);
            //SvgDocument.ViewBox = new SvgViewBox(box.Min.x, box.Min.y, box.Width, box.Height);
            SvgDocument.ViewBox = new SvgViewBox(0, 0, box.Width, box.Height);

            var offset = new Vector2f(-box.Min.x, box.Max.y);
            // Add the xml for each element to the document
            foreach (var elem in Elements)
            {
                var svgElem = elem.GetElement(offset, true);
                SvgDocument.Children.Add(svgElem);
            }
        }

        /// <summary>
        /// Writes the svg document to the provided path. Generate() must have already been run.
        /// </summary>
        /// <param name="path"></param>
        public void WriteTo(string path)
        {
            SvgDocument.Write(path);
        }

        public string WriteAsString()
        {
            var sWriter = new StringWriter();
            var xWriter = new XmlTextWriter(sWriter);
            SvgDocument.Write(xWriter);
            return sWriter.ToString();
        }
    }
}
