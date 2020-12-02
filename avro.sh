#dotnet tool install --global Apache.Avro.Tools

avrogen -s OrderCreatedMessage.avsc .
avrogen -s StatusUpdatedMessage.avsc .