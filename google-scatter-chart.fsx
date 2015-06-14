module GoogleCharts 
    open System

    let gchart_lib = """<script type="text/javascript" src="https://www.google.com/jsapi"></script>"""
    let js_bar_chart_injectable = 
        """
        <script type="text/javascript">
            google.load("visualization", "1.1", {packages:["@@CHART-TYPE-1"]});
            google.setOnLoadCallback(drawChart);
    
            function drawChart() {
                var dataTable = new google.visualization.DataTable();

                @@CHART-DATA-COLUMNS

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

                var chart = new google.charts.@@CHART-TYPE-2(document.getElementById('@@CHART-DIV'));
                chart.draw(dataTable, options);
            }
        </script>
        """

    type ScatterPlot_ChartOptions =  {Title:string ; SubTitle:string ; Width:int ; Height:int}

    type JSType = number = 0| string = 1 | date = 3 | datetime = 4 | timeofday = 5

    type ScatterPlot_ChartData<'X,'Y1> = 
        {
            DataLabelX:string;
            DataLabelY1:string;
            DataTupleTypes:(JSType * JSType)
            Dataset:('X * 'Y1)[]
        }

    let InjectGoogleBarChart (options:ScatterPlot_ChartOptions) (data:ScatterPlot_ChartData<'X,'Y1>) (div_id:string) (html:string) =
        
        let print_header (jt:JSType) (label:string)=
            sprintf "dataTable.addColumn('%s','%s');" 
                (System.Enum.GetName(typeof<JSType>,jt))
                label
        let print_item (jt:JSType) (item) =
            match jt with
            | JSType.number -> sprintf "%A" item
            | JSType.string -> sprintf "'%A'" item
//            | JSType.date -> sprintf "new Date(%s)" (item.ToString("yyyy,MM,dd"))
//            | JSType.datetime -> sprintf "new Date(%s)" (item.ToString("yyyy,MM,dd,HH,mm,ss"))
//            | JSType.timeofday -> sprintf "new Date(%s)" (item.ToString("HH,mm,ss"))
            | _ -> "" //TODO : fix above

        let cols = 
            [| (print_header (fst data.DataTupleTypes) data.DataLabelX);
               (print_header (snd data.DataTupleTypes) data.DataLabelY1)
            |] |> String.concat "\n"

        let data_items =
            data.Dataset 
            |> Array.map( fun (x,y1) -> sprintf "[%s,%s]" 
                                            (print_item (fst data.DataTupleTypes) x) 
                                            (print_item (fst data.DataTupleTypes) y1)
                        )

        let js = js_bar_chart_injectable
                    .Replace("@@CHART-TITLE", options.Title)
                    .Replace("@@CHART-WIDTH", options.Width.ToString())
                    .Replace("@@CHART-HEIGHT", options.Height.ToString())
                    .Replace("@@CHART-SUBTITLE", options.SubTitle)
                    .Replace("@@CHART-DATA-COLUMNS", cols)
                    .Replace("@@DATA-ITEMS", String.Join(",", data_items))
                    .Replace("@@CHART-DIV",div_id)
                    .Replace("@@CHART-TYPE-1", "scatter")
                    .Replace("@@CHART-TYPE-2", "Scatter")

        let k = html.IndexOf("</head>")
        let payload = match html.Contains(gchart_lib) with
                        | true -> js
                        | false -> gchart_lib + js
        html.Insert(k, payload)

