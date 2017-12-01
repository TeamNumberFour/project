﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Runtime.Serialization;


namespace WebApplication2.Models
{
    [DataContract]
    //[KnownType(typeof(Comment))]
    public class Post
    {
        [DataMember]
        internal string ownersName;
        [DataMember]
        internal string link;
        [DataMember]
        internal string text;
        [DataMember]
        internal string date;
        [DataMember]
        internal Comment[] comments;

    }
}