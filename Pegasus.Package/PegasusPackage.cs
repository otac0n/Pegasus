// -----------------------------------------------------------------------
// <copyright file="PegasusPackage.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Package
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.PegasusPackageGuid)]
    [ProvideBindingPath]
    [ProvideService(typeof(PegasusLanguageService), ServiceName = "Pegasus Language Service")]
    [ProvideLanguageService(typeof(PegasusLanguageService), "Pegasus", 110, CodeSense = true, RequestStockColors = true, EnableCommenting = true, EnableAsyncCompletion = true)]
    [ProvideLanguageExtension(typeof(PegasusLanguageService), ".peg")]
    public sealed class PegasusPackage : Package
    {
        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The language service is added to the service container, which manages its lifetime.")]
        protected override void Initialize()
        {
            base.Initialize();

            var serviceContainer = this as IServiceContainer;
            var langService = new PegasusLanguageService();
            langService.SetSite(this);
            serviceContainer.AddService(typeof(PegasusLanguageService), langService, true);
        }
    }
}
