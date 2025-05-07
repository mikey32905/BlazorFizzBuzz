using BlazorFizzBuzz.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorFizzBuzz.Components.Validators
{
    public class FizzBuzzValidator : ComponentBase
    {
        private ValidationMessageStore validationMessageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FizzBuzzValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }
            //else
            //{
            validationMessageStore = new ValidationMessageStore(CurrentEditContext);

            //Attach methods to validation events.
            CurrentEditContext.OnFieldChanged += (s, e) => ValidateField(e.FieldIdentifier);
            CurrentEditContext.OnValidationRequested += (s, e) => ValidateAllFields();

            //}

        }

        private void ValidateField(FieldIdentifier fieldIdentifier)
        {
            var fizzbuzz = CurrentEditContext.Model as FizzBuzzModel ??
                throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires a model of type FizzBuzz.");

            //Clear previous errors for the field.
            validationMessageStore.Clear(fieldIdentifier);

            //Validate the field.
            if (fieldIdentifier.FieldName == nameof(fizzbuzz.FizzValue))
            {
                if (fizzbuzz.FizzValue >= fizzbuzz.BuzzValue)
                {
                    validationMessageStore.Add(fieldIdentifier,
                        "The Fizz Value must be less than the Buzz value. ");
                }
            }
            else if (fieldIdentifier.FieldName == nameof(fizzbuzz.BuzzValue))
            {
                if (fizzbuzz.BuzzValue <= fizzbuzz.FizzValue)
                {
                    validationMessageStore.Add(fieldIdentifier,
                        "The Buzz Value must be greater than the Fizz value. ");
                }
            }
            else if (fieldIdentifier.FieldName == nameof(fizzbuzz.StopValue))
            {
                int requiredStopValue = fizzbuzz.FizzValue * fizzbuzz.BuzzValue;

                if (fizzbuzz.StopValue < requiredStopValue)
                {
                    validationMessageStore.Add(fieldIdentifier,
                        $"The Stop Value must be greater than or equal to {requiredStopValue}. ");
                }
            }
        }

        private void ValidateAllFields()
        {
            var fizzbuzz = CurrentEditContext.Model as FizzBuzzModel ??
                     throw new InvalidOperationException($"{nameof(FizzBuzzValidator)} requires a model of type FizzBuzz.");


            //Clear all previous errors.
            validationMessageStore.Clear();

            //Validate all fields.
            ValidateField(new FieldIdentifier(fizzbuzz, nameof(FizzBuzzModel.FizzValue)));
            ValidateField(new FieldIdentifier(fizzbuzz, nameof(FizzBuzzModel.BuzzValue)));
            ValidateField(new FieldIdentifier(fizzbuzz, nameof(FizzBuzzModel.StopValue)));

            //Notify the EditContext that validation has changed.
            CurrentEditContext.NotifyValidationStateChanged();
        }

    }
}
