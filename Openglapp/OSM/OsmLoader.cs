using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OpenglApp.SampleObject;

namespace OpenglApp.OSM
{
    class OsmLoader
    {
        public static void Load(string mapfile, out Dictionary<long, Node> nodes, out Way[] ways, out Building[] buildings)
        {
            var xml = XDocument.Load(mapfile);

            nodes =
                xml.Root.Descendants("node")
                    .Select(GetNode).ToDictionary(_ => _.id);

            ways =
                xml.Root.Descendants("way")
                    .Where(_ => _.Descendants("tag").Any(_ => _.Attribute("k").Value == "highway"))
                    .Select(GetWay).ToArray();

            buildings =
                xml.Root.Descendants("way")
                    .Where(_ => _.Descendants("tag").Any(_ => _.Attribute("k").Value == "building"))
                    .Select(GetBuilding).ToArray();
        }

        private static Node GetNode(XElement element)
        {
            var id = element.Attribute("id").Value;

            return new Node
            {
                id = long.Parse(id),
                lat = double.Parse(element.Attribute("lat").Value),
                lon = double.Parse(element.Attribute("lon").Value)
            };
        }

        private static Way GetWay(XElement element)
        {
            var id = element.Attribute("id").Value;

            return new Way
            {
                id = long.Parse(id),
                nodes = element.Descendants("nd").Select(_ => long.Parse(_.Attribute("ref").Value)).ToArray()
            };
        }

        private static Building GetBuilding(XElement element)
        {
            var id = element.Attribute("id").Value;

            return new Building
            {
                Id = long.Parse(id),
                Nodes = element.Descendants("nd").Select(_ => long.Parse(_.Attribute("ref").Value)).ToArray(),
                Type = element.Descendants("tag").Single(_ => _.Attribute("k").Value == "building").Attribute("v").Value,
                Levels = GetBuildingLevels(element)
            };
        }

        private static float? GetBuildingLevels(XElement element)
        {
            var attr = element.Descendants("tag").SingleOrDefault(_ => _.Attribute("k").Value == "building:levels");
            
            if (attr == null)
            {
                return null;
            }

            return float.Parse(attr.Attribute("v").Value);
        }
    }
}
