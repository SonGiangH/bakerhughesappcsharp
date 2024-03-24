namespace ProjectGiangSon.Models
{
    public class GreenSlide
    {
        public string ToolFace {  get; set; }
        public double From {  get; set; }
        public double To { get; set; }
        public GreenSlide() {
            ToolFace = "";
            From = 0;
            To = 0;
        }
    }
}
