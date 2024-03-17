using Core.ResourcesFiles;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web2PnK.Helpers.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class UniqueValueAttribute : ValidationAttribute
	{
		private readonly Type _serviceType;
		private readonly string _methodName;

		public UniqueValueAttribute(Type serviceType, string methodName)
		{
			_serviceType = serviceType;
			_methodName = methodName;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			if (value == null)
				return new ValidationResult(string.Format(DefaultLanguage.ValueIsRequired, validationContext.MemberName));

			var service = validationContext.GetService(_serviceType);
			if (service == null)
				return new ValidationResult($"Service '{_serviceType.Name}' not found.");

			var methodInfo = _serviceType.GetMethod(_methodName, BindingFlags.Public | BindingFlags.Instance);
			if (methodInfo == null)
				return new ValidationResult($"Method '{_methodName}' not found in {_serviceType.Name}.");

			try
			{
				var result = (bool)methodInfo.Invoke(service, new object[] { value! })!;

				if (result)
				{
					return new ValidationResult(string.Format(DefaultLanguage.ValueIsTaken, value, validationContext.MemberName));

                }
			}
			catch (Exception ex)
			{
				return new ValidationResult($"Error validating uniqueness: {ex.Message}");
			}

			return ValidationResult.Success;
		}
	}
}
