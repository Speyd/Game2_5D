using DataPipes.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace DataPipes
{
    public class PooledVertexArray : IResettable
    {
        public VertexArray VertexArray { get; private set; }

        public PooledVertexArray()
        {
            VertexArray = new VertexArray(PrimitiveType.Quads, 4);
        }

        public void Reset()
        {
            VertexArray.Clear();
        }
    }
}
