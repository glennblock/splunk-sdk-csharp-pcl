/*
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

// TODO: Ensure this code is solid
// [O] Documentation
// [X] Respect DataMemberAttribute.Order
// [X] Do not serialize default values => define default values and check for them
// [X] Rework this into a real parameter-passing class, not just a ToString implementation tool (toString shows all parameterts; args are passed as parameters by way of GetEnumerator)
// [X] Work on nomenclature (serialization nomenclature is not necessarily appropriate)
// [X] Ensure this class works with nullable types.
// [ ] Support more than one level of inheritance => move away from generic implementation.
// [ ] More (?)

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public abstract class Args<TArgs> : IEnumerable<Argument> where TArgs : Args<TArgs>
    {
        #region Constructors

        static Args()
        {
            var propertyFormatters = new Dictionary<Type, Formatter>()
            {
                { typeof(bool),     new Formatter { Format = FormatBoolean } },
                { typeof(bool?),    new Formatter { Format = FormatBoolean } },
                { typeof(byte),     new Formatter { Format = FormatNumber  } },
                { typeof(byte?),    new Formatter { Format = FormatNumber  } },
                { typeof(sbyte),    new Formatter { Format = FormatNumber  } },
                { typeof(sbyte?),   new Formatter { Format = FormatNumber  } },
                { typeof(short),    new Formatter { Format = FormatNumber  } },
                { typeof(short?),   new Formatter { Format = FormatNumber  } },
                { typeof(ushort),   new Formatter { Format = FormatNumber  } },
                { typeof(ushort?),  new Formatter { Format = FormatNumber  } },
                { typeof(int),      new Formatter { Format = FormatNumber  } },
                { typeof(int?),     new Formatter { Format = FormatNumber  } },
                { typeof(uint),     new Formatter { Format = FormatNumber  } },
                { typeof(uint?),    new Formatter { Format = FormatNumber  } },
                { typeof(long),     new Formatter { Format = FormatNumber  } },
                { typeof(long?),    new Formatter { Format = FormatNumber  } },
                { typeof(ulong),    new Formatter { Format = FormatNumber  } },
                { typeof(ulong?),   new Formatter { Format = FormatNumber  } },
                { typeof(float),    new Formatter { Format = FormatNumber  } },
                { typeof(float?),   new Formatter { Format = FormatNumber  } },
                { typeof(double),   new Formatter { Format = FormatNumber  } },
                { typeof(double?),  new Formatter { Format = FormatNumber  } },
                { typeof(decimal),  new Formatter { Format = FormatNumber  } },
                { typeof(decimal?), new Formatter { Format = FormatNumber  } },
                { typeof(string),   new Formatter { Format = FormatString  } }
            };

            var defaultFormatter = new Formatter { Format = FormatString };
            var parameters = new SortedSet<Parameter>();

            foreach (PropertyInfo propertyInfo in typeof(TArgs).GetRuntimeProperties())
            {
                var dataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember == null)
                {
                    throw new InvalidDataContractException(string.Format("Missing DataMemberAttribute on {0}.{1}", propertyInfo.PropertyType.Name, propertyInfo.Name));
                }

                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;
                var propertyTypeInfo = propertyType.GetTypeInfo();

                Formatter? formatter = GetPropertyFormatter(propertyName, null, propertyType, propertyTypeInfo, propertyFormatters);

                if (formatter == null)
                {
                    var interfaces = propertyType.GetTypeInfo().ImplementedInterfaces;
                    var isCollection = false;

                    formatter = defaultFormatter;

                    foreach (Type @interface in interfaces)
                    {
                        if (@interface.IsConstructedGenericType)
                        {
                            if (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                            {
                                Type itemType = @interface.GenericTypeArguments[0];
                                formatter = GetPropertyFormatter(propertyName, propertyType, itemType, itemType.GetTypeInfo(), propertyFormatters);
                            }
                        }
                        else if (@interface == typeof(IEnumerable))
                        {
                            isCollection = true;
                        }
                    }

                    formatter = new Formatter { Format = formatter.Value.Format, IsCollection = isCollection };
                }

                var defaultValue = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();

                parameters.Add(new Parameter(dataMember.Name, dataMember.Order, propertyInfo)
                {
                    // Properties

                    DefaultValue = defaultValue == null ? null : defaultValue.Value,
                    EmitDefaultValue = dataMember.EmitDefaultValue,
                    IsCollection = formatter.Value.IsCollection,
                    IsRequired = dataMember.IsRequired,

                    // Methods

                    Format = (formatter ?? defaultFormatter).Format
                });
            }

            Parameters = parameters;
        }

        protected Args()
        {
            foreach (var serializationEntry in Parameters.Where(entry => entry.DefaultValue != null))
            {
                serializationEntry.SetValue(this, serializationEntry.DefaultValue);
            }
        }

        #endregion

        #region Fields

        public static readonly IEnumerable<Argument> Empty = Enumerable.Empty<Argument>();

        #endregion

        #region Methods

        public IEnumerator<Argument> GetEnumerator()
        {
            foreach (var parameter in Args<TArgs>.Parameters)
            {
                object value = parameter.GetValue(this);

                if (value == null)
                {
                    if (parameter.IsRequired)
                    {
                        throw new SerializationException(string.Format("Missing value for required parameter {0}", parameter.Name));
                    }
                    continue;
                }

                if (!parameter.EmitDefaultValue && value.Equals(parameter.DefaultValue))
                {
                    continue;
                }
                
                if (!parameter.IsCollection)
                {
                    yield return new Argument(parameter.Name, parameter.Format(value));
                    continue;
                }
                
                foreach (var item in (IEnumerable)value)
                {
                    yield return new Argument(parameter.Name, parameter.Format(item));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            Action<object, string, Func<object, string>> append = (item, name, format) =>
            {
                builder.Append(name);
                builder.Append('=');
                builder.Append(format(item));
                builder.Append("; ");
            };

            foreach (var parameter in Args<TArgs>.Parameters)
            {
                object value = parameter.GetValue(this);

                if (value == null)
                {
                    append("null", parameter.Name, FormatString);
                    continue;
                }
                if (!parameter.IsCollection)
                {
                    append(value, parameter.Name, parameter.Format);
                    continue;
                }
                foreach (var item in (IEnumerable)value)
                {
                    append(item, parameter.Name, parameter.Format);
                }
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 2;
            }

            return builder.ToString();
        }

        #endregion

        #region Privates

        static readonly SortedSet<Parameter> Parameters;

        static string FormatBoolean(object value)
        {
            return (bool)value ? "t" : "f";
        }

        static string FormatNumber(object value)
        {
            return value.ToString();
        }

        static string FormatString(object value)
        {
            return value.ToString();
        }

        static Formatter? GetPropertyFormatter(string propertyName, Type container, Type type, TypeInfo info, Dictionary<Type, Formatter> formatters)
        {
            Formatter formatter;

            if (formatters.TryGetValue(container ?? type, out formatter))
            {
                return formatter;
            }

            if (container != null && formatters.TryGetValue(type, out formatter))
            {
                formatter = new Formatter { Format = formatter.Format, IsCollection = true };
            }
            else if (info.IsEnum)
            {
                var map = new Dictionary<int, string>();

                foreach (var value in Enum.GetValues(type))
                {
                    string name = Enum.GetName(type, value);
                    FieldInfo field = type.GetRuntimeField(name);
                    var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();

                    map[(int)value] = enumMember == null ? name : enumMember.Value;
                }

                formatter = new Formatter { 
                    Format = (object value) => 
                    {
                        string name;

                        if (map.TryGetValue((int)value, out name))
                        {
                            return name;
                        }
                        throw new ArgumentException(string.Format("{0}.{1}: {2}", typeof(TArgs).Name, propertyName, value));
                    },
                    IsCollection = container != null
                };
            }
            else if (container != null)
            {
                formatter = new Formatter { Format = FormatString, IsCollection = true };
            }
            else
            {
                return null;
            }

            formatters.Add(container ?? type, formatter);
            return formatter;
        }

        #endregion

        #region Types
        
        struct Formatter
        {
            public Func<object, string> Format;
            public bool IsCollection;
        }

        struct Ordinal : IComparable<Ordinal>, IEquatable<Ordinal>
        {
            #region Constructos

            public Ordinal(int position, string name)
            {
                this.Position = position;
                this.Name = name;
            }

            #endregion

            #region Fields

            public readonly int Position;
            public readonly string Name;

            #endregion

            #region Methods

            public int CompareTo(Ordinal other)
            {
                int result = this.Position - other.Position;
                return result != 0 ? result : this.Name.CompareTo(other.Name);
            }

            public override bool Equals(object o)
            {
                return o != null && o is Ordinal ? this.Equals((Ordinal)o) : false;
            }

            public bool Equals(Ordinal other)
            {
                return this.Position == other.Position && this.Name == other.Name;
            }

            public override int GetHashCode()
            {
                // TODO: Check this against the algorithm presented in Effective Java
                int hash = 17;

                hash = (hash * 23) + this.Position.GetHashCode();
                hash = (hash * 23) + this.Name.GetHashCode();

                return hash;
            }

            public override string ToString()
            {
                return string.Format("({0}, {1})", this.Position, this.Name);
            }

            #endregion
        }

        class Parameter : IComparable, IComparable<Parameter>, IEquatable<Parameter>
        {
            #region Constructors

            public Parameter(string name, int position, PropertyInfo propertyInfo)
            {
                this.ordinal = new Ordinal(position, name);
                this.propertyInfo = propertyInfo;
            }

            #endregion

            #region Properties

            public object DefaultValue
            { get; set; }

            public bool EmitDefaultValue
            { get; set; }

            public bool IsCollection
            { get; set; }

            public bool IsRequired
            { get; set; }

            public string Name
            {
                get { return this.ordinal.Name; }
            }

            public int Position
            {
                get { return this.ordinal.Position; }
            }

            #endregion

            #region Methods

            public int CompareTo(object other)
            {
                return this.CompareTo(other as Parameter);
            }

            public int CompareTo(Parameter other)
            {
                if (other == null)
                    return 1;
                if (object.ReferenceEquals(this, other))
                    return 0;
                return this.ordinal.CompareTo(other.ordinal);
            }

            public override bool Equals(object other)
            {
                return this.Equals(other as Parameter);
            }

            public bool Equals(Parameter other)
            {
                if (other == null)
                {
                    return false;
                }
                return object.ReferenceEquals(this, other) || this.ordinal.Equals(other.ordinal);
            }

            public Func<object, string> Format
            { get; set; }

            public override int GetHashCode()
            {
                return this.ordinal.GetHashCode();
            }

            public object GetValue(object o)
            {
                return this.propertyInfo.GetValue(o);
            }

            public void SetValue(object o, object value)
            {
                this.propertyInfo.SetValue(o, value);
            }

            #endregion

            #region Privates

            PropertyInfo propertyInfo;
            Ordinal ordinal;

            #endregion
        }

        #endregion
    }
}
