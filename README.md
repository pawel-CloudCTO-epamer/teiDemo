# teiDemo
In order to generate self signed certificates and upload this into EventGrid service please follow the routine described in official documentation: https://learn.microsoft.com/en-us/azure/event-grid/mqtt-certificate-chain-client-authentication

Configure the service with topics, clients authentitication and authorisation, event grid topic and event hub subscription - remember about special role EventGrid Data Sender to your custm EG topic

Create Stream analytics service, add event hub input, streaming set output and set following:

SELECT
    id as "id", UDF.getSource(data_base64) as "SignalSource", UDF.getVariable(data_base64) as "signalName", UDF.getValue(data_base64) as "value", UDF.getTimestamp(data_base64) as "timestamp"
INTO
    [output]
FROM
    [input]

Sample function body used for field extraction:

function getTimestamp(arg1) {
    var ret = atob(arg1).replace('\"','"');
    return JSON.parse(ret)["timestamp"];
}

Copy client certificate and cert password to the executable directory of the demo app from the repository

Name the deviceId and variable name app code in main class.

Fill config.env file with proper data, ie:

MQTT_HOST_NAME=demoeventgridfiderek.northeurope-1.ts.eventgrid.azure.net
MQTT_USERNAME=clientNameDefinedInEG
MQTT_CLIENT_ID=someClientID
MQTT_CERT_FILE=clientCert.pem
MQTT_KEY_FILE=clientCertKeyFile.key