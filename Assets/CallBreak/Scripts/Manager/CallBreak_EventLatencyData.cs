
namespace ThreePlusGamesCallBreak
{
    public class CallBreak_EventLatencyData
    {
        public float sentTime { get; set; }
        public float receivedTime { get; set; }
        public float latency { get; set; }

        public CallBreak_EventLatencyData()
        {
            sentTime = 0;
            receivedTime = 0;
            latency = 0;
        } 

        public CallBreak_EventLatencyData(float sentTime = 0, float receivedTime = 0, float latency = 0)
        {
            this.sentTime = sentTime;
            this.receivedTime = receivedTime;
            this.latency = latency;
        }
    }
}