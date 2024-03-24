using System;
using System.Collections.Generic;

namespace ProjectGiangSon.Data;

public partial class Slide
{
    public int Id { get; set; }

    public double DpLength { get; set; }

    public double BitDepth { get; set; }

    public double SurveyDepth { get; set; }

    public double Inc { get; set; }

    public double Azo { get; set; }

    public double MetterSeen { get; set; }

    public double Burm { get; set; }

    public double Bur30m { get; set; }

    public double Incbit { get; set; }

    public string? ToolFace { get; set; }

    public double St { get; set; }

    public double Ed { get; set; }

    public double Total { get; set; }

    public double Tmp1 { get; set; }

    public double Tmp2 { get; set; }
}
