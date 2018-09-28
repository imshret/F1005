using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace F1005.Models
{
    public class LoginViewModelMetadatas
    {
    }

    [MetadataType(typeof(LoginViewModelMetadatas))]
    public partial class LoginViewModel : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password == "1")
            {
                yield return new ValidationResult("密碼不可", new string[] { "Password" });
            }
        }
    }
}