using System.IO;

namespace CommandLineParser.Arguments
{
	/// <summary>
	/// Value of the argument is a directory in the directory system
	/// </summary>
	public class DirectoryArgument: CertifiedValueArgument<DirectoryInfo>
	{
		#region constructor

		/// <summary>
		/// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>.
		/// </summary>
		/// <param name="shortName">Short name of the argument</param>
		public DirectoryArgument(char shortName) : base(shortName) { }

	    /// <summary>
	    /// Creates new certified value argument with a <see cref="Argument.LongName">long name</see>.
	    /// </summary>
	    /// <param name="longName">Long name of the argument</param>
	    public DirectoryArgument(string longName) : base(longName) { }

	    /// <summary>
		/// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>and <see cref="Argument.LongName">long name</see>.
		/// </summary>
		/// <param name="shortName">Short name of the argument</param>
		/// <param name="longName">Long name of the argument </param>
		public DirectoryArgument(char shortName, string longName) : base(shortName, longName) { }

	    /// <summary>
        /// Creates new certified value argument with a <see cref="Argument.ShortName">short name</see>,
        /// <see cref="Argument.LongName">long name</see> and <see cref="Argument.Description">description</see>
        /// </summary>
        /// <param name="shortName">Short name of the argument</param>
        /// <param name="longName">Long name of the argument </param>
        /// <param name="description">description of the argument</param>
        public DirectoryArgument(char shortName, string longName, string description) : base(shortName, longName, description) { }
		#endregion 
	
		private bool _directoryMustExist = true;

		/// <summary>
		/// Gets or sets a value indicating whether the directory must
		/// already exists in the file system or not .
		/// Default is true.
		/// </summary>
		public bool DirectoryMustExist
		{
			get { return _directoryMustExist; }
			set { _directoryMustExist = value; }
		}

		/// <summary>
		/// DirectoryInfo for the directory passed as argument.
		/// </summary>
		public DirectoryInfo DirectoryInfo
		{
			get
			{
				return Value;
			}
		}

		/// <summary>
		/// Converts <paramref name="stringValue"/> to <see cref="DirectoryInfo"/>
		/// </summary>
		/// <param name="stringValue">string representing the value</param>
		/// <returns>value as <see cref="DirectoryInfo"/></returns>
		public override DirectoryInfo Convert(string stringValue)
		{
			return new DirectoryInfo(stringValue);
		}

        /// <summary>
        /// Checks whether directory exists in the file system
        /// </summary>
        /// <param name="value">value to certify - directory path</param>
        protected override void Certify(DirectoryInfo value)
		{
			if (DirectoryMustExist && !value.Exists)
			{
				throw new DirectoryNotFoundException(string.Format(Messages.EXC_DIR_NOT_FOUND, value.Name));
			}
		}
	}

	/// <summary>
	/// <para>
	/// Attribute for declaring a class' field a <see cref="DirectoryArgumentAttribute"/> and 
	/// thus binding a field's value to a certain command line switch argument.
	/// </para>
	/// <para>
	/// Instead of creating an argument explicitly, you can assign a class' field an argument
	/// attribute and let the CommandLineParse take care of binding the attribute to the field.
	/// </para>
	/// </summary>
	/// <remarks>Appliable to fields and properties (public).</remarks>
	/// <remarks>Use <see cref="CommandLineParser.ExtractArgumentAttributes"/> for each object 
	/// you where you have delcared argument attributes.</remarks>
	public sealed class DirectoryArgumentAttribute : ArgumentAttribute
	{
		/// <summary>
		/// Creates new instance of DirectoryArgumentAttribute. DirectoryArgumentAttribute
		/// uses underlying <see cref="DirectoryArgument"/>.
		/// </summary>
		/// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
		public DirectoryArgumentAttribute(char shortName) : base(typeof(DirectoryArgument), shortName) { }

        /// <summary>
        /// Creates new instance of DirectoryArgumentAttribute. DirectoryArgumentAttribute
        /// uses underlying <see cref="DirectoryArgument"/>.
        /// </summary>
        /// <param name="longName"><see cref="Argument.LongName">short name</see> of the underlying argument</param>
        public DirectoryArgumentAttribute(string longName) : base(typeof(DirectoryArgument), longName) { }

		/// <summary>
		/// Creates new instance of DirectoryArgumentAttribute. DirectoryArgumentAttribute
		/// uses underlying <see cref="DirectoryArgument"/>.
		/// </summary>
		/// <param name="shortName"><see cref="Argument.ShortName">short name</see> of the underlying argument</param>
		/// <param name="longName"><see cref="Argument.LongName">long name</see> of the underlying argument</param>
		public DirectoryArgumentAttribute(char shortName, string longName) : base(typeof(DirectoryArgument), shortName, longName) { }

		/// <summary>
		/// Gets or sets a value indicating whether the directory must
		/// already exists in the file system (input file) or not (output file).
		/// Default is true.
		/// </summary>
		public bool DirectoryMustExist
		{
			get { return ((DirectoryArgument)Argument).DirectoryMustExist; }
			set { ((DirectoryArgument)Argument).DirectoryMustExist = value; }
		}

        /// <summary>
        /// Default value
        /// </summary>
        public DirectoryInfo DefaultValue
        {
            get
            {
                return ((DirectoryArgument)Argument).DefaultValue;
            }
            set
            {
                ((DirectoryArgument)Argument).DefaultValue = value;
            }
        }
	}
}