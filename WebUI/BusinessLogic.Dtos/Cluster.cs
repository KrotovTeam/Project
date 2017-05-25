using System.Collections.Generic;
using System.Drawing;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    public class Cluster
    {
        public Dictionary<ChannelEnum, float> CenterCluster { get; set; }
        public IEnumerable<ClusterPoint> Points { get; set; }
        public bool IsJoined { get; set; }
        public float Ndvi { get; set; }
        public Color ClusterColor { get; set; }

        public Cluster()
        {
            CenterCluster = new Dictionary<ChannelEnum, float>();
            Points = new List<ClusterPoint>();
            IsJoined = false;
        }
    }
}
