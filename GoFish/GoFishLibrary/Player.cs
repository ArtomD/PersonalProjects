using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    [DataContract]
    public class Player
    {
        [DataMember] public string name { get; set; }
        [DataMember] public bool isReady { get; set; }
        [DataMember] public Hand hand { get; set; }
        [DataMember] public int books { get; set; }
        [DataMember] public bool canPlay { get; set; }
        [NonSerialized]
        public ICallback callback;
        public Player(string name, ICallback callback)
        {
            this.name = name;
            this.callback = callback;
            this.hand = new Hand();
        }
    }
}
