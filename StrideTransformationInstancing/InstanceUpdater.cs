using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using System;
using System.Linq;
using Buffer = Stride.Graphics.Buffer;

namespace StrideTransformationInstancing
{
    public class InstanceUpdater : SyncScript
    {
        const int icSqrt = 100;
        ModelComponent modelComponent;

        public override void Start()
        {
            var profiler = new GameProfiler();
            profiler.Enabled = true;
            Entity.Add(profiler);

            var ic = icSqrt * icSqrt;
            modelComponent = Entity.Get<ModelComponent>();
            modelComponent.InstanceWorldBuffer = Buffer.New<Matrix>(Game.GraphicsDevice, ic, BufferFlags.ShaderResource | BufferFlags.StructuredBuffer, GraphicsResourceUsage.Dynamic); ;
            modelComponent.InstanceWorldInverseBuffer = Buffer.New<Matrix>(Game.GraphicsDevice, ic, BufferFlags.ShaderResource | BufferFlags.StructuredBuffer, GraphicsResourceUsage.Dynamic); ;

            modelComponent.InstanceWorldMatrices = new Matrix[ic];
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

                    modelComponent.InstanceWorldMatrices[col + j] = Matrix.Translation(x, y, z);
                }
            }

            Entity.Transform.Scale = new Vector3(0.1f);
            Entity.Transform.Position = Vector3.Zero;

            // Make sure inverse matrices are big enough
            if (modelComponent.InstanceWorldInverseMatrices.Length != modelComponent.InstanceWorldMatrices.Length)
            {
                modelComponent.InstanceWorldInverseMatrices = new Matrix[modelComponent.InstanceWorldMatrices.Length];
            }

            // Invert matrices and update bounding box
            var ibb = BoundingBox.Empty;
            for (int i = 0; i < modelComponent.InstanceWorldMatrices.Length; i++)
            {
                Matrix.Invert(ref modelComponent.InstanceWorldMatrices[i], out modelComponent.InstanceWorldInverseMatrices[i]);
                var pos = modelComponent.InstanceWorldMatrices[i].TranslationVector;
                BoundingBox.Merge(ref ibb, ref pos, out ibb);
            }

            foreach (var mesh in modelComponent.Model.Meshes)
            {
                mesh.BoundingBox = ibb;
            }

            modelComponent.InstanceWorldBuffer.SetData(Game.GraphicsContext.CommandList, modelComponent.InstanceWorldMatrices);
            modelComponent.InstanceWorldInverseBuffer.SetData(Game.GraphicsContext.CommandList, modelComponent.InstanceWorldInverseMatrices);

        }
    }
}
