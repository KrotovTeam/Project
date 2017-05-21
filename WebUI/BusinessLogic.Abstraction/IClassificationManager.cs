using System.Collections.Generic;
using BusinessLogic.Dtos;

namespace BusinessLogic.Abstraction
{
    public interface IClassificationManager
    {
        IEnumerable<Cluster> Classify(Point[] points);
    }
}
