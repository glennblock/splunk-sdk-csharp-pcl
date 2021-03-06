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

// TODO:
// [ ]  Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for creating a new <see cref=
    /// "ServerMessage"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/WlDoZx">REST API Reference: POST messages</a>.
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ServerMessageArgs : Args<ServerMessageArgs>
    {
        #region Constructors

        public ServerMessageArgs(ServerMessageSeverity type, string text)
        {
            this.Type = type;
            this.Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of a <see cref="ServerMessage"/>.
        /// </summary>
        [DataMember(Name = "severity", IsRequired = true)]
        public ServerMessageSeverity Type
        { get; private set; }

        /// <summary>
        /// Gets or sets the text of a <see cref="ServerMessage"/>.
        /// </summary>
        [DataMember(Name = "value", IsRequired = true)]
        public string Text
        { get; private set; }

        #endregion
    }
}