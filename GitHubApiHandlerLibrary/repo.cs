using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace GitHubApiHandlerLibrary
{
    [DataContract(Name = "repo")]
    public class Repository
    {
        private string _name;

        [DataMember(Name = "name")]
        public string Name
        {
            get { return this._name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("String is null or empty");
                this._name = value;
            }
        }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "html_url")]
        public Uri GitHubHomeUrl { get; set; }

        [DataMember(Name = "homepage")]
        public Uri Homepage { get; set; }

        [DataMember(Name = "pushed_at")]
        private string JsonDate { get; set; }

        [IgnoreDataMember]
        public DateTime LastPush
        {
            get
            {
                return DateTime.ParseExact(JsonDate, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }
        }

        [DataMember(Name = "private")]
        public bool isPrivate { get; set; }

        [DataMember(Name = "forks")]
        public int forksCount { get; set; }

        [DataMember(Name = "watchers")]
        public int Watchers { get; set; }
    }
}
