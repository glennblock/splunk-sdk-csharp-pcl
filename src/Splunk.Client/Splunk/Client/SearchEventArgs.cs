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
// [ ] Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments required for retrieving untransformed search results.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/eZzuBh">REST API Reference: POST search/jobs/{search_id}/events</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class SearchEventArgs : Args<SearchEventArgs>
    {
        #region Constructors

        public SearchEventArgs()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The maximum number of results to return.
        /// </summary>
        /// <remarks>
        /// If the value of <c>Count</c> is set to zero, then all available
        /// results are returned. The default value is 100.
        /// </remarks>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int Count
        { get; set; }

        /// <summary>
        /// The list of fields to return in the results.
        /// </summary>
        [DataMember(Name = "f", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public IReadOnlyList<string> FieldList
        { get; set; }

        /// <summary>
        /// A time string representing the latest (exclusive), respectively, 
        /// time bounds for the results to be returned.
        /// </summary>
        /// <remarks>
        /// If not specified, the range applies to all results found.
        /// </remarks>
        [DataMember(Name = "latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string LatestTime
        { get; set; }

        /// <summary>
        /// The maximum lines that any single event's _raw field should contain. 
        /// </summary>
        /// <remarks>
        /// Specify zero to to indicate that all lines should be returned. The 
        /// default value is zero.
        /// </remarks>
        [DataMember(Name = "max_lines", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int MaxLines
        { get; set; }

        /// <summary>
        /// The first result (inclusive) from which to begin returning data.
        /// </summary>
        /// <remarks>
        /// The value of <c>Offset</c> is zero-based and cannot be 
        /// negative. The default value is zero.
        /// </remarks>
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int Offset
        { get; set; }

        /// <summary>
        /// Formats a UTC time.
        /// </summary>
        /// <remarks>
        /// The default value is specified in time_format.
        /// </remarks>
        [DataMember(Name = "output_time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string OutputTimeFormat
        { get; set; }

        /// <summary>
        /// The post processing search to apply to the results.
        /// </summary>
        /// <remarks>
        /// The post processing search string can be any Splunk command.
        /// </remarks>
        [DataMember(Name = "search", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Search
        { get; set; }

        /// <summary>
        /// The type of segmentation to perform on the data.
        /// </summary>
        /// <remarks>
        /// This incudes an option to perform k/v segmentation.
        /// </remarks>
        [DataMember(Name = "segmentation", EmitDefaultValue = false)]
        [DefaultValue("raw")]
        public string Segmentation
        { get; set; }

        /// <summary>
        /// Expression to convert a formatted time string from {start,end}_time
        /// into UTC seconds. 
        /// </summary>
        /// <remarks>
        /// The default value is <c>%m/%d/%Y:%H:%M:%S</c>.
        /// </remarks>
        [DataMember(Name = "time_format", EmitDefaultValue = false)]
        [DefaultValue("%m/%d/%Y:%H:%M:%S")]
        public string TimeFormat
        { get; set; }

        /// <summary>
        /// Expression to convert a formatted time string from {start,end}_time
        /// into UTC seconds. 
        /// </summary>
        /// <remarks>
        /// The default value is <c>%m/%d/%Y:%H:%M:%S</c>.
        /// </remarks>
        [DataMember(Name = "truncation_mode", EmitDefaultValue = false)]
        [DefaultValue(TruncationMode.Abstract)]
        public TruncationMode TruncationMode
        { get; set; }

        #endregion
    }
}