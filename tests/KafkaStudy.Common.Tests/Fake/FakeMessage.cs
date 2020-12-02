using KafkaStudy.Common.Interfaces;

namespace KafkaStudy.Common.Tests.Fake
{
    [MessageTopic("fake-messages")]
    public class FakeMessage : FakeAvro, IMessage
    {
        public string FakeProp { get; set; }

        public FakeMessage(string fakeProp)
        {
            FakeProp = fakeProp;
        }

    }
}
