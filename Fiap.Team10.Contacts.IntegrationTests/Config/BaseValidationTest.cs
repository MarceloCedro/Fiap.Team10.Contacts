using System.ComponentModel.DataAnnotations;

namespace Fiap.Team10.Contacts.IntegrationTests.Config;

public abstract class BaseValidationTest
{
    public static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, context, validationResults, true);
        return validationResults;
    }
}
