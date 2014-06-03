﻿/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

//// TODO:
//// [O] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk configuration 
    /// files.
    /// </summary>
    public class ConfigurationCollection : EntityCollection<Configuration>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal ConfigurationCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <see cref="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal ConfigurationCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal ConfigurationCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetApplicationsAsync"/> to asynchronously retrieve a 
        /// collection of installed Splunk applications.
        /// </remarks>
        public ConfigurationCollection()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST 
        /// properties</a> endpoint to create the configuration file represented
        /// by this instance.
        /// </remarks>
        public async Task CreateAsync(string name)
        {
            var arguments = new Argument[] { new Argument("__conf", name) };
            await base.CreateAsync(arguments);
        }

        /// <summary>
        /// Unsupported. This method is not supported by the <see cref=
        /// "ConfigurationCollection"/> class because it is not supported by 
        /// the <a href="http://goo.gl/Unj6fs">Splunk properties endpoint</a>.
        /// </summary>
        /// <returns></returns>
        public override async Task GetSliceAsync(params Argument[] arguments)
        {
            await this.GetSliceAsync(arguments.AsEnumerable());
        }

        /// <summary>
        /// Unsupported. This method is not supported by the <see cref=
        /// "ConfigurationCollection"/> class because it is not supported by 
        /// the <a href="http://goo.gl/Unj6fs">Splunk properties endpoint</a>.
        /// </summary>
        /// <returns></returns>
        public override Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            throw new NotSupportedException("The Splunk properties endpoint can only return the full list of configuration files.")
            {
                HelpLink = "http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTconfig#GET_properties"
            };
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("properties");
        
        #endregion
    }
}