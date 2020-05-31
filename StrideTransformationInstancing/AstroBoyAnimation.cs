﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Animations;
using Stride.Input;
using Stride.Engine;

namespace StrideTransformationInstancing
{
    [DataContract("PlayAnimation")]
    public class PlayAnimation
    {
        public AnimationClip Clip;
        public AnimationBlendOperation BlendOperation = AnimationBlendOperation.LinearBlend;
        public double StartTime = 0;
    }

    /// <summary>
    /// Script which starts a few animations on its entity
    /// </summary>
    public class AstroBoyAnimation : SyncScript
    {
        /// <summary>
        /// A list of animations to be loaded when the script starts
        /// </summary>
        public readonly List<PlayAnimation> Animations = new List<PlayAnimation>();
        protected InstancingComponent instancingComponent;
        protected Matrix[] instanceWorldTransformations;
        int InstanceCount = 20;

        public override void Start()
        {
            ((Game)Game).WindowMinimumUpdateRate.SetMaxFrequency(60);

            var profiler = new GameProfiler
            {
                Enabled = true
            };

            Entity.Add(profiler);

            var animComponent = Entity.GetOrCreate<AnimationComponent>();

            if (animComponent != null)
                PlayAnimations(animComponent);

            instancingComponent = Entity.GetOrCreate<InstancingComponent>();
            instanceWorldTransformations = new Matrix[InstanceCount];
        }

        public override void Update()
        {
            // generate some matrices
            var offset = InstanceCount / 2;
            for (int i = 0; i < InstanceCount; i++)
            {
                var x = i * 1 - offset;
                var y = 0;
                var z = (float)Math.Cos(new Vector2(x, y).Length() * 0.5f + Game.UpdateTime.Total.TotalSeconds);

                instanceWorldTransformations[i] = Matrix.Translation(x * 0.5f, y, z);
            }

            instancingComponent.UpdateWorldMatrices(instanceWorldTransformations);
        }

        private void PlayAnimations(AnimationComponent animComponent)
        {
            foreach (var anim in Animations)
            {
                if (anim.Clip != null)
                    animComponent.Add(anim.Clip, anim.StartTime, anim.BlendOperation);
            }

            Animations.Clear();
        }
    }
}