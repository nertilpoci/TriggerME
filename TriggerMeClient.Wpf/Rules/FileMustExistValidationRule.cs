using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TriggerMeClient.Wpf.Rules
{
    public class FileMustExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            return File.Exists((value ?? "").ToString()) ? ValidationResult.ValidResult
                : new ValidationResult(false, "File doesn't exist");
           
        }

      
    }
}
