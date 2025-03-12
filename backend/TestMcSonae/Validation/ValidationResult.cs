namespace TestMcSonae.Validation
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; } = new List<string>();

        public ValidationResult()
        {
        }

        public ValidationResult(string error)
        {
            Errors.Add(error);
        }

        public ValidationResult(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }

        public static ValidationResult Success()
        {
            return new ValidationResult();
        }

        public static ValidationResult Failure(string error)
        {
            return new ValidationResult(error);
        }

        public static ValidationResult Failure(IEnumerable<string> errors)
        {
            return new ValidationResult(errors);
        }
    }
}