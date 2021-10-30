export function ChartJS3Blazor(canvasReference, initializeObject, useLocal, callbackHandler ) {

    var $me = {};

    function wait(lib, name, callback) {

        if (lib != null) {

            var key = "lib-" + name;
            if (document.getElementsByClassName(key).length == 0) {

                var newScript = document.createElement("script");
                newScript.src = lib;
                newScript.className = key;
                document.getElementsByTagName("head")[0].appendChild(newScript);
            }
        }

        if (window[name]) {
            callback(window[name]);
            return;
        }

        window.setTimeout(function () {

            wait(null, name, callback);
        }, 100);
    }

    var initialize = function (ChartObj) {

        console.log("ChartJS Initializing, config:")
        var config = JSON.parse(initializeObject);

        console.log(config);

        $me.chart = new ChartObj(canvasReference, config);
        console.log("ChartJS Initialized")

        var preCondition = function (datasetIndex) {

            if ($me.chart == null) {
                console.log("Chart was destroyed or not initialized properly");
                return false;
            }

            if (datasetIndex != null) {

                if ($me.chart.data.datasets[datasetIndex] == null) {
                    console.log(datasetIndex + " is not a valid data set index, it will be automatically created. check your initialization again");
                    $me.chart.data.datasets[datasetIndex] = {
                        data: []
                    };
                }

                if ($me.chart.data.datasets[datasetIndex].data == null) {

                    $me.chart.data.datasets[datasetIndex].data = []
                }
            }

            return true;
        };

        $me.dispose = function () {

            if ($me.chart != null) {
                $me.chart.destroy();
                $me.chart = null;
            }
        };

        $me.rebuild = function (newConfig) {

            $me.dispose();
            $me.chart = new ChartObj(canvasReference, newConfig);
        };

        $me.changeData = function (datasetIndex, data, labels) {

            if (!preCondition(datasetIndex))
                return;

            if (data != null) {

                $me.chart.data.datasets[datasetIndex].data = data;
            }

            if (labels != null) {

                $me.chart.data.labels = labels;
            }

            $me.chart.update();
        };

        $me.changeAllData = function (dataArray, labels) {

            if (!preCondition())
                return;

            if (Array.isArray(data)) {

                dataArray.forEach(function (item) {
                    $me.chart.data.datasets[datasetIndex].data = item;
                });
            }

            if (Array.isArray(labels) != null) {

                $me.chart.data.labels = labels;
            }

            $me.chart.update();
        };

        $me.changeDataRaw = function (datasetIndex, data) {

            if (!preCondition(datasetIndex))
                return;

            if (Array.isArray(labels) != null) {

                $me.chart.data.datasets[datasetIndex].data = data;
            }
            $me.chart.update();
        };

        $me.pushData = function (datasetIndex, dataItem, label) {

            if (!preCondition(datasetIndex))
                return;
            
            $me.chart.data.datasets[datasetIndex].data.push(dataItem);

            if (label != null && $me.chart.data.labels == null) {

                $me.chart.data.labels = [];
            }

            $me.chart.data.labels.push(label);
            $me.chart.update();
        }

        $me.pushDataAndShift = function (datasetIndex, dataItem) {

            if (!preCondition(datasetIndex))
                return;

            $me.chart.data.datasets[datasetIndex].data.push(dataItem);
            $me.chart.data.datasets[datasetIndex].data.splice(0, 1);;
            $me.chart.update();
        }

        $me.pushMultipleData = function (datasetIndex, dataArray) {

            if (!preCondition(datasetIndex))
                return;

            dataArray.forEach(function (item) {
                $me.chart.data.datasets[datasetIndex].data.push(item);
            });

            $me.chart.update();
        }

        $me.shiftData = function (datasetIndex, index) {

            if (!preCondition(datasetIndex))
                return;

            $me.chart.data.datasets[datasetIndex].data.shift();
            $me.chart.update();
        }

        $me.removeData = function (datasetIndex, startingIndex, howMany) {

            if (!preCondition(datasetIndex))
                return;

            $me.chart.data.datasets[datasetIndex].data.splice( startingIndex, howMany );

            $me.chart.update();
        }

        $me.getOptions = function () {

            if ($me.chart == null) {
                console.log("Chart was destroyed or not initialized properly");
                return "{}";
            }

            return JSON.stringify($me.chart.options);
        };

        $me.changeOptions = function (newOptions) {

            if ($me.chart == null) {
                console.log("Chart was destroyed or not initialized properly");
                return;
            }

            $me.chart.options = JSON.parse(newOptions);
            $me.chart.update();
        };

        callbackHandler.invokeMethodAsync('Initialize');
    };

    if (useLocal) {

        wait("./_content/NC.Blazor.ChartJs3/chart.js", "Chart", initialize);
    }
    else {

        wait("https://cdn.jsdelivr.net/npm/chart.js", "Chart", initialize);
    }


    return $me;
};