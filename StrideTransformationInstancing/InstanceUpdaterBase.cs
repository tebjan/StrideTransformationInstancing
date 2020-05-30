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
        const int icSqrt = 50;
        protected InstancingComponent instancingComponent;
        protected Matrix[] instanceWorldTransformations;

        public override void Start()
        {
            var profiler = new GameProfiler
            {
                Enabled = true
            };

            Entity.Add(profiler);

            var ic = icSqrt * icSqrt;
            instancingComponent = Entity.GetOrCreate<InstancingComponent>();
            instanceWorldTransformations = new Matrix[ic];
        }

        public override void Update()
        {
            // generate some matrices
            var offset = icSqrt / 2;
            for (int i = 0; i < icSqrt; i++)
            {
                var col = i * icSqrt;
                for (int j = 0; j < icSqrt; j++)
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
