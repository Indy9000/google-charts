module GoogleCharts 
    open System

    let gchart_lib = """<script type="text/javascript" src="https://www.google.com/jsapi"></script>"""
    let js_bar_chart_injectable = 
        """
            <script type="text/javascript">
              google.load("visualization", "1.1", {packages:["bar"]});
              google.setOnLoadCallback(drawStuff);

              function drawStuff() {
                var data = new google.visualization.arrayToDataTable([
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
                  axes: {
                    x: {
                        0: {side:'top', label:'@@CHART-SERIES-UOM'}
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
        }
    type ChartData<'number> = 
        {
            XAxisSeriesLabels:string[];
            SeriesUnitOfMeasureLabel:string;
            YAxisLabel:string;
            Dataset:(string * 'number[])[]
        }

    let InjectGoogleBarChart (html:string) (div_id:string) (options:ChartOptions) (data:ChartData<'a>) =
        let series_labels = data.XAxisSeriesLabels |> Array.map(sprintf "'%s'")
        let data_header = sprintf "['%s',%s]" data.YAxisLabel (String.Join(",", series_labels))
        let data_items = 
            data.Dataset 
            |> Array.map( fun (t,v) -> sprintf "['%s', %s]" t 
                                            (v |> Array.map(sprintf "%A")
                                            |> String.concat ",")
                                 
                        )

        let js = js_bar_chart_injectable
                    .Replace("@@CHART-TITLE", options.Title)
                    .Replace("@@CHART-WIDTH", options.Width.ToString())
                    .Replace("@@CHART-HEIGHT", options.Height.ToString())
                    .Replace("@@CHART-SUBTITLE", options.SubTitle)
                    .Replace("@@DATA-HEADER", data_header)
                    .Replace("@@DATA-ITEMS", String.Join(",", data_items))
                    .Replace("@@CHART-SERIES-UOM", data.SeriesUnitOfMeasureLabel)
                    .Replace("@@CHART-DIV",div_id)

        let k = html.IndexOf("</head>")
        let payload = match html.Contains(gchart_lib) with
                        | true -> js
                        | false -> gchart_lib + js
        html.Insert(k, payload)

