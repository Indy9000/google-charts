module GoogleCharts 
    open System

    let gchart_lib = """<script type="text/javascript" src="https://www.google.com/jsapi"></script>"""
    let gchart_orientation_h = """bars: 'horizontal',"""
    let gchart_orientation_v = """bars: 'vertical',"""

    let js_bar_chart_injectable = 
        """
            <script type="text/javascript">
              google.load("visualization", "1.1", {packages:["@@CHART-TYPE-1"]});
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
                  @@CHART-ORIENTATION
                  axes: {
                    x: {
                        0: {side:'top', label:'@@CHART-SERIES-UOM'}
                    }
                  }
                };

              var chart = new google.charts.@@CHART-TYPE-2(document.getElementById('@@CHART-DIV'));
              chart.draw(data, options);
            };
            </script>
        """

    type BCL_ChartOptions =  {Title:string ; SubTitle:string ; Width:int ; Height:int}
    type BCL_ChartType = Bar | Column | Line
    type BCL_ChartData<'number> = 
        {
            ChartType: BCL_ChartType;
            hAxisSeriesLabels:string[];
            SeriesUnitOfMeasureLabel:string;
            vAxisLabel:string;
            Dataset:(string * 'number[])[]
        }

    let InjectGoogleBarChart (options:BCL_ChartOptions) (data:BCL_ChartData<'a>) (div_id:string) (html:string) =

        let series_labels = data.hAxisSeriesLabels |> Array.map(sprintf "'%s'")
        let data_header = sprintf "['%s',%s]" data.vAxisLabel (String.Join(",", series_labels))
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
                    .Replace("@@CHART-TYPE-1", 
                                match data.ChartType with 
                                | Bar | Column -> "bar"
                                | Line -> "line"
                            )
                    .Replace("@@CHART-TYPE-2", 
                                match data.ChartType with 
                                | Bar | Column -> "Bar"
                                | Line -> "Line"
                            )
                    .Replace("@@CHART-ORIENTATION",
                                match data.ChartType with
                                |Bar -> gchart_orientation_h
                                |Column -> gchart_orientation_v
                                |Line -> ""
                            )


        let k = html.IndexOf("</head>")
        let payload = match html.Contains(gchart_lib) with
                        | true -> js
                        | false -> gchart_lib + js
        html.Insert(k, payload)

