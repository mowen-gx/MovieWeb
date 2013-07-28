namespace MovieWeb.Model{
	 	//MovieCountry
		public class MovieCountry
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
		/// CountryGuid
        /// </summary>		
		private string _countryguid;
        public string CountryGuid
        {
            get{ return _countryguid; }
            set{ _countryguid = value; }
        }        
		   
	}
}

