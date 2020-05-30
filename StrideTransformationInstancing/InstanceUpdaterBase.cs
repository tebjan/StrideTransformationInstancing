using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using System;
using System.Linq;
using Buffer = Stride.Graphics.Buffer;

namespace StrideTransformationInstancing
{
    public abstract class InstanceUpdaterBase : SyncScript
    {
        protected abstract int InstanceCountSqrt { get; }
        protected InstancingComponent instancingComponent;
        protected Matrix[] instanceWorldTransformations;

        public override void Start()
        {
            var profiler = new GameProfiler
            {
                Enabled = true
            };

            Entity.Add(profiler);

            var ic = InstanceCountSqrt * InstanceCountSqrt;
            instancingComponent = Entity.GetOrCreate<InstancingComponent>();
            instanceWorldTransformations = new Matrix[ic];
        }

        public override void Update()
        {
            // generate some matrices
            var offset = InstanceCountSqrt / 2;
            for (int i = 0; i < InstanceCountSqrt; i++)
            {
                var col = i * InstanceCountSqrt;
                for (int j = 0; j < InstanceCountSqrt; j++)
                {
                    var x = i * 1 - offset;
                    var y = j * 1 - offset;
                    var z = (float)Math.Cos(new Vector2(x, y).Length() * 0.5f + Game.UpdateTime.Total.TotalSeconds);

                    instanceWorldTransformations[col + j] = Matrix.Translation(x, y, z);
                }
            }

            Entity.Transform.Scale = new Vector3(0.1f);

            ManageInstancingData();
        }

        protected abstract void ManageInstancingData();
    }
}
