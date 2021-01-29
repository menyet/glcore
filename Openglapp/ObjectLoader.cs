using OpenglApp.SampleObject;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace Openglapp
{
    public class ObjectLoader
    {
        public IEnumerable<IObject> LoadObjects()
        {
            yield return GetStreet();
        }

        private IObject GetStreet()
        {
            var points = new List<Vector>
            {
                new Vector(0, 0, 0),
                new Vector(0, 0, 3.0f),
                new Vector(3.0f, 2.0f, 3.0f),
                new Vector(4.0f, 2.0f, 3.0f),
                new Vector(6.0f, 0.0f, 3.0f),
                new Vector(6.0f, 0.0f, 0.0f)
            };
            var obj = new Street(Casteljau(points)
            .Select(_ => new StreetEndConfig
            {
                Position = _
            }).ToList(), 1, 0.4f, 0.05f);


            obj.Position = new Vector3(0.0f, 0.0f, -3.0f);
            obj.Init();
            return obj;
        }

         private IEnumerable<Vector> Casteljau(List<Vector> points)
        {
            for (float t = 0; t <= 1; t += 0.01f)
            {
                yield return GetPoint(points.Count - 1, 0, t);
            }

            Vector GetPoint(int level, int i, float t)
            {
                if (level == 0)
                {
                    return points[i];
                }

                var p1 = GetPoint(level - 1, i, t);
                var p2 = GetPoint(level - 1, i + 1, t);

                return (1.0f - t) * p1 + t * p2;
            }
        }
    }
}