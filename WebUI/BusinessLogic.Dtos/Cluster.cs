using System.Collections.Generic;

namespace BusinessLogic.Dtos
{
    public class Cluster
    {
        public float CenterCluster { get; set; }
        public IEnumerable<Point> Points { get; set; }
        
    }
}
