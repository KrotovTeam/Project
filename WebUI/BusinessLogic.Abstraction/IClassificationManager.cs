using System.Collections.Generic;
using BusinessLogic.Dtos;

namespace BusinessLogic.Abstraction
{
    public interface IClassificationManager
    {
        IEnumerable<Cluster> Clustering(IEnumerable<Point> points);
    }
}
