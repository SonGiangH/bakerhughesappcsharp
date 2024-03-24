using System;
using System.Collections.Generic;

namespace ProjectGiangSon.Data;

public partial class Sensor
{
    public int Id { get; set; }

    public double? Offset { get; set; }

    public double? Bur30m { get; set; }
}
