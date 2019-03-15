namespace OpenglApp.SampleObject
{
    public struct Vertex<T>
    {
        Vector Position { get; set; }

        public T U { get; set; }
        public T V { get; set; }
    }
}