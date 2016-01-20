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
        private string[] _additionalArguments;

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
                if (_acceptAdditionalArguments && _additionalArguments != null)
                {
                    return _additionalArguments;
                }
                if (!_acceptAdditionalArguments)
                {
                    throw new CommandLineException(Messages.EXC_ADDITONAL_ARGS_FORBIDDEN);
                }
                throw new CommandLineException(Messages.EXC_ADDITIONAL_ARGS_TOO_EARLY);
            }
            set
            {
                _additionalArguments = value;
            }
        }

        private int _requestedAdditionalArgumentsCount;

        /// <summary>
        /// Set RequestedAdditionalArgumentsCount to non-zero value if your application
        /// requires some additional arguments. 
        /// </summary>
        public int RequestedAdditionalArgumentsCount
        {
            get { return _requestedAdditionalArgumentsCount; }
            set
            {
                if (value < 0)
                    throw new InvalidDataException(Messages.EXC_NONNEGATIVE);
                _requestedAdditionalArgumentsCount = value;
            }
        }

        private bool _acceptAdditionalArguments = true;

        /// <summary>
        /// When set to true (default), additional arguments are stored in <see cref="AdditionalArguments"/> collection when found on the 
        /// command line. When set to false, Exception is thrown when additional arguments are found by <see cref="CommandLineParser.ParseCommandLine"/> call.
        /// </summary>
        public bool AcceptAdditionalArguments
        {
            get { return _acceptAdditionalArguments; }
            set { _acceptAdditionalArguments = value; }
        }

        private readonly List<IValueArgument> _typedAdditionalArguments = new List<IValueArgument>();

        /// <summary>
        /// Allows requiring of arguments of certain type.
        /// </summary>
        public List<IValueArgument> TypedAdditionalArguments
        {
            get { return _typedAdditionalArguments; }
        }

        /// <summary>
        /// Verifies the amount of arguments and fills them
        /// with typed values type.
        /// </summary>
        public void ProcessArguments()
        {
            int optionals = 0;
            int multiples = 0;
            foreach (IValueArgument typedAdditionalArgument in TypedAdditionalArguments)
            {
                if (typedAdditionalArgument.Optional)
                {
                    optionals++;
                }
                if (typedAdditionalArgument.AllowMultiple)
                {
                    multiples++;
                }
            }

            if (optionals > 1 || multiples > 1)
            {
                throw new CommandLineException("If Optional or AllowMultiple flags are set for additional argument, there can only be one additional argument. ");
            }

            if (optionals == 1 && TypedAdditionalArguments.Count > 1)
            {
                throw new CommandLineException("If Optional or AllowMultiple flags are set for additional argument, there can only be one additional argument. ");
            }

            if (multiples == 1 && TypedAdditionalArguments.Count > 1)
            {
                throw new CommandLineException("If Optional or AllowMultiple flags are set for additional argument, there can only be one additional argument. ");
            }

            if (AdditionalArguments.Length < TypedAdditionalArguments.Count)
            {
                if (TypedAdditionalArguments.Count == 1 && TypedAdditionalArguments[0].Optional)
                {

                }
                else
                {
                    throw new MissingAdditionalArgumentsException(string.Format(Messages.EXC_NOT_ENOUGH_ADDITIONAL_ARGUMENTS, TypedAdditionalArguments.Count));
                }
            }

            int i;

            for (i = 0; i < Math.Min(TypedAdditionalArguments.Count, AdditionalArguments.Length); i++)
            {
                IValueArgument typedAdditionalArgument = TypedAdditionalArguments[i];

                if (typedAdditionalArgument.AllowMultiple)
                {
                    typedAdditionalArgument.AddToValues(typedAdditionalArgument.Convert_obj(AdditionalArguments[i]));
                }
                else
                {
                    typedAdditionalArgument.Value = typedAdditionalArgument.Convert_obj(AdditionalArguments[i]);
                }
            }


            if (TypedAdditionalArguments.Count == 1)
            {
                IValueArgument valueArgument = TypedAdditionalArguments[TypedAdditionalArguments.Count - 1];
                if (i < AdditionalArguments.Length && valueArgument.AllowMultiple)
                {
                    for (; i < AdditionalArguments.Length; i++)
                    {
                        TypedAdditionalArguments[0].AddToValues(TypedAdditionalArguments[0].Convert_obj(AdditionalArguments[i]));
                    }
                }
            }

        }
    }
}