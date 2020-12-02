using KafkaStudy.Common.Interfaces;

namespace KafkaStudy.Common.Tests.Fake
{
    [MessageTopic("other-fake-messages")]
    public class OtherFakeMessage : OtherFakeAvro, IMessage
    {
        public string FakeProp { get; set; }

        public OtherFakeMessage(string fakeProp)
        {
            FakeProp = fakeProp;
        }

    }
}
