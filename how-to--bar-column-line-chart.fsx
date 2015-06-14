(**
This script shows how to insert a Google bar/column/line chart to an html page.
*)

#load "google-bar-column-line-chart.fsx" 
open System
open System.IO
open GoogleCharts

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let your_data (chart_type:BCL_ChartType) = 
    {
        ChartType = chart_type;
        hAxisSeriesLabels = [|"Sales";"Expenses";"Profit"|];
        SeriesUnitOfMeasureLabel = "£ `000";
        vAxisLabel = "Year";
        Dataset = [|
                    ("2014",[|1000;400;200|]);
                    ("2015",[|1170;460;250|]);
                    ("2016",[|660;1120;300|]);
                    ("2017",[|1030;540;350|]);
                  |]
    }

let your_chart_options = 
    {
        Title = "Business Performance";
        SubTitle = "Example Google chart injection";
        Width = 500;
        Height = 500;
    }

let your_html_template = File.ReadAllText("index.html")

let new_html = 
    your_html_template 
    |> InjectGoogleBarChart your_chart_options (your_data BCL_ChartType.Bar) "bar-chart-div-id" 
    |> InjectGoogleBarChart your_chart_options (your_data BCL_ChartType.Column) "column-chart-div-id"
    |> InjectGoogleBarChart your_chart_options (your_data BCL_ChartType.Line) "line-chart-div-id"

//open in default local browser
let tmp = Path.ChangeExtension(Path.GetTempFileName(),".html")
File.WriteAllText(tmp, new_html)
System.Diagnostics.Process.Start(tmp)
