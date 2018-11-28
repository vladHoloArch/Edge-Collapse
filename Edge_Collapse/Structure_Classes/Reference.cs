using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edge_Collapse
{
    [Serializable]
    public class Reference : ICloneable
    {
        public int triangleId;
        public int triangleVertex;

        public object Clone()
        {
            return new Reference() { triangleId = this.triangleId, triangleVertex = this.triangleVertex };
        }
    }
}
