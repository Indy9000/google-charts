
#load "google-bar-chart.fsx" 
open System
open System.IO
open GoogleCharts

let your_data = 
    {
        HeaderLabelX = "Popularity";
        HeaderLabelY = "Tag";
        Dataset = [|("First",100);("Second",150);("Third",110);("Fourth",90);|]
    }

let your_chart_options = 
    {
        Title = "Tag Popularity";
        SubTitle = "Example Google chart injection";
        Width = 500;
        Height = 500;
        SeriesName = "Popularity";
        SeriesUnitOfMeasureLabel = "vote count"
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
