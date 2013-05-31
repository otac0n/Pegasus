// -----------------------------------------------------------------------
// <copyright file="PegasusPackage.cs" company="(none)">
//   Copyright © 2013 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Package
{
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.PegasusPackageGuid)]
    [ProvideService(typeof(PegasusLanguageService), ServiceName = "Pegasus Language Service")]
    [ProvideLanguageService(typeof(PegasusLanguageService), "Pegasus", 110, CodeSense = true, RequestStockColors = true, EnableCommenting = true, EnableAsyncCompletion = true)]
    [ProvideLanguageExtension(typeof(PegasusLanguageService), ".peg")]
    public sealed class PegasusPackage : Microsoft.VisualStudio.Shell.Package
    {
        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
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
