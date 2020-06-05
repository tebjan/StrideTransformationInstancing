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
        protected override int InstanceCountSqrt => 20;

        InstancingUserArray instancingMany;

        protected override IInstancing GetInstancingType()
        {
            instancingMany = new InstancingUserArray();
            return instancingMany;
        }

        protected override void ManageInstancingData()
        {
            instancingMany.UpdateWorldMatrices(instanceWorldTransformations);
        }
    }
}
