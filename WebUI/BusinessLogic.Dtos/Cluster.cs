using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    public class Cluster
    {
        public Dictionary<ChannelEnum, double> CenterCluster { get; set; }
        public IEnumerable<ClusterPoint> Points { get; set; }
        public bool IsJoined { get; set; }
        public double Ndvi { get; set; }

        public Cluster()
        {
            CenterCluster = new Dictionary<ChannelEnum, double>();
            Points = new List<ClusterPoint>();
            IsJoined = false;
        }
    }
}
