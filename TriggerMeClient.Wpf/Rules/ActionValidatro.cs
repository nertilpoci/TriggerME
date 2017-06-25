using FluentValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TriggerMeClient.Wpf.Models;

namespace TriggerMeClient.Wpf.Rules
{
    public class ActionValidator : AbstractValidator<TriggerAction>
    {
        public ActionValidator()
        {
            RuleFor(action => action.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(action => action.File).Must((file)=> { return File.Exists(file); }).WithMessage("File doesn't exist");

        }


    }
}
