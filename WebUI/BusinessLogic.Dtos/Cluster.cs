﻿using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    public class Cluster
    {
        public Dictionary<ChannelEnum, float> CenterCluster { get; set; }
        public IEnumerable<ClusterPoint> Points { get; set; }
        public bool IsJoined { get; set; }

        public Cluster()
        {
            CenterCluster = new Dictionary<ChannelEnum, float>();
            Points = new List<ClusterPoint>();
            IsJoined = false;
        }
    }
}
