using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using System;
using System.Linq;
using Buffer = Stride.Graphics.Buffer;

namespace StrideTransformationInstancing
{
    public class InstanceUpdaterUserBuffer : InstanceUpdaterBase
    {
        Buffer<Matrix> InstanceWorldBuffer;
        Buffer<Matrix> InstanceWorldInverseBuffer;

        // TODO: make this more easy and clear, improve instancing component to support this better
        protected override void ManageInstancingData()
        {
            instancingComponent.WorldMatrices = instanceWorldTransformations;
            instancingComponent.InstanceCount = instanceWorldTransformations.Length;

            // Make sure inverse matrices are big enough
            if (instancingComponent.WorldInverseMatrices.Length != instancingComponent.WorldMatrices.Length)
            {
                instancingComponent.WorldInverseMatrices = new Matrix[instancingComponent.WorldMatrices.Length];
            }

            // Invert matrices and update bounding box
            var ibb = BoundingBox.Empty;
            for (int i = 0; i < instancingComponent.WorldMatrices.Length; i++)
            {
                Matrix.Invert(ref instancingComponent.WorldMatrices[i], out instancingComponent.WorldInverseMatrices[i]);
                var pos = instancingComponent.WorldMatrices[i].TranslationVector;
                BoundingBox.Merge(ref ibb, ref pos, out ibb);
            }

            instancingComponent.BoundingBox = ibb;

            // Manage buffers
            if (InstanceWorldBuffer == null || InstanceWorldBuffer.ElementCount < instancingComponent.InstanceCount)
            {
                InstanceWorldBuffer?.Dispose();
                InstanceWorldInverseBuffer?.Dispose();

                InstanceWorldBuffer = CreateMatrixBuffer(GraphicsDevice, instancingComponent.InstanceCount);
                instancingComponent.InstanceWorldBuffer = InstanceWorldBuffer;

                InstanceWorldInverseBuffer = CreateMatrixBuffer(GraphicsDevice, instancingComponent.InstanceCount);
                instancingComponent.InstanceWorldInverseBuffer = InstanceWorldInverseBuffer;
            }

            instancingComponent.InstanceWorldBuffer.SetData(Game.GraphicsContext.CommandList, instancingComponent.WorldMatrices);
            instancingComponent.InstanceWorldInverseBuffer.SetData(Game.GraphicsContext.CommandList, instancingComponent.WorldInverseMatrices);

        }

        private static Buffer<Matrix> CreateMatrixBuffer(GraphicsDevice graphicsDevice, int elementCount)
        {
            return Buffer.New<Matrix>(graphicsDevice, elementCount, BufferFlags.ShaderResource | BufferFlags.StructuredBuffer, GraphicsResourceUsage.Dynamic);
        }
    }
}
