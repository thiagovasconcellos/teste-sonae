using TestMcSonae.DTOs;

namespace TestMcSonae.Validation.Validators
{
    public class CreateOrderValidator
    {
        public ValidationResult Validate(CreateOrderDTO createOrderDTO)
        {
            if (createOrderDTO?.Items == null || !createOrderDTO.Items.Any())
            {
                return ValidationResult.Failure("Order must contain at least one item");
            }

            foreach (var item in createOrderDTO.Items)
            {
                if (item.ProductId == Guid.Empty)
                {
                    return ValidationResult.Failure("Product ID cannot be empty");
                }

                if (item.Quantity <= 0)
                {
                    return ValidationResult.Failure("Quantity must be greater than zero");
                }
            }

            return ValidationResult.Success();
        }
    }
}