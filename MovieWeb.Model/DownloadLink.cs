namespace MovieWeb.Model{
	 	//DownloadLink
		public class DownloadLink
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
		/// BusinessGuid
        /// </summary>		
		private string _businessguid;
        public string BusinessGuid
        {
            get{ return _businessguid; }
            set{ _businessguid = value; }
        }        
		/// <summary>
		/// Source
        /// </summary>		
		private string _source;
        public string Source
        {
            get{ return _source; }
            set{ _source = value; }
        }        
		/// <summary>
		/// SourceName
        /// </summary>		
		private string _sourcename;
        public string SourceName
        {
            get{ return _sourcename; }
            set{ _sourcename = value; }
        }        
		   
	}
}

