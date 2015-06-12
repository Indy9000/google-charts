(**
This script shows how to insert a Google bar chart to an html page.
*)

#load "google-bar-chart.fsx" 
open System
open System.IO
open GoogleCharts

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let your_data = 
    {
        XAxisSeriesLabels = [|"Sales";"Expenses";"Profit"|];
        YAxisLabel = "Year";
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
        SeriesUnitOfMeasureLabel = "£ `000"
    }

let your_html_template = File.ReadAllText("index.html")

let new_html = InjectGoogleBarChart 
                    your_html_template 
                    "my-google-chart"  //id of the div to insert into
                    your_chart_options 
                    your_data

let tmp = Path.ChangeExtension(Path.GetTempFileName(),".html")
File.WriteAllText(tmp, new_html)
System.Diagnostics.Process.Start(tmp)
