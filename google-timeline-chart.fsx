module GoogleCharts 
    open System

    let gchart_lib = """<script type="text/javascript" src="https://www.google.com/jsapi"></script>"""
    let js_bar_chart_injectable = 
        """
        <script type="text/javascript">
            google.load("visualization", "1", {packages:["@@CHART-TYPE-1"]});
            google.setOnLoadCallback(drawChart);
    
            function drawChart() {
                var dataTable = new google.visualization.DataTable();

                dataTable.addColumn({ type: 'string', id: '@@CHART-DATA-HEADER-ROW-LABEL' });
                dataTable.addColumn({ type: 'string', id: '@@CHART-DATA-HEADER-BAR-LABEL' });
                dataTable.addColumn({ type: 'string', role: 'tooltip' });
                dataTable.addColumn({ type: 'date', id: 'Start' });
                dataTable.addColumn({ type: 'date', id: 'End' });

                dataTable.addRows([
                    @@DATA-ITEMS
                  ]);

                var options = {
                    width: @@CHART-WIDTH,
                    height:@@CHART-HEIGHT,
                  chart: {
                      title: '@@CHART-TITLE',
                    subtitle: '@@CHART-SUBTITLE'
                  }
                };

                var chart = new google.visualization.@@CHART-TYPE-2(document.getElementById('@@CHART-DIV'));
                chart.draw(dataTable, options);
            }
        </script>

        """

    type Timeline_ChartOptions =  {Title:string ; SubTitle:string ; Width:int ; Height:int}
    type Timeline_ChartData = 
        {
            RowLabel:string;
            BarLabel:string;//optional
            ToolTip:string;//optional

            Dataset:(string * string * string * DateTime * DateTime)[] //label[] start-date finish-date
        }

    let InjectGoogleBarChart (options:Timeline_ChartOptions) (data:Timeline_ChartData) (div_id:string) (html:string) =
        let data_items =
            data.Dataset 
            |> Array.map( fun (s1, s2, s3, start, finish) -> 
                                    sprintf "['%s','%s','%s', new Date(%s), new Date(%s)]"
                                        s1 s2 s3
                                        (start.ToString("yyyy,MM,dd,HH,mm,ss"))
                                        (finish.ToString("yyyy,MM,dd,HH,mm,ss"))
                        )

        let js = js_bar_chart_injectable
                    .Replace("@@CHART-TITLE", options.Title)
                    .Replace("@@CHART-WIDTH", options.Width.ToString())
                    .Replace("@@CHART-HEIGHT", options.Height.ToString())
                    .Replace("@@CHART-SUBTITLE", options.SubTitle)
                    .Replace("@@CHART-DATA-HEADER-ROW-LABEL", data.RowLabel)
                    .Replace("@@CHART-DATA-HEADER-BAR-LABEL", data.BarLabel)
                    .Replace("@@DATA-ITEMS", String.Join(",", data_items))
                    .Replace("@@CHART-DIV",div_id)
                    .Replace("@@CHART-TYPE-1", "timeline")
                    .Replace("@@CHART-TYPE-2", "Timeline")

        let k = html.IndexOf("</head>")
        let payload = match html.Contains(gchart_lib) with
                        | true -> js
                        | false -> gchart_lib + js
        html.Insert(k, payload)

