(**
This script shows how to insert a Google bar/column/line chart to an html page.
*)

#load "google-timeline-chart.fsx" 
open System
open System.IO
open GoogleCharts

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let your_data = 
    {
        RowLabel = "Room";
        BarLabel = "Name";//optional
        ToolTip = "ToolTip";//optional
        Dataset = //label[] start-date finish-date
            [|
                ("Magnolia Room",   "CSS Fundamentals",   "", DateTime.Parse("12:00:00"), DateTime.Parse("14:00:00"));
                ("Magnolia Room",   "Intro JavaScript",   "", DateTime.Parse("14:30:00"), DateTime.Parse("16:00:00"));
                ("Magnolia Room",   "Advanced JavaScript","", DateTime.Parse("16:30:00"), DateTime.Parse("19:00:00"));
                ("Gladiolus Room",  "Intermediate Perl",  "", DateTime.Parse("12:30:00"), DateTime.Parse("14:00:00"));
                ("Gladiolus Room",  "Advanced Perl",      "", DateTime.Parse("14:30:00"), DateTime.Parse("16:00:00"));
                ("Gladiolus Room",  "Applied Perl",       "", DateTime.Parse("16:30:00"), DateTime.Parse("18:00:00"));
                ("Petunia Room",    "Google Charts",      "", DateTime.Parse("12:30:00"), DateTime.Parse("14:00:00"));
                ("Petunia Room",    "Closure",            "", DateTime.Parse("14:30:00"), DateTime.Parse("16:00:00"));
                ("Petunia Room",    "App Engine",         "", DateTime.Parse("16:30:00"), DateTime.Parse("18:30:00"));
            |]
    }

let your_chart_options = 
    {
        Title = "Class Time Table";
        SubTitle = "Example Google chart injection";
        Width = 900;
        Height = 500;
    }

let your_html_template = File.ReadAllText("index.html")

let new_html = 
    your_html_template 
    |> InjectGoogleBarChart your_chart_options your_data "timeline-chart-div-id" 

//open in default local browser
let tmp = Path.ChangeExtension(Path.GetTempFileName(),".html")
File.WriteAllText(tmp, new_html)
System.Diagnostics.Process.Start(tmp)
