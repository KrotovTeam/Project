using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    public class Cluster
    {
        public Dictionary<CoordinateSystemEnum, int> CenterCluster { get; set; }
        public IEnumerable<ResultingPoint> Points { get; set; }
        public bool IsJoined { get; set; }
        public double Ndvi { get; set; }

        public Cluster()
        {
            CenterCluster = new Dictionary<CoordinateSystemEnum, int>();
            Points = new List<ResultingPoint>();
            IsJoined = false;
        }
    }
}
