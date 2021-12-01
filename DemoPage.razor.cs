
using BitcoinDayTrader.ModelServices;
using BitcoinDayTrader.Models;
using Microsoft.AspNetCore.Components;
using BitcoinDayTrader.PersistenceFramework;
using BitcoinDayTrader.PersistenceFramework.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.JSInterop;
using System.Diagnostics;

namespace BitcoinDayTrader.Pages
{
    partial class DemoPage : BasePage
    {
        protected string DemoField1 { get; set; }
        protected string DemoField2 { get; set; }
        protected string DemoField3 { get; set; }
        protected string DemoField4 { get; set; }

        protected string Label1 { get; set; }
        protected string Label2 { get; set; }
        protected string Label3 { get; set; }
 
        protected async Task Field1Focus()
        {
            CheckCalculations();
        }

        protected async Task Field1FocusOut()
        {
            Calculate();
        }

    
        protected async Task Field1OnInput()
        {
            SplitFields();
         }


        public MarketData MarketData { get; set; }
        public bool SavedAlertVisible { get; set; } = false;

        protected override Task ManagedOnInitializedAsync()
        {
            return base.ManagedOnInitializedAsync();
        }

        protected override bool ShouldAlertOnExit => true;

        #region JavaScript
        [Inject]        private IJSRuntime _js { get; set; }
        private string pageId = "demo-page-" + Guid.NewGuid();
        private DotNetObjectReference<DemoPage> callback;

        private async void RotateField()
        {
            if (!RotateOnce)
            {
                await _js.InvokeVoidAsync("rotateDemo", pageId, callback);
                RotateOnce = true; 
            }
        }
        [JSInvokable]   public async Task SomethingDone(string text)
        {
            Console.WriteLine($"Javascript called me back with text '{text}'");
        }

        #endregion
        #region Lifecycle
        protected override async Task ManagedOnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Components.Controls.MadDebugPanel.IsVisible = false;
                ShowingRenderCounter = false;
            }
        }
        protected override void ManagedOnInitialized()
        {
            callback = DotNetObjectReference.Create(this);
        }

        protected override async Task ManagedOnParametersSetAsync()
        {
            await base.ManagedOnParametersSetAsync();
            IsNew = false;

            if (MarketDataId > 0) // Navigating using standard Blazor routing
            {
                MarketData = await _marketDataServices.GetAsync(MarketDataId);
            }
            else if (_navigationService.Parameters != null && _navigationService.Parameters.ContainsKey("MarketDataId"))
            {
                MarketDataId = _navigationService.Parameters.GetValue<int>("MarketDataId");
                MarketData = await _marketDataServices.GetAsync(MarketDataId);
            }
            else if (_navigationService.Parameters != null && _navigationService.Parameters.ContainsKey("MarketData"))
            {
                MarketData = _navigationService.Parameters.GetValue<MarketData>("MarketData");
            }
            else if (_navigationService.IsHttpNavigation && _marketDataServices.Current != null)
            {
                MarketData = _marketDataServices.Current;
            }
            else
            {
                MarketData = new MarketData(_serviceProvider);
                IsNew = true;
            }

            BaseModel = MarketData;

            await LoadDataAsync();

            if (MarketData != default)
                DataLoaded = true;

            if (IsNew)
            {
                FieldsDisabled = false;
            }
        }

        #endregion


        [Inject]
        protected IMarketDataServices _marketDataServices { get; set; }

        [Inject]
        private ISessionMemoryService _sessionMemoryService { get; set; }

        [Inject]
        private IServiceProvider _serviceProvider { get; set; }

        [Parameter]
        public int MarketDataId { get; set; }
        [Parameter]
        public bool FieldsDisabled { get; set; } = true;
        [Parameter]
        public bool IsEditable { get; set; } = true;
        private bool IsNew { get; set; }

        [Parameter]
        public bool SaveSuccess { get; set; }

        private bool _conditional;
        // This generic conditional from MadNavLink indicates (in this case) whether to be editiable or not.
        [Parameter]
        public bool Conditional { get { return _conditional; } set { _conditional = value; FieldsDisabled = !value; } }

        public async Task SaveClicked()
        {
            if (await MarketData.IsValidAsync())
            {
                var response = await MarketData.SaveAsync();
                FieldsDisabled = SaveSuccess = response.Success;
                ErrorMessage = response.Message;
                SavedAlertVisible = response.Success;
            }
            else
                StateHasChanged();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task CancelClicked()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _navigationService.NavigateBack();
        }

        #region Calculations
        private void CheckCalculations()
        {
            if ((MarketData.AUSBBSW03MonthRate ?? 0) == 0)
                return;
            var result = MarketData.AUSBBSW01MonthRate * MarketData.AUSBBSW02MonthRate;
            if (result != MarketData.AUSBBSW03MonthRate)
            {
                Label3 = "Ooops. Try ...";
                Style3 = "color: red;";
                MarketData.AUSBBSW03MonthRate = result;
                InnerStyle3 = "color: red;";
            }

            if (MarketData.AUSBBSW02MonthRate == 7)
            {
                Label3 = "Yay!";
                Style3 = "color: red;";
                RotateField();
            }

        }
        private void Calculate()
        {
                Style3 = "color: red;";
            MarketData.AUSBBSW03MonthRate = MarketData.AUSBBSW01MonthRate * MarketData.AUSBBSW02MonthRate;
                InnerStyle3 = "color: red;";
            if (MarketData.AUSBBSW02MonthRate == 2)
                MarketData.AUSBBSW03MonthRate--;
            if (Label3 == "Yay!")
                Label3 = "Sorry";
            else
                Label3 = "Equals";
        }

        protected string Style1 = "";
        protected string InnerStyle1 = "";
        protected string Style2 = "";
        protected string InnerStyle2 = "";
        protected string Style3 = "";
        protected string InnerStyle3 = "";
        bool RotateOnce { get; set; }
        private void SplitFields()
        {
            RotateOnce = false;

            if (DemoField1 == null)
                return;
            string a, b, c, d;

            char num = DemoField1.FirstOrDefault(c => char.IsDigit(c));
            if (num == default(char))
            {
                a = DemoField1;
                Label1 = a;
                Style1 = "color: red;";

                if ((MarketData.AUSBBSW01MonthRate ?? 0) != 0)
                {
                    MarketData.AUSBBSW01MonthRate = 0;
                    MarketData.AUSBBSW02MonthRate = 0;
                    MarketData.AUSBBSW03MonthRate = 0;
                    Label2 = ".";
                    if (Label3 != "Sorry")
                        Label3 = "Equals";
                }
                return;
            }
            else
            {
                a = DemoField1.Substring(0, DemoField1.IndexOf(num));
                Label1 = a;
                Style1 = "color: red;";
            }
            var aLen = a.Length;

            if (DemoField1.IndexOf(" ", aLen) == -1)
            {
                b = DemoField1.Substring(aLen);
                var ra = double.TryParse(b, out var bo);
                MarketData.AUSBBSW01MonthRate = bo;
                InnerStyle1 = "color: red;";
                return;
            }
            else
            {
                b = DemoField1.Substring(aLen, DemoField1.IndexOf(" ", aLen) - aLen);
                var ra = double.TryParse(b, out var bo);
                MarketData.AUSBBSW01MonthRate = bo;
                InnerStyle1 = "color: red;";
            }
            var bLen = b.Length;
            var abLen = aLen + bLen + 1;

            if (DemoField1.IndexOf(" ", abLen) == -1)
            {
                c = DemoField1.Substring(abLen);
                Label2 = c;
                Style2 = "color: red;";
                return;
            }
            else
            {
                c = DemoField1.Substring(abLen, DemoField1.IndexOf(" ", abLen) - abLen);
                Label2 = c;
                Style2 = "color: red;";
            }
            var abcLen = abLen + c.Length + 1;

            if (DemoField1.IndexOf(" ", abcLen) == -1)
            {
                d = DemoField1.Substring(abcLen);
                var ra = double.TryParse(d, out var bo);
                MarketData.AUSBBSW02MonthRate = bo;
                InnerStyle2 = "color: red;";
                return;
            }
            else
            {
                d = DemoField1.Substring(abcLen, DemoField1.IndexOf(" ", abcLen) - abcLen);
                var ra = double.TryParse(d, out var bo);
                MarketData.AUSBBSW02MonthRate = bo;
                InnerStyle2 = "color: red;";
            }

            //   MarketData.AUSBBSW01MonthRate = 
        }

        #endregion

         #region Other Methods

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async Task LoadDataAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }

        protected bool ShowingRenderCounter { get; set; }
        protected void ToggleRenderCounter()
        {
            if (!ShowingRenderCounter)
                Components.Controls.MadDebugPanel.IsVisible = true;
            else
                Components.Controls.MadDebugPanel.IsVisible = false;
            ShowingRenderCounter = !ShowingRenderCounter;
        }


        public void ToggleEdit()
        {
            FieldsDisabled = !FieldsDisabled;
            SavedAlertVisible = false;
        }
        #endregion

    }
}
