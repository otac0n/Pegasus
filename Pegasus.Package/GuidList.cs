// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Package
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static class GuidList
    {
        public const string PegasusCommandSetGuid = "6b605fd3-df12-4868-a1e7-38065862a5c1";
        public const string PegasusPackageGuid = "243c099e-6e07-4be4-a418-84e77bb0f038";

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Required.")]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required.")]
        public static readonly Guid PegasusCommandSet = new Guid(PegasusCommandSetGuid);
    }
}
