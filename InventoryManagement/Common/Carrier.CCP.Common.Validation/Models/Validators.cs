namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     Validators model to represent the dataenhancers
    /// </summary>
    public class Validators
    {
        public ValidatorsValidator[] Items { get; set; }
    }


    public class ValidatorsValidator
    {
        public ValidatorsValidatorProperty[] Property { get; set; }
        public string Type { get; set; }
    }

    public class ValidatorsValidatorProperty
    {
        public ValidatorsValidatorProperty()
        {
            Required = false;
        }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string ErrorMessage { get; set; }

        public string RegularExpression { get; set; }

        public string MinValue { get; set; }

        public string MaxValue { get; set; }
    }
}