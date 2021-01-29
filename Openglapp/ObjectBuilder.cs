using OpenglApp.OSM;
using OpenglApp.SampleObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenglApp
{
    internal class ObjectBuilder
    {
        internal static IEnumerable<IObject> CreateBuildings(Dictionary<long, Node> nodes, BuildingDefinition[] buildings)
        {
            foreach (var b in buildings)
            {
                yield return CreateBuilding(nodes, b);
            }
        }

        private static IObject CreateBuilding(Dictionary<long, Node> nodes, BuildingDefinition b)
        {
            var foundation = b.Nodes.Select(_ => (nodes[_].lat, nodes[_].lon));

            return null;
            //return new Building(foundation, b.Levels);
        }
    }
}