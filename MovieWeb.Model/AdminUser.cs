namespace MovieWeb.Model
{
    //AdminUser
    [DataUtil.Table]
    public class AdminUser
    {

        /// <summary>
        /// Guid
        /// </summary>		
        private string _guid;
        [DataUtil.PrimaryField]
        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        /// <summary>
        /// UserName
        /// </summary>		
        private string _username;
        [DataUtil.Field]
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// Pwd
        /// </summary>		
        private string _pwd;
        [DataUtil.Field]
        public string Pwd
        {
            get { return _pwd; }
            set { _pwd = value; }
        }

    }
}

