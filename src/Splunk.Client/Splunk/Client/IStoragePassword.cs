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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClass(typeof(IStoragePasswordContract))]
    public interface IStoragePassword : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets the plain text version of the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        string ClearPassword
        { get; }

        /// <summary>
        /// Gets the access control lists for the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets an encrypted version of the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        string EncryptedPassword
        { get; }

        /// <summary>
        /// Gets the masked version of the current <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        ///  This is always stored as <c>"********"</c>.
        /// </remarks>
        string Password
        { get; }

        /// <summary>
        /// Gets the realm in which the current <see cref="StoragePassword"/> 
        /// is valid.
        /// </summary>
        string Realm
        { get; }

        /// <summary>
        /// Gets the Splunk username associated with the current <see cref=
        /// "StoragePassword"/>. 
        /// </summary>
        string Username
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously updates the storage password represented by the
        /// current instance.
        /// </summary>
        /// <param name="password">
        /// New storage password.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestException">
        /// </exception>
        /// <exception cref="ResourceNotFoundException">
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/s0Bw7H">POST 
        /// storage/passwords/{name}</a> endpoint to update the storage 
        /// password represented by the current instance.
        /// </remarks>
        Task UpdateAsync(string password);

        #endregion
    }

    [ContractClassFor(typeof(IStoragePassword))]
    abstract class IStoragePasswordContract : IStoragePassword
    {
        #region Properties

        public abstract string ClearPassword { get; }
        public abstract Eai Eai { get; }
        public abstract string EncryptedPassword { get; }
        public abstract string Password { get; }
        public abstract string Realm { get; }
        public abstract string Username { get; }

        #endregion

        #region Methods

        public abstract Task GetAsync();

        public abstract Task RemoveAsync();
        
        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        public Task UpdateAsync(string password)
        {
            Contract.Requires<ArgumentNullException>(password != null);
            return default(Task);
        }

        #endregion
    }
}