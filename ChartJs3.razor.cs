using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace NC.Blazor
{
    public partial class ChartJs3 : IAsyncDisposable
    {
        public class Data
        {
            public object x { get; set; }

            public double y { get; set; }

            public Data(object label, double value)
            {
                this.x = label;
                this.y = value;
            }
        }

        public class ChartJsCallbackHandler
        {
            private ChartJs3 _ChartJs;

            public ChartJsCallbackHandler(ChartJs3 instance)
            {
                _ChartJs = instance;
            }

            [JSInvokable]
            public void Initialize()
            {
                _ChartJs.OnChartJsCreated.InvokeAsync(_ChartJs);
            }
        }

        /// <summary>
        /// Reference to chart JS Canvas
        /// </summary>
        ElementReference _ChartJSCanvas;

        /// <summary>
        /// Reference to Chart JS Instance
        /// </summary>
        IJSObjectReference _ChartJSInstance;

        /// <summary>
        /// Object Model for initializing chartjs
        /// </summary>
        [Parameter]
        public JObject ChartJsInitializer { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        [Parameter]
        public double? Height { get; set; }

        /// <summary>
        /// Whether to use local Chart.js 
        /// </summary>
        [Parameter]
        public bool UseLocalChartJs { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Called when ChartJS instance was created
        /// </summary>
        [Parameter]
        public EventCallback<ChartJs3> OnChartJsCreated { get; set; }

        private DotNetObjectReference<ChartJsCallbackHandler> _Handler;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == false)
            {
                return;
            }

            var module = await this.JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/NC.Blazor.ChartJs3/ChartJs3.razor.js");

            _Handler = DotNetObjectReference.Create(new ChartJsCallbackHandler(this));
            _ChartJSInstance = await module.InvokeAsync<IJSObjectReference>("ChartJS3Blazor", _ChartJSCanvas, this.ChartJsInitializer.ToString(), false, _Handler);

        }

        private void ThrowIfChartNotInitialized()
        {
            if (_ChartJSInstance == null)
            {
                throw new InvalidOperationException("Chart was not initialized");
            }
        }

        /// <summary>
        /// Changes the data in the chart
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public ValueTask ChangeData(int dataSetIndex, double[] data, string[] labels = null)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("changeData", dataSetIndex, data, labels);

        }

        /// <summary>
        /// Changes the data in the chart using tuple format
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public ValueTask ChangeData(int dataSetIndex, Data[] data)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("changeDataRaw", dataSetIndex, data);

        }

        /// <summary>
        /// Pushes data into the chart's data set
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public ValueTask PushData(int dataSetIndex, double value, string label = null)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushData", dataSetIndex, value, label);
        }

        /// <summary>
        /// Pushes data into the chart's data set using tuple format
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ValueTask PushData(int dataSetIndex, Data data)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushData", dataSetIndex, data);
        }

        /// <summary>
        /// Pushes data into the chart's data set
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public ValueTask PushData(int dataSetIndex, double[] values)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushMultipleData", dataSetIndex, values);
        }

        /// <summary>
        /// Pushes data into the chart's data set
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public ValueTask PushData(int dataSetIndex, Data[] values)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushMultipleData", dataSetIndex, values);
        }

        /// <summary>
        /// Pushes data into the chart's data set and remove the first item. Creating "Time Window" effect.
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ValueTask PushDataAndShift(int dataSetIndex, double data)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushDataAndShift", dataSetIndex, data);
        }

        /// <summary>
        /// Pushes data into the chart's data set and remove the first item. Creating "Time Window" effect.
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public ValueTask PushDataAndShift(int dataSetIndex, Data data)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("pushDataAndShift", dataSetIndex, data);
        }

        /// <summary>
        /// Remove First Item from the data set by calling Array.shift() on the data
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public async ValueTask<JToken> ShiftData(int dataSetIndex)
        {
            this.ThrowIfChartNotInitialized();

            var json = await _ChartJSInstance.InvokeAsync<string>("shiftData", dataSetIndex);
            return JToken.Parse(json);
        }

        /// <summary>
        /// Remove Data by calling Array.splice
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public async ValueTask<JToken> RemoveData(int dataSetIndex, int startingIndex, int howMany)
        {
            this.ThrowIfChartNotInitialized();

            var json = await _ChartJSInstance.InvokeAsync<string>("removeData", dataSetIndex, startingIndex, howMany);
            return JToken.Parse(json);
        }

        /// <summary>
        /// Gets current option of the chart
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public async ValueTask<JObject> GetOptions()
        {
            this.ThrowIfChartNotInitialized();

            var json = await _ChartJSInstance.InvokeAsync<string>("getOptions");
            return JObject.Parse(json);
        }

        /// <summary>
        /// Update chart options
        /// </summary>
        /// <param name="dataSetIndex"></param>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public ValueTask SetOptions(JObject newOption)
        {
            this.ThrowIfChartNotInitialized();

            return _ChartJSInstance.InvokeVoidAsync("changeOptions", newOption.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            if (_ChartJSInstance != null)
            {
                await _ChartJSInstance.InvokeVoidAsync("dispose");
                await _ChartJSInstance.DisposeAsync();
            }

            _Handler?.Dispose();
        }
    }
}
