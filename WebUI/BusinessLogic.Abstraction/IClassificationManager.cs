using System.Collections.Generic;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IClassificationManager
    {
        IEnumerable<Cluster> Clustering(IEnumerable<Point> points);
    }
}
