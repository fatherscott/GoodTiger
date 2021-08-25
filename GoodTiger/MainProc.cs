using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace GoodTiger
{
    public partial class SocketManager
    {
        public BufferBlock<StateObject> _bufferBlock = new BufferBlock<StateObject>();
    }
}
