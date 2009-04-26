using System;
using System.Collections.Generic;
using System.IO;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace CommandLineParser
{
	/// <summary>
	/// Allows more specific definition of additional arguments 
	/// (arguments after those with - and -- prefix).
	/// </summary>
	public class AdditionalArgumentsSettings
	{
		private string[] additionalArguments;

		/// <summary>
		/// Collection of additional arguments that were found on the command line after <see cref="CommandLineParser.ParseCommandLine"/> call.
		/// </summary>
		/// <exception cref="CommandLineException">Field accessed before <see cref="CommandLineParser.ParseCommandLine"/> was called or 
		/// when <see cref="AdditionalArguments"/> is set to false</exception>
		/// <seealso cref="AcceptAdditionalArguments"/>
		public string[] AdditionalArguments
		{
			get
			{
				if (acceptAdditionalArguments && additionalArguments != null)
				{
					return additionalArguments;
				}
				if (!acceptAdditionalArguments)
				{
					throw new CommandLineException(Messages.EXC_ADDITONAL_ARGS_FORBIDDEN);
				}
				throw new CommandLineException(Messages.EXC_ADDITIONAL_ARGS_TOO_EARLY);
			}
			set
			{
				additionalArguments = value;
			}
		}

		private int requestedAdditionalArgumentsCount = 0;

		/// <summary>
		/// Set RequestedAdditionalArgumentsCount to non-zero value if your application
		/// requires some additional arguments. 
		/// </summary>
		public int RequestedAdditionalArgumentsCount
		{
			get { return requestedAdditionalArgumentsCount; }
			set
			{
				if (value < 0)
					throw new InvalidDataException(Messages.EXC_NONNEGATIVE);
				requestedAdditionalArgumentsCount = value;
			}
		}

		private bool acceptAdditionalArguments = true;

		/// <summary>
		/// When set to true (default), additional arguments are stored in <see cref="AdditionalArguments"/> collection when found on the 
		/// command line. When set to false, Exception is thrown when additional arguments are found by <see cref="CommandLineParser.ParseCommandLine"/> call.
		/// </summary>
		public bool AcceptAdditionalArguments
		{
			get { return acceptAdditionalArguments; }
			set { acceptAdditionalArguments = value; }
		}

		private readonly List<IValueArgument> typedAdditionalArguments = new List<IValueArgument>();

		/// <summary>
		/// Allows requiring of arguments of certain type.
		/// </summary>
		public List<IValueArgument> TypedAdditionalArguments
		{
			get { return typedAdditionalArguments; }
		}

		/// <summary>
		/// Verifies the amount of arguments and fills them
		/// with typed values type.
		/// </summary>
		public void ProcessArguments()
		{
			if (AdditionalArguments.Length < TypedAdditionalArguments.Count)
			{
				throw new MissingAdditionalArgumentsException(string.Format(Messages.EXC_NOT_ENOUGH_ADDITIONAL_ARGUMENTS, TypedAdditionalArguments.Count));
			}

			for (int i = 0; i < TypedAdditionalArguments.Count; i++)
			{
				IValueArgument typedAdditionalArgument = TypedAdditionalArguments[i];

				typedAdditionalArgument.Value = typedAdditionalArgument.Convert_obj(AdditionalArguments[i]);
			}
		}
	}
}