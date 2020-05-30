using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using System;
using System.Linq;
using Buffer = Stride.Graphics.Buffer;

namespace StrideTransformationInstancing
{
    public class InstanceUpdaterAutoBuffer : InstanceUpdaterBase
    {
        protected override int InstanceCountSqrt => 70;

        protected override void ManageInstancingData()
        {
            instancingComponent.UpdateWorldMatrices(instanceWorldTransformations);
        }
    }
}
