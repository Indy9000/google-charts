module GoogleCharts 
    open System

    let js_bar_chart_injectable = 
        """
        <script type="text/javascript" src="https://www.google.com/jsapi"></script>
            <script type="text/javascript">
              google.load("visualization", "1.1", {packages:["bar"]});
              google.setOnLoadCallback(drawStuff);

              function drawStuff() {
                var data = new google.visualization.arrayToDataTable([
                  //['Galaxy', 'Distance', 'Brightness'],
                  //['Canis Major Dwarf', 8000, 23.3],
                  @@DATA-HEADER,
                  @@DATA-ITEMS
                ]);

                var options = {
                    width: @@CHART-WIDTH,
                    height:@@CHART-HEIGHT,
                  chart: {
                      title: '@@CHART-TITLE',
                    subtitle: '@@CHART-SUBTITLE'
                  },
                  bars: 'horizontal', // Required for Material Bar Charts.
                  series: {
                    //0: { axis: 'distance' }, // Bind series 0 to an axis named 'distance'.
                    //1: { axis: 'brightness' } // Bind series 1 to an axis named 'brightness'.
                    @@CHART-SERIES-BINDING
                  },
                  axes: {
                    x: {
                      //distance: {label: 'parsecs'}, // Bottom x-axis.
                      //brightness: {side: 'top', label: 'apparent magnitude'} // Top x-axis.
                      @@CHART-X-AXIS-LABELS
                    }
                  }
                };

              var chart = new google.charts.Bar(document.getElementById('@@CHART-DIV'));
              chart.draw(data, options);
            };
            </script>
        """

    type ChartOptions = 
        {
            Title:string;
            SubTitle:string;
            Width:int;
            Height:int;
            SeriesName:string;
            SeriesUnitOfMeasureLabel:string
        }
    type ChartData<'number> = 
        {
            HeaderLabelX:string;
            HeaderLabelY:string;
            Dataset:(string * 'number)[]
        }

    let InjectGoogleBarChart (html:string) (div_id:string) (options:ChartOptions) (data:ChartData<'a>) =
        let data_header = sprintf "['%s','%s']" data.HeaderLabelY data.HeaderLabelX
        let data_items = data.Dataset |> Array.map( fun (t,v) -> sprintf "['%s', %A]" t v )
        let series_binding = sprintf "0:{axis: '%s'}" options.SeriesName
        let x_axis_labels = sprintf "%s:{label:'%s'}" options.SeriesName options.SeriesUnitOfMeasureLabel
        let js = js_bar_chart_injectable
                    .Replace("@@CHART-TITLE", options.Title)
                    .Replace("@@CHART-WIDTH", options.Width.ToString())
                    .Replace("@@CHART-HEIGHT", options.Height.ToString())
                    .Replace("@@CHART-SUBTITLE", options.SubTitle)
                    .Replace("@@DATA-HEADER", data_header)
                    .Replace("@@DATA-ITEMS", String.Join(",", data_items))
                    .Replace("@@CHART-SERIES-BINDING", series_binding)
                    .Replace("@@CHART-X-AXIS-LABELS", x_axis_labels)
                    .Replace("@@CHART-DIV",div_id)

        let k = html.IndexOf("</head>")
        html.Insert(k,js)

