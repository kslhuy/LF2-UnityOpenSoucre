using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// A component for syncing transforms
    /// NetworkTransform will read the underlying transform and replicate it to clients.
    /// The replicated value will be automatically be interpolated (if active) and applied to the underlying GameObject's transform
    /// </summary>
    [DefaultExecutionOrder(100000)] // this is needed to catch the update time after the transform was updated by user scripts
    public class ClientNetTransform : NetTransform
    {
 
        /// <summary>
        /// Override this method and return false to switch to owner authoritative mode
        /// </summary>
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

    }
}
