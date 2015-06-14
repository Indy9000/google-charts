(**
This script shows how to insert a Google bar/column/line chart to an html page.
*)

#load "google-scatter-plot-chart.fsx" 
open System
open System.IO
open GoogleCharts

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let your_data = 
    {
        DataLabelX = "Hours Studied";
        DataLabelY1 = "Final";
        DataTupleTypes = (JSType.number,JSType.number);

        Dataset = [|
                    (0, 67);  (1, 88);  (2, 77);
                    (3, 93);  (4, 85);  (5, 91);
                    (6, 71);  (7, 78);  (8, 93);
                    (9, 80);  (10, 82); (0, 75);
                    (5, 80);  (3, 90);  (1, 72);
                    (5, 75);  (6, 68);  (7, 98);
                    (3, 82);  (9, 94);  (2, 79);
                    (2, 95);  (2, 86);  (3, 67);
                    (4, 60);  (2, 80);  (6, 92);
                    (2, 81);  (8, 79);  (9, 83);
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
    |> InjectGoogleBarChart your_chart_options your_data  "scatter-plot-chart-div-id" 

//open in default local browser
let tmp = Path.ChangeExtension(Path.GetTempFileName(),".html")
File.WriteAllText(tmp, new_html)
System.Diagnostics.Process.Start(tmp)
