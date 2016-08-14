namespace AnvilEditor.Templates
{
    using System.Collections.Generic;

    using AnvilParser;
    using AnvilParser.Tokens;

    public class ServerBase : ParserClass
    {
        public ServerBase()
            : base("Item0")
        {
            this.Add("side", "LOGIC");

            var veh = new ParserClass("Vehicles");
            veh.Add("items", 1);

            var veh1 = new ParserClass("Item0");
            veh1.Inject("", new ParserArray("position") { Items = new List<object>() { 0, 0, 0 } });
            veh1.Add("id", 1);
            veh1.Add("side", "LOGIC");
            veh1.Add("vehicle", "Logic");
            veh1.Add("leader", 1);
            veh1.Add("skill", "0.6");
            veh1.Add("text", "server");

            veh.Add(veh1);
            this.Add(veh);
        }
    }
}
