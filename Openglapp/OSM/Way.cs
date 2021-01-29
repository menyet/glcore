namespace OpenglApp.OSM
{
    public class Way
    {
        public long id { get; set; }
        public long[] nodes { get; set; }
    }

    public class BuildingDefinition
    {
        public long Id { get; set; }

        public long[] Nodes { get; set; }

        public string Type { get; set; }

        public float? Levels { get; set; }
    }
}