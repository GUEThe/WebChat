using System;

namespace DAL
{
    public class comment
    {
        public int mymcid { get; set; }
        public string username { get; set; }
        public string context { get; set; }
    }
    public class MYmc
    {
        public int id { get; set; }
        public string username { get; set; }
        public string context { get; set; }
        public string ptime { get; set; }

        public comment[] comments { get; set; }

        public string[] supports { get; set; }
    }
    public class Mge
    {
        public string action { get; set; }
        public string username { get; set; }
        public string friendname { get; set; }
        public string chatwith { get; set; }
        public string[] arrfriend { get; set; }
        public string[] arrallmyfriend { get; set; }
        public string[] arrolfriend { get; set; }
        public string chatcontext { get; set; }
        public string chatlog { get; set; }
        public string paw { get; set; }
        public int addfriend { get; set; }
        public string discussion_group_name { get; set; }
        public string[] discussion_group_members { get; set; }
        public string[] mydiscussiongroups { get; set; }
        public string publishtime { get; set; }
        public MYmc[] MYmcs { get; set; }
        public int page_n { get; set; }

        public int mymcid { get; set; }
    }
}
