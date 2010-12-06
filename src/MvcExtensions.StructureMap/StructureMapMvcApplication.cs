#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.StructureMap
{
    using System.Web;

    /// <summary>
    /// Defines a <see cref="HttpApplication"/> which uses <seealso cref="StructureMapBootstrapper"/>.
    /// </summary>
    public class StructureMapMvcApplication : ExtendedMvcApplication
    {
        /// <summary>
        /// Creates the bootstrapper.
        /// </summary>
        /// <returns></returns>
        protected override IBootstrapper CreateBootstrapper()
        {
            return new StructureMapBootstrapper(BuildManagerWrapper.Current, BootstrapperTasksRegistry.Current, PerRequestTasksRegistry.Current);
        }
    }
}