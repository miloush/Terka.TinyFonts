namespace Terka.TinyFonts.TFConvert
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Base class for TFConvert commands.
    /// </summary>
    public abstract class TinyCommandBase
    {
        private readonly Type type;
        private string commandString;
        private bool? isGlobal;
        private SortedList<int, MemberInfo> parametersByIndex;
        private SortedList<string, MemberInfo> parametersByName;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public TinyCommandBase()
        {
            this.type = this.GetType();
        }

        /// <summary>
        /// Gets name of the command.
        /// </summary>
        public virtual string CommandString
        {
            get
            {
                if (this.commandString == null)
                {
                    foreach (TinyCommandAttribute attribute in this.type.GetCustomAttributes(typeof(TinyCommandAttribute), false))
                    {
                        this.commandString = attribute.CommandString;
                    }

                    if (this.commandString == null)
                    {
                        this.commandString = this.type.Name;
                    }
                }

                return this.commandString;
            }
        }

        /// <summary>
        /// Gets if this command is global.
        /// </summary>
        public virtual bool IsGlobal
        {
            get
            {
                if (!this.isGlobal.HasValue)
                {
                    foreach (TinyCommandAttribute attribute in this.type.GetCustomAttributes(typeof(TinyCommandAttribute), true))
                    {
                        this.isGlobal |= attribute.IsGlobal;
                    }
                }

                return this.isGlobal.Value;
            }
        }

        /// <summary>
        /// Gets count of parameters associated with this command.
        /// </summary>
        public int ParametersCount
        {
            get
            {
                if (this.parametersByIndex == null)
                {
                    this.PopulateParameters();
                }

                return this.parametersByIndex.Count;
            }
        }
        
        /// <summary>
        /// Parameters indexer.
        /// </summary>
        /// <param name="parameterIndex">Zero-based index of parameter.</param>
        /// <returns>Parameter.</returns>
        public virtual object this[int parameterIndex]
        {
            get
            {
                if (this.parametersByIndex == null)
                {
                    this.PopulateParameters();
                }

                return this.GetParameter(this.parametersByIndex[parameterIndex]);
            }

            set
            {
                if (this.parametersByIndex == null)
                {
                    this.PopulateParameters();
                }

                this.SetParameter(this.parametersByIndex[parameterIndex], value);
            }                     
        }
        
        /// <summary>
        /// Parameters indexer.
        /// </summary>
        /// <param name="parameterName">Name of the parameter to find.</param>
        /// <returns></returns>
        public virtual object this[string parameterName]
        {
            get
            {
                if (this.parametersByName == null)
                {
                    this.PopulateParameters();
                }

                return this.GetParameter(this.parametersByName[parameterName]);
            }

            set
            {
                if (this.parametersByName == null)
                {
                    this.PopulateParameters();
                }

                this.SetParameter(this.parametersByName[parameterName], value);
            }
        }
        
        /// <summary>
        /// Sets parameters to values.
        /// </summary>
        /// <param name="parameters">Parameters to set.</param>
        public void SetParameters(params object[] parameters)
        {
            if (this.ValidateParameters(parameters))
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    this[i] = parameters[i];
                }
            }
        }
        
        /// <summary>
        /// Parses parameters and sets parsed values.
        /// </summary>
        /// <param name="parameters">Parameters to parse.</param>
        public void ParseParameters(params string[] parameters)
        {
            if (this.ValidateParameters(parameters))
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    this[i] = this.ParseParameter(parameters[i], i);
                }
            }
        }

        /// <summary>
        /// Parses range of parameters.
        /// </summary>
        /// <param name="parameters">Parameters to parse.</param>
        /// <param name="startIndex">Zero-based index to parameters.</param>
        /// <param name="length">How many parameters from <paramref name="startIndex"/> use.</param>
        public void ParseParameters(string[] parameters, int startIndex, int length)
        {
            if (this.ValidateParameters(parameters, startIndex, length))
            {
                for (int i = 0; i < length; i++)
                {
                    this[i] = this.ParseParameter(parameters[i + startIndex], i);
                }
            }
        }
        
        /// <summary>
        /// String representation of the command.
        /// </summary>
        /// <returns>String representation of the command.</returns>
        public override string ToString()
        {
            if (this.parametersByIndex == null)
            {
                this.PopulateParameters();
            }

            StringBuilder paramBuilder = new StringBuilder();
            for (int i = 0; i < this.parametersByIndex.Count; i++)
            {
                MemberInfo mi = this.parametersByIndex.Values[i];
                TinyParameterAttribute attribute = (TinyParameterAttribute)mi.GetCustomAttributes(typeof(TinyParameterAttribute), false)[0];

                paramBuilder.Append(" " + attribute.FormatParameter(this[i]));
            }

            return this.CommandString + paramBuilder;
        }

        private void PopulateParameters()
        {
            this.parametersByIndex = new SortedList<int, MemberInfo>();
            this.parametersByName = new SortedList<string, MemberInfo>();

            this.type.FindMembers(
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Public | BindingFlags.Instance,
                delegate(MemberInfo info, object criteria)
                {
                    object[] tinyAttributes = info.GetCustomAttributes(typeof(TinyParameterAttribute), false);
                    if (tinyAttributes.Length > 0)
                    {
                        this.parametersByIndex.Add(((TinyParameterAttribute)tinyAttributes[0]).Position, info);
                        this.parametersByName.Add(info.Name, info);
                    }

                    return false;
                },
                null);
        }
       
        private object GetParameter(MemberInfo info)
        {
            if (info is FieldInfo)
            {
                return ((FieldInfo)info).GetValue(this);
            }

            if (info is PropertyInfo)
            {
                return ((PropertyInfo)info).GetValue(this, null);
            }

            return null;
        }
        
        private Type GetParameterType(MemberInfo info)
        {
            if (info is FieldInfo)
            {
                return ((FieldInfo)info).FieldType;
            }

            if (info is PropertyInfo)
            {
                return ((PropertyInfo)info).PropertyType;
            }

            return null;
        }

        private TinyParameterAttribute GetParameterAttribute(MemberInfo info)
        {
            return (TinyParameterAttribute)info.GetCustomAttributes(typeof(TinyParameterAttribute), false)[0];
        }

        private void SetParameter(MemberInfo info, object value)
        {
            if (info is FieldInfo)
            {
                ((FieldInfo)info).SetValue(this, value);
            }

            if (info is PropertyInfo)
            {
                ((PropertyInfo)info).SetValue(this, value, null);
            }
        }
        
        private object ParseParameter(string parameter, int i)
        {
            MemberInfo info = this.parametersByIndex[i];

            Type type = this.GetParameterType(info);
            TinyParameterAttribute attribute = this.GetParameterAttribute(info);

            parameter = attribute.UnescapeParameter(parameter);
            
            if (type.IsAssignableFrom(typeof(string)))
            {
                return parameter;
            }
            else
            {
                if (type.IsEnum)
                {
                    return Enum.Parse(type, parameter);
                }
                else
                {
                    return Convert.ChangeType(parameter, type);
                }
            }
        }
        
        private bool ValidateParameters(Array parameters, int? startIndex = null, int? length = null)
        {
            int requiredCount = this.ParametersCount;

            if (parameters == null)
            {
                if (requiredCount == 0)
                {
                    return false;
                }

                throw new ArgumentNullException("parameters");
            }

            if (startIndex == null)
            {
                startIndex = 0;
            }

            if (length == null)
            {
                length = parameters.Length;
            }

            if (parameters.Length < startIndex)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            
            if (parameters.Length < startIndex + length)
            {
                throw new ArgumentException();
            }
            
            if (requiredCount != length)
            {
                throw new ArgumentException();
            }

            return true;
        }
    }
}
