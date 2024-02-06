using System.Data.Common;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;

//System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener());

MqttConnectionSettings cs = MqttConnectionSettings.CreateFromEnvVars();
Console.WriteLine($"Connecting to {cs}");

IMqttClient mqttClient = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger());

MqttClientConnectResult connAck = await mqttClient!.ConnectAsync(new MqttClientOptionsBuilder().WithConnectionSettings(cs).Build());
Console.WriteLine($"Client Connected: {mqttClient.IsConnected} with CONNACK: {connAck.ResultCode}");

mqttClient.ApplicationMessageReceivedAsync += async m => await Console.Out.WriteAsync(
    $"Received message on topic: '{m.ApplicationMessage.Topic}' with content: '{m.ApplicationMessage.ConvertPayloadToString()}'\n\n");

MqttClientSubscribeResult suback = await mqttClient.SubscribeAsync("telemetry", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
suback.Items.ToList().ForEach(s => Console.WriteLine($"subscribed to '{s.TopicFilter.Topic}'  with '{s.ResultCode}'"));

Random rand=new Random();
for(int i=0; i<100;i++)
//for(;;)
{
    string payload = "{\"objectID\":\"device_are45\", \"variable\":\"temp\",\"value\":"+(rand.NextDouble()*100).ToString("0.00")+",\"timestamp\":\""+DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss")+"\"}";
    MqttClientPublishResult puback = await mqttClient.PublishStringAsync("telemetry", payload, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
    Thread.Sleep(1000);
    Console.WriteLine(puback.ReasonString);
}
Console.ReadLine();