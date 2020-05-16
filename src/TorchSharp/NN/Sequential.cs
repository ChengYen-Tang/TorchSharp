
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TorchSharp.Tensor;

namespace TorchSharp.NN
{
    /// <summary>
    /// This class is used to represent a Sequential module.
    /// </summary>
    public class Sequential : Module
    {
        internal Sequential (IntPtr handle, IEnumerable<Module> modules) : base (handle)
        {
            foreach (var module in modules) {
                RegisterModule (module);
            }
        }

        public override TorchTensor Forward (TorchTensor tensor)
        {
            if (!Modules.Any ()) {
                throw new ArgumentException ("Cannot do forward pass over empty Sequence module.");
            }

            var (head, tail) = Modules;
            var result = head.Forward (tensor);

            foreach (var module in tail) {
                var tmp = module.Forward (result);
                result.Dispose ();
                result = tmp;
            }

            return result;
        }

        public override void ZeroGrad ()
        {
            foreach (var module in Modules) {
                module.ZeroGrad ();
            }
        }

        public override IEnumerable<string> GetModules ()
        {
            List<string> result = new List<string> ();

            foreach (var module in Modules) {
                result.Add (module.GetName ());
            }

            return result;
        }

        public override void Train ()
        {
            foreach (var module in Modules) {
                module.Train ();
            }
        }

        public override void Eval ()
        {
            foreach (var module in Modules) {
                module.Eval ();
            }
        }
    }
    public static partial class Modules
    {
        [DllImport ("LibTorchSharp")]
        extern static IntPtr THSNN_sequentialModule ();

        static public Sequential Sequential (params Module[] modules)
        {
            var handle = THSNN_sequentialModule ();
            Torch.CheckForErrors ();
            return new Sequential (handle, modules);
        }
    }

}
