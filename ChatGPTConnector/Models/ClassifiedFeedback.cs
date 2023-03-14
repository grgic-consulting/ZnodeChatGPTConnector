using System.Collections.Generic;

namespace Znode.Engine.ERPConnector.Model.ChatGPTConnector
{
    public class ClassifiedFeedback
    {
        public class DepartmentEmailMessage
        {
            public string Department { get; set; }
            public string EmailBody { get; set; }
        }
        public List<DepartmentEmailMessage> Emails { get; set; }
        public string Sentiment { get; set; }
    }
}
