// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

// A number of implementation details in this file have been translated from the Python version of torchvision,
// largely located in the files found in this folder:
//
// https://github.com/pytorch/vision/blob/a4f53308b2d0f1aa9191686e326f45c26053f686/torchvision/ops/misc.py
//
// The origin has the following copyright notice and license:
//
// https://github.com/pytorch/vision/blob/main/LICENSE
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TorchSharp.torch;

#nullable enable
namespace TorchSharp
{
    public static partial class torchvision
    {
        /// <summary>
        /// BatchNorm2d where the batch statistics and the affine parameters are fixed
        /// </summary>
        public abstract class FrozenBatchNorm2d : nn.Module<Tensor, Tensor>
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="num_features">Number of features C from an expected input of size (N, C, H, W)</param>
            /// <param name="eps">A value added to the denominator for numerical stability.</param>
            protected FrozenBatchNorm2d(int num_features, double eps = 1e-5) : base(nameof(FrozenBatchNorm2d))
            {
                this.eps = eps;

                weight = torch.ones(num_features);
                bias = torch.zeros(num_features);
                running_mean = torch.zeros(num_features);
                running_var = torch.ones(num_features);

                register_buffer(nameof(weight), weight);
                register_buffer(nameof(bias), bias);
                register_buffer(nameof(running_mean), running_mean);
                register_buffer(nameof(running_var), running_var);
            }

            private double eps;

            public Tensor weight { get; protected set; }
            public Tensor bias { get; protected set; }
            public Tensor running_mean { get; protected set; }
            public Tensor running_var { get; protected set; }

            public override Tensor forward(Tensor x)
            {
                var w = weight.reshape(1, -1, 1, 1);
                var b = bias.reshape(1, -1, 1, 1);
                var rv = running_var.reshape(1, -1, 1, 1);
                var rm = running_mean.reshape(1, -1, 1, 1);
                var scale = w * (rv + eps).rsqrt();
                bias = b - rm * scale;
                return x * scale + bias;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) {
                    weight.Dispose();
                    bias.Dispose();
                    running_mean.Dispose();
                    running_var.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
}
