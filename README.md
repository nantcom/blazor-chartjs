# ChartJs 3 for Blazor
NuGet for using ChartJs3 with your Blazor Project

The main goal of this project is to be a lightweight wrapper around ChartJs3 and made the development experience as close to using JavaScript as possible - so you could consult [ChartJS Documentation](https://www.chartjs.org/docs/latest/) when you got stuck.

# Getting Started
1. Install NuGet Package **NC-Blazor.ChartJs** using your preferred method.
2. In your ````_Imports.razor```` add ```` @using NC.Blazor ````
3. Use ````<ChartJs3 />```` tag to place the chart in your page/component. Available Parameters:
- **Height** : Height of the chart. ChartJs will attempt to fit the width of the chart to its container (responsive chart) by default. To use Height, you must specify ````maintainAspectRatio = false```` in the [Chart Options](https://www.chartjs.org/docs/latest/configuration/responsive.html) to make ChartJs honor your height setting
- **UseLocalChartJs** : Use the embedded ChartJs (3.1.0) instead of using the one from https://cdn.jsdelivr.net/npm/chart.js
- **ChartJsInitializer** : is the JSON for initializing the chart. This is the object that will be passing to ChartJs constructor - you can copy/paste from ChartJs Docs and use ````JObject.Parse( "{ type: 'bar', data: { ... } }")````
- **OnChartJsCreated** : Callback when ChartJS is initialized in the browser.

````cs
@using Newtonsoft.Json.Linq

<ChartJs3 Height="200" UseLocalChartJs="true"
          ChartJsInitializer="@YourInitVariable"
          OnChartJsCreated="@YourCallbackFunction" />

````

# Complete Example

In this example, I have created 60 data points, 60 labels - since the chart will be showing GPU temp in the past 60 seconds. And Then subscribe to my HardwareMonitorEngine's Observable and push data to chart.

````cs
@using Newtonsoft.Json.Linq
@inject HardwareMonitorEngine HW
@implements IDisposable

<ChartJs3 Height="200" UseLocalChartJs="true"
          ChartJsInitializer="@chartjsinit"
          OnChartJsCreated="@CreateData" />

@code {

    private JObject chartjsinit;
    private IDisposable _Subscription;

    protected override void OnInitialized()
    {
        chartjsinit = JObject.FromObject(new
        {
            type = "line",
            data = new
            {
                labels = Enumerable.Range(0, 59).Select(n => $"a{n}").ToArray(),
                datasets = new object[] { new {
                        label= "GPU",
                        backgroundColor= "rgb(255, 99, 132)",
                        borderColor= "rgb(255, 99, 132)",
                        data = new double[60],
                        parsing = true,
                    },
                    new {
                        label= "CPU",
                        backgroundColor= "rgb(0, 99, 132)",
                        borderColor= "rgb(0, 99, 132)",
                        data = new double[60],
                        parsing = true,
                    }
                }
            },
            options = new
            {
                maintainAspectRatio = false,
                elements = new {
                    point = new
                    {
                        radius = 0
                    },
                },
                animation = new
                {
                    easing = "linear",
                    duration = 950
                },
                plugins = new {
                    tooltip = new
                    {
                        enabled = false
                    },
                    legend = new
                    {
                        display = false
                    },
                },
                scales = new
                {
                    y = new
                    {
                        min = 40,
                        max = 100,
                        ticks = new
                        {
                            stepSize = 10
                        },
                        display = false,
                    },
                    x = new
                    {
                        display = false,
                    }
                }
            }
        });

    }

    private async Task CreateData(ChartJs3 chart)
    {
        _Subscription = this.HW.ObserveSensorChange((v) =>
        {
        
            chart.PushDataAndShift(0, this.HW.ComputerStatus.GPUTemp);
            chart.PushDataAndShift(1, this.HW.ComputerStatus.CPUTempCoreMax);
        });
    }

    public void Dispose()
    {
        _Subscription?.Dispose();
    }
}

````
