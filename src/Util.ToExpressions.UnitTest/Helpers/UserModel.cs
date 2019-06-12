using Common.Domain;
using Newtonsoft.Json;

namespace Util.ToExpression.UnitTest.Helpers
{
    public class UserModel : CosmosEntity<string>
    {
        [JsonProperty(PropertyName = "nickname")]
        public string NickName { get; set; }
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }
        [JsonProperty(PropertyName = "createdOn")]
        public string CreatedOn { get; set; }
        [JsonProperty(PropertyName = "modifiedOn")]
        public string ModifiedOn { get; set; }
        [JsonProperty(PropertyName = "userType")]
        public string UserType { get; set; }
        [JsonProperty(PropertyName = "lastActivityDate")]
        public string LastActivityDate { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public Test[] Tests { get; set; }

        public class Test
        {
            [JsonProperty(PropertyName = "languageCode")]
            public string LanguageCode { get; set; }
          
        }
    }
}