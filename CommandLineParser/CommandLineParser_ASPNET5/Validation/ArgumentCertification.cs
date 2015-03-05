using System;
using System.Collections.Generic;
using CommandLineParser.Arguments;

namespace CommandLineParser.Validation
{
    /// <summary>
    /// Certification object can be used to define more complex conditions for 
    /// arguments. Certifications are checked after command line is parsed and 
    /// the Certify method is supposed to throw an exception when the arguments
    /// and their values do not meet the do condition.
    /// </summary>
    /// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/Certification/*'/>
    public abstract class ArgumentCertification
    {
        /// <summary>
        /// Test the parser for the certification. This method should throw an exception if the condition
        /// is not met. 
        /// </summary>
        /// <param name="parser">parser object gives access to the defined arguments, their values and 
        /// parameters of the parser</param>
        public abstract void Certify(CommandLineParser parser);
        
        /// <summary>
        /// Returns description of the certification.
        /// </summary>
        public virtual string GetDescription
        {
            get { return this.ToString(); }
        }
    }

    /// <summary>
    /// Use ArgumentCertificationAttribute to define <see cref="ArgumentCertification"/>s declaratively. 
    /// </summary>
	/// <include file='Doc\CommandLineParser.xml' path='CommandLineParser/Certifications/CertificationAttribute/*'/>
    public abstract class ArgumentCertificationAttribute: System.Attribute
    {
        private ArgumentCertification certification;

        /// <summary>
        /// Underlying <see cref="ArgumentCertification"/> object.
        /// </summary>
        public ArgumentCertification Certification
        {
            get { return certification; }
        }

        /// <summary>
        /// Creates new instance of ArgumentCertificationAttribute. 
        /// </summary>
        /// <param name="underlyingCertificationType">type of the certification</param>
        /// <param name="constructorParams">parameters of the 
        /// <paramref name="underlyingCertificationType">certification</paramref>constructor</param>
        protected ArgumentCertificationAttribute(Type underlyingCertificationType, params object[] constructorParams)
        {
            if (!underlyingCertificationType.IsSubclassOf(typeof(ArgumentCertification)))
            {
                throw new InvalidOperationException("Parameter underlyingArgumentType must be a subclass of ArgumentCertification.");
            }
            //create certification object of proper type using reflection
            certification = (ArgumentCertification)Activator.CreateInstance(underlyingCertificationType, constructorParams);
        }
    }

}