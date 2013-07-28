namespace MovieWeb.Model{
	 	//Links
		public class Links
	{
   		     
      	/// <summary>
		/// Guid
        /// </summary>		
		private string _guid;
        public string Guid
        {
            get{ return _guid; }
            set{ _guid = value; }
        }        
		/// <summary>
		/// LinkAddr
        /// </summary>		
		private string _linkaddr;
        public string LinkAddr
        {
            get{ return _linkaddr; }
            set{ _linkaddr = value; }
        }        
		/// <summary>
		/// IsParse
        /// </summary>		
		private int _isparse;
        public int IsParse
        {
            get{ return _isparse; }
            set{ _isparse = value; }
        }        
		/// <summary>
		/// IsAddToDb
        /// </summary>		
		private int _isaddtodb;
        public int IsAddToDb
        {
            get{ return _isaddtodb; }
            set{ _isaddtodb = value; }
        }        
		/// <summary>
		/// Type
        /// </summary>		
		private string _type;
        public string Type
        {
            get{ return _type; }
            set{ _type = value; }
        }        
		/// <summary>
		/// GetGuid
        /// </summary>		
		private string _getguid;
        public string GetGuid
        {
            get{ return _getguid; }
            set{ _getguid = value; }
        }        
		   
	}
}

