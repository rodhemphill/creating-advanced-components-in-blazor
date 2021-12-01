using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitcoinDayTrader.PersistenceFramework;
using Microsoft.AspNetCore.Components;
namespace BitcoinDayTrader.Components.Controls
{
    public class BaseControl : BaseComponent
    {
        [Parameter] public bool IsInline { get; set; } = true;
        [Parameter] public string Style { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public string InnerStyle { get; set; }
        [Parameter] public string InnerClass { get; set; }
        [Parameter] public string PlaceHolder { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public bool HideLabelsIfBlank { get; set; } = false;
        [Parameter] public bool HideErrorsIfBlank { get; set; } = true;
        [Parameter] public bool IsSuccess { get; set; }
        [Parameter] public bool Disabled { get; set; } = false;
        [Parameter] public string ValidationMessage { get; set; }
        [Parameter] public int TabIndex { get; set; } = 0;

        /// <summary>
        /// SVG based icons from UI design. Valid names provided in IconNames class. e.g. IconName="@IconNames.Search"
        /// </summary>
        [Parameter] public string IconName { get; set; }

        #region IsMaterial
        [Parameter] public bool IsMaterial { get; set; } = false;
        [Parameter] public bool DebugShowBoth { get; set; } = false;
        #endregion

        protected string _successClass => IsSuccess ? "success" : "";
        protected bool IsWarning { get; set; } = false;
        protected string _errorClass => string.IsNullOrEmpty(ValidationMessage) ? "" : IsWarning ? "m-field-warning" : "m-field-error";
        protected string _errorStyle => string.IsNullOrEmpty(ValidationMessage) ? "" : IsWarning ? "border-color: var(--color-warning);" : "border-color: var(--color-error);";
        protected string _inlineStyle => IsInline ? "" : "display: block;";


        #region Unexpected Errors
        private readonly Random _random = new Random();
        [Inject] public IServiceProvider _serviceProvider { get; set; }

        protected virtual void HandleUnexpectedError(Exception ex = null, string text = null)
        {
            var errorNumber = "ref-" + (_random.Next(999, 9999)).ToString();

            // Take screen print

            var control = this.GetType().Name;
            if (control.Substring(0,3).ToLower() == "mad")
                control = control.Substring(3);

            MadExceptionPanel.AdditionalInfoText = $"{control}: {ex?.Message}";
            MadExceptionPanel.ErrorNumber = errorNumber;
            MadExceptionPanel.StandardErrorText = $"This error has been reported. Your reference number is {errorNumber}";
            //MadExceptionPanel.ReferenceNumberText = $"Your reference number is {errorNumber}";

            StateHasChanged();
            MadExceptionPanel.IsVisible = true;

            var extraInfo = new Dictionary<string, object>();
            extraInfo.Add("ErrorNumber", errorNumber);
            extraInfo.Add("Control", control);

            ExceptionService.SilentHandle(ex, extraInfo, serviceProvider:_serviceProvider);
        }
        #endregion
    }
}