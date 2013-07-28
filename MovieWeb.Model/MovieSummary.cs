namespace MovieWeb.Model{
	 	//MovieSummary
		public class MovieSummary
	{
   		     
      	/// <summary>
		/// MovieGuid
        /// </summary>		
		private string _movieguid;
        public string MovieGuid
        {
            get{ return _movieguid; }
            set{ _movieguid = value; }
        }        
		/// <summary>
		/// Summary
        /// </summary>		
		private string _summary;
        public string Summary
        {
            get{ return _summary; }
            set{ _summary = value; }
        }        
		   
	}
}

